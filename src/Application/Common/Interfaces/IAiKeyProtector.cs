namespace Application.Common.Interfaces;

public interface IAiKeyProtector
{
    string Protect(string apiKey, long userId, long connectionId);
    string Unprotect(string ciphertext, long userId, long connectionId);
    string Fingerprint(string canonicalRequest);
}
