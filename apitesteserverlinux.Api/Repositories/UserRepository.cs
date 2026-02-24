using apitesteserverlinux.Api.Data;
using apitesteserverlinux.Domain.Entities;
using apitesteserverlinux.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace apitesteserverlinux.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
        => await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(x => x.Email == email.ToLowerInvariant());

    public async Task<IEnumerable<User>> GetAllAsync()
        => await _context.Users.AsNoTracking().ToListAsync();

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user is null) return;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
    public Task<bool> AnyAsync()
    => _context.Users.AnyAsync();

    public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var normalized = NormalizePhone(phoneNumber);

        return await _context.Users
            .AnyAsync(u => u.PhoneNumber == normalized);
    }

    private static string NormalizePhone(string phone)
    {
        // remove espaços, traços, parênteses
        var digits = new string(phone.Where(char.IsDigit).ToArray());

        // padrão Brasil (DDD + número)
        if (digits.Length == 11) // ex: 11999999999
            return $"+55{digits}";

        if (digits.Length == 13 && digits.StartsWith("55"))
            return $"+{digits}";

        throw new ArgumentException("Número de celular inválido. Use DDD + número.");
    }


}
