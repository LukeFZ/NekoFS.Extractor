namespace LibNekoFS.Interfaces;

public interface ICodedOutputStream : IDisposable
{
    public void WriteByte(byte data);
    public void WriteShort(short data);
    public void WriteUShort(ushort data);
    public void WriteInt(int data);
    public void WriteUInt(uint data);
    public void WriteLong(long data);
    public void WriteULong(ulong data);
    public void WriteString(string data);
    public void WriteBytes(byte[] data);
}