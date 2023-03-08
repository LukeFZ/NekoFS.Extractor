using LibNekoFS.Coded;
using LibNekoFS.Interfaces;
using LibNekoFS.Memory;

namespace LibNekoFS.NekoData;

public class NekoDataFileSystem : IFileSystem
{
    public FileSystemType Type => FileSystemType.NekoData;

    private NekoDataHeader _header;
    private readonly CentralDirectory _centralDirectory;

    public IFile BackingFile { get; }

    public NekoDataFileSystem(IFile file)
    {
        _header = new NekoDataHeader();
        _centralDirectory = new CentralDirectory();
        BackingFile = file;
    }

    public void Init()
    {
        using var fileStream = BackingFile.NewReadStream();
        using var codedFileInputStream = new CodedInputFileStream(fileStream);
        _header = new NekoDataHeader(codedFileInputStream);

        fileStream.Seek(-10, SeekOrigin.End);

        var footer = new byte[10];
        codedFileInputStream.ReadBytes(footer, 10);

        var memFile = new MemoryFile();
        using var memFileOutputStream = memFile.NewWriteStream();
        using var memFileWriter = new CodedOutputFileStream(memFileOutputStream);

        memFileWriter.WriteBytes(footer.Reverse().ToArray());

        using var memFileStream = memFile.NewReadStream();
        using var memFileReader = new CodedInputFileStream(memFileStream);

        var centralDirectoryLength = memFileReader.ReadInt(); // Actual lib uses UInt and does conversion manually
        var currentPos = (int)memFileStream.Position;
        
        fileStream.Seek(-centralDirectoryLength - currentPos, SeekOrigin.End);

        if (_header.Version == 0)
        {
            _centralDirectory.Populate(codedFileInputStream);
        }
        else
        {
            var encryptedCentralDirectory = new byte[centralDirectoryLength];
            codedFileInputStream.ReadBytes(encryptedCentralDirectory, centralDirectoryLength);

            var cipher = NekoChaCha20.ForFiles(centralDirectoryLength);
            var decryptedCentralDirectory = new byte[centralDirectoryLength];
            cipher.ProcessBytes(encryptedCentralDirectory, decryptedCentralDirectory);

            var centralDirectoryMemFile = new MemoryFile();
            var memFileWriteStream = new CodedOutputFileStream(centralDirectoryMemFile.NewWriteStream());
            memFileWriteStream.WriteBytes(decryptedCentralDirectory);
            memFileWriteStream.Dispose();

            var centralDirectoryReader = new CodedInputFileStream(centralDirectoryMemFile.NewReadStream());
            _centralDirectory.Populate(centralDirectoryReader);
            centralDirectoryReader.Dispose();
        }
    }

    public IFile Open(string fileName)
    {
        var entry = _centralDirectory.Entries[fileName];

        return new NekoDataFile(this, fileName, entry);
    }

    public string[] GetFiles() => _centralDirectory.Entries.Keys.ToArray();
}