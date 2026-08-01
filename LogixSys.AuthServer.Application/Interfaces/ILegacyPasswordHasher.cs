namespace LogixSys.AuthServer.Application.Interfaces;

public interface ILegacyPasswordHasher
{
    bool Verify(
        string hashedPassword,
        string providedPassword);
}