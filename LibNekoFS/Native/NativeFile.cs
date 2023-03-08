using LibNekoFS.Interfaces;

namespace LibNekoFS.Native;

public class NativeFile : IFile
{
    public string Name => $"Native: {_fileInfo.FullName}";
    public long Size => _fileInfo.Length;
    public bool CanAccess => _fileInfo.Exists;

    public IFileSystem FileSystem { get; }

    private readonly FileInfo _fileInfo;

    public NativeFile(NativeFileSystem fileSystem, string name)
    {
        FileSystem = fileSystem;
        _fileInfo = new FileInfo(Path.Join(fileSystem.BaseDirectory, name));
    }

    public IInputFileStream NewReadStream() => new NativeFileStream(this, _fileInfo.OpenRead());
}