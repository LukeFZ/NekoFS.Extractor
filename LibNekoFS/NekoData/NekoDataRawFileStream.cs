using LibNekoFS.Interfaces;

namespace LibNekoFS.NekoData;

public class NekoDataRawFileStream : IInputFileStream
{
    private readonly MemoryStream _stream;

    private readonly byte[] _backingBuffer;

    public long Position => _stream.Position;
    public long Size => _backingBuffer.Length;
    public IFile File { get; }

    public NekoDataRawFileStream(NekoDataFile file)
    {
        File = file;

        _backingBuffer = GC.AllocateUninitializedArray<byte>((int)file.Size);

        using var fileStream = ((NekoDataFileSystem)file.FileSystem).BackingFile.NewReadStream();
        fileStream.Seek((int)file.Entry.Offset, SeekOrigin.Begin);

        fileStream.Read(_backingBuffer, (int)file.Size);

        _stream = new MemoryStream(_backingBuffer);
    }

    public int Read(byte[] buffer, int count) => _stream.Read(buffer, 0, count);

    public void Seek(int offset, SeekOrigin origin) => _stream.Seek(offset, origin);

    public void Dispose()
    {
        _stream.Dispose();
        GC.SuppressFinalize(this);
    }

    public void CopyTo(Stream stream)
    {
        _stream.CopyTo(stream);
    }
}