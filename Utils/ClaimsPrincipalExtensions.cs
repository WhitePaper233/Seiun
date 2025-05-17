using System.Security.Claims;
using Seiun.Utils.Enums;

namespace Seiun.Utils;

public static class ClaimsPrincipalExtensions
{
	public static Guid? GetUserId(this ClaimsPrincipal user)
	{
		var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		return userId == null ? null : new Guid(userId);
	}

	public static string? GetUserName(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.Name)?.Value;

	public static UserRole GetUserRole(this ClaimsPrincipal user)
	{
		var role = user.FindFirst(ClaimTypes.Role)?.Value;
		return role == null ? UserRole.Unknown : Enum.Parse<UserRole>(role);
	}
}
