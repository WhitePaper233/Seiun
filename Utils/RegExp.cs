namespace Seiun.Utils;

public static class RegExp
{
	public static class User
	{
		public const string UserNamePattern = @"^[a-zA-Z0-9_]{3,16}$";
		public const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$";
	}
}
