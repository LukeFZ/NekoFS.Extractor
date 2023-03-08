using LibNekoFS.Interfaces;

namespace LibNekoFS.Memory;

public class MemoryFile : IFile
{
    public string Name => "Memory: temp";
    public long Size => _backingBuffer.Length;
    public bool CanAccess => true;
    public IFileSystem FileSystem => throw new NotSupportedException();

    private readonly byte[] _backingBuffer;

    public MemoryFile()
    {
        _backingBuffer = new byte[0x80000];
    }

    public IInputFileStream NewReadStream() => new MemoryFileStream(this, new MemoryStream(_backingBuffer));

    public IOutputFileStream NewWriteStream() => new MemoryFileOutputStream(this, new MemoryStream(_backingBuffer));
}