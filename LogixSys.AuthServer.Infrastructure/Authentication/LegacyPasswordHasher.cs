using LogixSys.AuthServer.Application.Interfaces;
using System.Security.Cryptography;

namespace LogixSys.AuthServer.Infrastructure.Authentication;

public sealed class LegacyPasswordHasher
    : ILegacyPasswordHasher
{
    public bool Verify(
        string hashedPassword,
        string providedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword))
            return false;

        if (string.IsNullOrEmpty(providedPassword))
            return false;


        byte[] decodedHash;

        try
        {
            decodedHash =
                Convert.FromBase64String(hashedPassword);
        }
        catch
        {
            return false;
        }


        // ASP.NET Identity 2.x format:
        //
        // Byte 0-3   : Version
        // Byte 4-19  : Salt (16 bytes)
        // Byte 20-51 : Subkey (32 bytes)


        if (decodedHash.Length != 49)
            return false;


        byte[] salt = new byte[16];

        Buffer.BlockCopy(
            decodedHash,
            1,
            salt,
            0,
            16);


        byte[] expectedSubkey = new byte[32];

        Buffer.BlockCopy(
            decodedHash,
            17,
            expectedSubkey,
            0,
            32);


        using var deriveBytes =
            new Rfc2898DeriveBytes(
                providedPassword,
                salt,
                1000,
                HashAlgorithmName.SHA1);


        byte[] actualSubkey =
            deriveBytes.GetBytes(32);


        return CryptographicOperations
            .FixedTimeEquals(
                expectedSubkey,
                actualSubkey);
    }
}