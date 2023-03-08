using System.Diagnostics;
using LibNekoFS.Interfaces;

namespace LibNekoFS.NekoData;

public class NekoDataFile : IFile
{
    public CentralDirectory.Entry Entry { get; }
    public string Name { get; }
    public long Size => Entry.Size;
    public bool CanAccess => true;
    public IFileSystem FileSystem { get; }

    public NekoDataFile(IFileSystem fileSystem, string name, CentralDirectory.Entry entry)
    {
        FileSystem = fileSystem;
        Name = name;
        Entry = entry;
    }

    public IInputFileStream NewReadStream()
    {
        return Entry.Type switch
        {
            0 => new NekoDataRawFileStream(this),
            2 => new NekoDataLZ4StreamingFileStream(this),
            _ => throw new UnreachableException("Invalid neko entry type!")
        };
    }
}