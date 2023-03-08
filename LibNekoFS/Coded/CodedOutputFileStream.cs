using System.Text;
using LibNekoFS.Interfaces;

namespace LibNekoFS.Coded;

public class CodedOutputFileStream : ICodedOutputStream
{
    private readonly IOutputFileStream _stream;

    public CodedOutputFileStream(IOutputFileStream stream)
    {
        _stream = stream;
    }

    public void WriteByte(byte data)
    {
        var bytes = new byte[1];
        bytes[0] = data;
        WriteBytes(bytes);
    }

    public void WriteShort(short data) => WriteUShort((ushort) data);

    public void WriteUShort(ushort data)
    {
       var bytes = new byte[2];
        bytes[0] = (byte)data;
        bytes[1] = (byte)(data >> 8);
        WriteBytes(bytes);
    }

    public void WriteInt(int data) => WriteVarInt(uint.RotateRight((uint)data, 63));

    public void WriteUInt(uint data) => WriteVarInt(data);

    public void WriteLong(long data) => WriteVarInt(ulong.RotateRight((ulong) data, 63));

    public void WriteULong(ulong data) => WriteVarInt(data);

    public void WriteString(string data)
    {
        var length = data.Length;
        if (length > 16384)
            throw new IOException("Could not write string: length > 16384");

        WriteInt(length);
        WriteBytes(Encoding.UTF8.GetBytes(data));
    }

    public void WriteBytes(byte[] data)
    {
        _stream.Write(data);
    }

    private void WriteVarInt(ulong value)
    {
        while ((value & 0x80) != 0)
        {
            WriteByte((byte) ((value & 0x7f) | 0x80));
            value >>= 7;
        }

        WriteByte((byte) value);
    }

    public void Dispose()
    {
        _stream.Dispose();
    }
}