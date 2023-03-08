using System.Text;
using LibNekoFS.Interfaces;

namespace LibNekoFS.Coded;

public class CodedInputFileStream : ICodedInputStream
{
    private readonly IInputFileStream _stream;

    public CodedInputFileStream(IInputFileStream stream)
    {
        _stream = stream;
    }

    public byte ReadByte()
    {
        var bytes = new byte[1];
        ReadBytes(bytes, 1);
        return bytes[0];
    }

    public short ReadShort() => (short) ReadUShort();

    public ushort ReadUShort()
    {
        var bytes = new byte[2];
        ReadBytes(bytes, 2);

        return (ushort)(bytes[0] | (bytes[1] << 8));
    }

    public int ReadInt() => (int) uint.RotateRight((uint) ReadVarInt(5), 1);

    public uint ReadUInt() => (uint) ReadVarInt(5);

    public long ReadLong() => (long) ulong.RotateRight(ReadULong(), 1);

    public ulong ReadULong() => ReadVarInt(10);

    public string ReadString()
    {
        var count = ReadInt();
        switch (count)
        {
            case < 0:
                throw new IOException("Could not read string: count < 0");
            case > 16384:
                throw new IOException("Could not read string: count > 16384");
        }

        var strBytes = new byte[count];
        ReadBytes(strBytes, count);

        return Encoding.UTF8.GetString(strBytes);
    }

    public void ReadBytes(byte[] buffer, int count)
    {
        var read = _stream.Read(buffer, count);
        if (read != count)
            throw new IOException("Failed to read requested amount of bytes.");
    }
    private ulong ReadVarInt(int maxLen)
    {
        ulong value = 0;
        var size = 0;
        uint currentByte;

        while (((currentByte = ReadByte()) & 0x80) == 0x80)
        {
            value |= (currentByte & 0x7f) << (size * 7);
            size++;
            if (size > maxLen)
                throw new IOException("Could not read number: Malformed VarInt");
        }

        return value | ((currentByte & 0x7f) << (size * 7));
    }

    private int ReadBytesUnchecked(byte[] buffer, int count) => _stream.Read(buffer, count);

    public void Dispose()
    {
        _stream.Dispose();
    }
}