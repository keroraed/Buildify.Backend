using Buildify.Core.Entities.Identity;

namespace Buildify.Core.Services;

public interface ITokenService
{
    string CreateToken(AppUser user, IList<string> roles);
}
