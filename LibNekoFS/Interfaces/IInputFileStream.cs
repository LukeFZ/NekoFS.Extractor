namespace LibNekoFS.Interfaces;

public interface IInputFileStream : IDisposable
{
    public long Position { get; }
    public long Size { get; }
    public IFile File { get; }
    public int Read(byte[] buffer, int count);
    public void Seek(int offset, SeekOrigin origin);

    public void CopyTo(Stream stream);
}