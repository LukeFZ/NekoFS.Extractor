namespace LibNekoFS.Interfaces;

public interface IFileSystem
{
    public FileSystemType Type { get; }
    public IFile Open(string fileName);
    public string[] GetFiles();
}