using System.Security.Cryptography;

namespace Core.Interfaces;

public interface ICryptoService
{
    RSA LoadRsaKey(string filePath);
}