using Seiun.Entities;

namespace Seiun.Services;

public interface IJwtService
{
	string GenerateToken(UserEntity userEntity);
}
