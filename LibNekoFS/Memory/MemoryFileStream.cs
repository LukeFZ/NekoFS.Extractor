using LibNekoFS.Interfaces;

namespace LibNekoFS.Memory;

public class MemoryFileStream : IInputFileStream
{
    public long Position => _stream.Position;
    public long Size => _stream.Length;
    public IFile File { get; }

    private readonly MemoryStream _stream;

    public MemoryFileStream(MemoryFile file, MemoryStream stream)
    {
        File = file;
        _stream = stream;
    }

    public int Read(byte[] buffer, int count) => _stream.Read(buffer, 0, count);

    public void Seek(int offset, SeekOrigin origin) => _stream.Seek(offset, origin);

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void CopyTo(Stream stream)
    {
        throw new NotImplementedException();
    }
}