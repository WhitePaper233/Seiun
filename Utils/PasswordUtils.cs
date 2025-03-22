using System.Security.Cryptography;
using System.Text;

namespace Seiun.Utils;

public static class PasswordUtils
{
    /// <summary>
    /// 创建密码哈希
    /// </summary>
    /// <param name="password">密码</param>
    /// <returns>哈希值 盐值</returns>
    public static (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password)
    {
        using var hmac = new HMACSHA512();
        var passwordSalt = hmac.Key;
        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return (passwordHash, passwordSalt);
    }

    /// <summary>
    /// 验证密码哈希
    /// </summary>
    /// <param name="password">密码</param>
    /// <param name="passwordHash">哈希值</param>
    /// <param name="passwordSalt">盐值</param>
    /// <returns>密码是否正确</returns>
    public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return !computedHash.Where((t, i) => t != passwordHash[i]).Any();
    }
}