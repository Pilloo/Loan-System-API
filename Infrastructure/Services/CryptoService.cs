using System.Security.Cryptography;
using Core.Interfaces;

namespace Infrastructure.Services;

public class CryptoService : ICryptoService
{
    public RSA LoadRsaKey(string filePath)
    {
        RSA rsaKey = RSA.Create();

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }
        
        string pemContent = File.ReadAllText(filePath);
        rsaKey.ImportFromPem(pemContent);
        
        return rsaKey;
    }
}