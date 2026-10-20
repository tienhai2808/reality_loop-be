using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Infrastructure.AI.Security;

internal sealed class AiKeyProtector(IOptions<AiKeyOptions> options) : IAiKeyProtector
{
    public string Protect(string apiKey, long userId, long connectionId)
    {
        var settings = options.Value;
        var key = Convert.FromBase64String(settings.Keys[settings.ActiveKeyId]);
        var plaintext = Encoding.UTF8.GetBytes(apiKey);
        try
        {
            var nonce = RandomNumberGenerator.GetBytes(12);
            var ciphertext = new byte[plaintext.Length];
            var tag = new byte[16];
            using var aes = new AesGcm(key, 16);
            aes.Encrypt(nonce, plaintext, ciphertext, tag, AssociatedData(userId, connectionId));
            return string.Join('.', "v1", settings.ActiveKeyId, Convert.ToBase64String(nonce),
                Convert.ToBase64String(tag), Convert.ToBase64String(ciphertext));
        }
        finally { CryptographicOperations.ZeroMemory(key); CryptographicOperations.ZeroMemory(plaintext); }
    }

    public string Unprotect(string ciphertext, long userId, long connectionId)
    {
        try
        {
            var parts = ciphertext.Split('.');
            if (parts.Length != 5 || parts[0] != "v1" || !options.Value.Keys.TryGetValue(parts[1], out var encodedKey))
                throw new CryptographicException();
            var key = Convert.FromBase64String(encodedKey);
            var data = Convert.FromBase64String(parts[4]);
            var plaintext = new byte[data.Length];
            try
            {
                using var aes = new AesGcm(key, 16);
                aes.Decrypt(Convert.FromBase64String(parts[2]), data, Convert.FromBase64String(parts[3]), plaintext,
                    AssociatedData(userId, connectionId));
                return Encoding.UTF8.GetString(plaintext);
            }
            finally { CryptographicOperations.ZeroMemory(key); CryptographicOperations.ZeroMemory(plaintext); }
        }
        catch (Exception ex) when (ex is CryptographicException or FormatException or ArgumentException)
        {
            // Do not include the ciphertext/key or cryptographic exception payload in application logs.
            throw new InvalidOperationException("The stored AI credential cannot be decrypted. Check the configured key ring.");
        }
    }

    public string Fingerprint(string canonicalRequest)
    {
        var key = Convert.FromBase64String(options.Value.FingerprintKey);
        var input = Encoding.UTF8.GetBytes(canonicalRequest);
        try { return Convert.ToHexStringLower(HMACSHA256.HashData(key, input)); }
        finally { CryptographicOperations.ZeroMemory(key); CryptographicOperations.ZeroMemory(input); }
    }

    private static byte[] AssociatedData(long userId, long connectionId) =>
        Encoding.UTF8.GetBytes(FormattableString.Invariant($"RealityLoop.AI.v1|{userId}|{connectionId}"));
}
