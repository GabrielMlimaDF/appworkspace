using apitesteserverlinux.Domain.Entities;

namespace apitesteserverlinux.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<bool> AnyAsync();
    Task<bool> PhoneNumberExistsAsync(string phoneNumber);


}
