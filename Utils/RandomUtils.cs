namespace Seiun.Utils;

public static class RandomUtils
{
	private static readonly Random Random = new();

	// ReSharper disable once StringLiteralTypo
	private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

	public static string GenerateRandomString(uint length)
	{
		var results = new char[length];
		for (uint i = 0; i < length; ++i) results[i] = Characters[Random.Next(Characters.Length)];

		return new string(results);
	}
}
