using LibNekoFS.Interfaces;

namespace LibNekoFS.NekoData;

public readonly struct NekoDataHeader
{
    public readonly byte[] Header;
    public readonly int Version;

    public NekoDataHeader(ICodedInputStream input)
    {
        Header = new byte[24];
        input.ReadBytes(Header, 24);

        Version = input.ReadByte();
    }
}