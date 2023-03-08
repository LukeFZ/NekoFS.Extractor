using LibNekoFS.Interfaces;

namespace LibNekoFS.NekoData;

public class CentralDirectory
{
    public Dictionary<string, Entry> Entries { get; } = new();

    public readonly struct Entry
    {
        public readonly byte Type;
        public readonly long Size;
        public readonly long CompressedSize;
        public readonly uint Crc32;
        public readonly ulong Offset;
        public readonly int BlockCount;
        public readonly List<(long start, long end)> BlockList;

        public Entry(ICodedInputStream stream)
        {
            Type = stream.ReadByte();

            Size = stream.ReadLong();
            if (Type != 0)
                CompressedSize = stream.ReadLong();

            Crc32 = stream.ReadUInt();
            Offset = stream.ReadULong();

            BlockCount = stream.ReadInt();
            BlockList = new List<(long compressedSize, long decompressedSize)>();

            var lastCompOffset = 0L;
            var lastDecompOffset = 0L;

            for (int i = 0; i < BlockCount; i++)
            {
                var blockCompressedOffset = stream.ReadLong();
                var blockUncompressedOffset = stream.ReadLong();

                if (blockCompressedOffset == 0) continue;

                BlockList.Add((blockCompressedOffset - lastCompOffset, blockUncompressedOffset - lastDecompOffset));

                lastCompOffset = blockCompressedOffset;
                lastDecompOffset = blockUncompressedOffset;
            }

            BlockList.Add((CompressedSize - lastCompOffset, Size - lastDecompOffset));
        }

        public override string ToString()
        {
            return $"Type: {Type}, Size: {Size}, Compressed size: {CompressedSize}, Crc32: {Crc32}, Offset: {Offset}";
        }
    }

    public void Populate(ICodedInputStream stream)
    {
        var count = stream.ReadLong();
        for (int i = 0; i < count; i++)
        {
            var name = stream.ReadString();
            var entry = new Entry(stream);

            Entries.Add(name, entry);
        }
    }
}