namespace LibNekoFS.Interfaces;

public interface IFile
{
    public string Name { get; }
    public long Size { get; }
    public bool CanAccess { get; }
    public IFileSystem FileSystem { get; }
    public IInputFileStream NewReadStream();
}