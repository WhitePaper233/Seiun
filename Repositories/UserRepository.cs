using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;
using Seiun.Entities;
using Seiun.Utils;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public class UserRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<UserEntity>(dbContext, minioClient), IUserRepository
{
    public async Task<UserEntity?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await DbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }

    public async Task<UserEntity?> GetByUserNameAsync(string userName)
    {
        return await DbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        return await DbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task UpdateAvatarAsync(UserEntity user, Stream avatarData)
    {
        if (user.AvatarFileName != null)
        {
            // Delete old avatars
            var removeObjectArgs = new RemoveObjectArgs().WithBucket(Constants.BucketNames.Avatar)
                .WithObject(user.AvatarFileName);
            await MinioCl.RemoveObjectAsync(removeObjectArgs).ConfigureAwait(false);
        }

        // Upload new avatar
        var avatarName = $"{Guid.NewGuid()}.webp";
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(Constants.BucketNames.Avatar)
            .WithObject(avatarName)
            .WithContentType(MediaTypeNames.Image.Webp)
            .WithStreamData(avatarData)
            .WithObjectSize(avatarData.Length);
        await MinioCl.PutObjectAsync(putObjectArgs);

        // Update user avatar file name
        user.AvatarFileName = avatarName;
        DbContext.Users.Update(user);
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 获取头像文件流
    /// </summary>
    /// <param name="fileName">头像文件名</param>
    /// <returns>头像文件流</returns>
    public async Task<MemoryStream> GetAvatarAsync(string fileName)
    {
        var avatarStream = new MemoryStream();
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(Constants.BucketNames.Avatar)
            .WithObject(fileName)
            .WithCallbackStream(data => data.CopyTo(avatarStream));
        await MinioCl.GetObjectAsync(getObjectArgs).ConfigureAwait(false);
        avatarStream.Seek(0, SeekOrigin.Begin); // 重置流位置
        return avatarStream;
    }

    public async Task<List<UserEntity>> GetUsersByUserNameAsync(string? keyword)
    {
        return await DbContext.Users
            .Where(u => string.IsNullOrEmpty(keyword) || u.UserName.Contains(keyword))
            .ToListAsync();
    }

    public async Task<List<UserEntity>> GetAllRolesAsync(int index, int size, Guid? keyword)
    {
        var query = DbContext.Users
            .Where(u => u.Role == UserRole.SuperAdmin 
                    || u.Role == UserRole.Admin 
                    || u.Role == UserRole.Creator)
            .AsQueryable();

        if (keyword.HasValue)
        {
            query = query.Where(a => a.Id == keyword.Value);
        }

        return await query
            .Skip((index - 1) * size)
            .Take(size)
            .ToListAsync();
    }


    public async Task<int> GetTotalRolesAsync(Guid? keyword)
    {
        var query = DbContext.Users
            .Where(u => u.Role == UserRole.SuperAdmin 
                    || u.Role == UserRole.Admin 
                    || u.Role == UserRole.Creator)
            .AsQueryable();

        if (keyword.HasValue)
        {
            query = query.Where(a => a.Id == keyword.Value);
        }

        return await query.CountAsync();
    }

    public async Task RemoveRoleAsync(Guid userId)
    {
        var user = await DbContext.Users.FindAsync(userId) ?? throw new Exception($"User with ID {userId} not found");
        user.Role = UserRole.User;  
        DbContext.Users.Update(user);
    }

    public async Task ChangeRoleAsync(Guid userId)
    {
        var user = await DbContext.Users.FindAsync(userId) ?? throw new Exception($"User with ID {userId} not found");
        user.Role = UserRole.SuperAdmin;  
        DbContext.Users.Update(user);
    }
}