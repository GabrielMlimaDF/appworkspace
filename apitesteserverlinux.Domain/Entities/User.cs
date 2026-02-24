namespace apitesteserverlinux.Domain.Entities;
public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    // Telefone no padrão E.164 (ex: +5511999999999)
    public string? PhoneNumber { get; private set; }

    // Hash da senha (nunca senha em texto puro)
    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    // 🔹 Role do usuário no SaaS
    public UserRole Role { get; private set; } = UserRole.User;

    private User() { } // EF Core

    public User(
        string name,
        string email,
        string passwordHash,
        string? phoneNumber = null,
        UserRole role = UserRole.User // 🔹 default
    )
    {
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role;

        if (phoneNumber is not null)
            SetPhoneNumber(phoneNumber);
    }

    public void SetPhoneNumber(string phoneNumber)
    {
        PhoneNumber = NormalizePhone(phoneNumber);
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

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
