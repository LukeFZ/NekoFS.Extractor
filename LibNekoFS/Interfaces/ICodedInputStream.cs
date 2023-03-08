namespace LibNekoFS.Interfaces;

public interface ICodedInputStream : IDisposable
{
    public byte ReadByte();
    public short ReadShort();
    public ushort ReadUShort();
    public int ReadInt();
    public uint ReadUInt();
    public long ReadLong();
    public ulong ReadULong();
    public string ReadString();
    public void ReadBytes(byte[] buffer, int count);
}