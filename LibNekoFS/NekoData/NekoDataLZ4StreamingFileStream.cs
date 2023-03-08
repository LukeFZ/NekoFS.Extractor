using K4os.Compression.LZ4;
using LibNekoFS.Interfaces;

namespace LibNekoFS.NekoData;

public class NekoDataLZ4StreamingFileStream : IInputFileStream
{
    private readonly MemoryStream _stream;
    
    private readonly byte[] _backingBuffer;

    public long Position => _stream.Position;
    public long Size => _backingBuffer.Length;
    public IFile File { get; }

    public NekoDataLZ4StreamingFileStream(NekoDataFile file)
    {
        File = file;

        _backingBuffer = GC.AllocateUninitializedArray<byte>((int) file.Size);

        using var fileStream = ((NekoDataFileSystem) file.FileSystem).BackingFile.NewReadStream();
        fileStream.Seek((int)file.Entry.Offset, SeekOrigin.Begin);

        var decompTotalLen = 0L;

        foreach (var (compLen, decompLen) in file.Entry.BlockList)
        {
            var compBlock = new byte[compLen];
            fileStream.Read(compBlock, (int) compLen);

            LZ4Codec.Decode(compBlock, 0, (int) compLen, _backingBuffer, (int) decompTotalLen, (int) decompLen);
            decompTotalLen += decompLen;
        }

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