using LibNekoFS.Interfaces;

namespace LibNekoFS.Memory;

public class MemoryFileOutputStream : IOutputFileStream
{
    public long Position => _stream.Position;
    public long Size => _stream.Length;
    public IFile File { get; }

    private readonly MemoryStream _stream;

    public MemoryFileOutputStream(MemoryFile file, MemoryStream stream)
    {
        File = file;
        _stream = stream;
    }

    public void Write(byte[] buffer) => _stream.Write(buffer);

    public void Seek(int offset, SeekOrigin origin) => _stream.Seek(offset, origin);

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}