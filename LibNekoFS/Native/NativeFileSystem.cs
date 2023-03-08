using LibNekoFS.Interfaces;

namespace LibNekoFS.Native;

public class NativeFileSystem : IFileSystem
{
    public FileSystemType Type => FileSystemType.Native;

    public string BaseDirectory { get; }

    public NativeFileSystem(string baseDirectory)
    {
        BaseDirectory = baseDirectory;
    }

    public IFile Open(string fileName) => new NativeFile(this, fileName);

    public string[] GetFiles() => Directory.GetFiles(BaseDirectory);
}