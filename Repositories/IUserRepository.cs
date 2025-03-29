using Seiun.Entities;

namespace Seiun.Repositories;

public interface IUserRepository : IBaseRepository<UserEntity>
{
    Task<UserEntity?> GetByPhoneNumberAsync(string phoneNumber);
    Task<UserEntity?> GetByUserNameAsync(string userName);
    Task<UserEntity?> GetByEmailAsync(string email);
    Task UpdateAvatarAsync(UserEntity user, Stream avatarData);
    Task<MemoryStream> GetAvatarAsync(string fileName);
    Task<List<UserEntity>> GetUsersByUserNameAsync(string? keyword);
    Task<List<UserEntity>> GetAllRolesAsync(int index, int size, Guid? keyword);
    Task<int> GetTotalRolesAsync(Guid? keyword);
    Task RemoveRoleAsync(Guid userId);
    Task ChangeRoleAsync(Guid userId);
}