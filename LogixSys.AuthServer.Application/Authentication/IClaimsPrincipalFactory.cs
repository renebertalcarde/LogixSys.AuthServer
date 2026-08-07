using System.Security.Claims;

namespace LogixSys.AuthServer.Application.Authentication;

public interface IClaimsPrincipalFactory
{
    ClaimsPrincipal Create(UserProfile profile);
}