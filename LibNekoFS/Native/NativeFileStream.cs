using LibNekoFS.Interfaces;

namespace LibNekoFS.Native;

public class NativeFileStream : IInputFileStream
{
    private readonly FileStream _stream;
    public long Position => _stream.Position;
    public long Size => _stream.Length;

    public IFile File { get; }

    public NativeFileStream(NativeFile file, FileStream stream)
    {
        File = file;
        _stream = stream;
    }

    public int Read(byte[] buffer, int count)
    {
        return _stream.Read(buffer, 0, count);
    }

    public void Seek(int offset, SeekOrigin origin) => _stream.Seek(offset, origin);

    public void Dispose()
    {
        _stream.Dispose();
        GC.SuppressFinalize(this);
    }

    public void CopyTo(Stream stream)
    {
        throw new NotImplementedException();
    }
}