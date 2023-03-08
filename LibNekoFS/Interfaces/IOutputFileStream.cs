namespace LibNekoFS.Interfaces;

public interface IOutputFileStream : IDisposable
{
    public long Position { get; }
    public long Size { get; }
    public IFile File { get; }
    public void Write(byte[] data);
    public void Seek(int offset, SeekOrigin origin);
}