using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

namespace LibNekoFS;

public sealed class NekoChaCha20 : ChaChaEngine
{
    public NekoChaCha20(byte[] key, byte[] nonce, int counter)
    {
        Init(false, new ParametersWithIV(new KeyParameter(key), nonce));
        for (int i = 0; i < counter; i++)
            AdvanceCounter();
    }

    public static NekoChaCha20 ForFiles(int centralDirectoryLen)
    {
        var key = Convert.FromHexString("A5057F03AA62829AC7DC4C64F9F7F428B414E71516E2C3A8DE9C77F9880F2ED4");
        var iv = Convert.FromHexString("0200010901000100");
        return new NekoChaCha20(key, iv, centralDirectoryLen);
    }
}