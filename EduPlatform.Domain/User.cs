using CSharpFunctionalExtensions;

namespace EduPlatform.Domain;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string HashedPassword { get; private set; }
    public IReadOnlyList<UserRole> Roles { get; private set; }

    private User(Guid id, string name, string email, string hashedPassword)
    {
        Id = id;
        Name = name;
        Email = email;
        HashedPassword = hashedPassword;
        Roles = new List<UserRole>() { };
    }

    private User(Guid id, string name, string email, string hashedPassword, IReadOnlyList<UserRole> roles)
    {
        Id = id;
        Name = name;
        Email = email;
        HashedPassword = hashedPassword;
        Roles = roles;
    }

    public static Result<User> Create(string name, string email, string password)
    {
        var validate = Validate(name, email, password);

        if (validate.IsFailure)
            return Result.Failure<User>(validate.Error);

        var user = new User(Guid.NewGuid(), name, email, password);

        return Result.Success(user);
    }

    public static Result<User> Create(string name, string email, string password, IReadOnlyList<UserRole> roles)
    {
        var validate = Validate(name, email, password);

        if (validate.IsFailure)
            return Result.Failure<User>(validate.Error);

        var user = new User(Guid.NewGuid(), name, email, password, roles);

        return Result.Success(user);
    }

    private static Result Validate(string name, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Name is invalid");

        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure("Email is invalid");

        if (string.IsNullOrWhiteSpace(password))
            return Result.Failure("Password is invalid");

        return Result.Success();
    }
    
    public static User Load(Guid id, string name, string email, string hashedPassword)
    {
        return new User(id, name, email, hashedPassword);
    }

    public static User Load(Guid id, string name, string email, string hashedPassword, IReadOnlyList<UserRole> roles)
    {
        return new User(id, name, email, hashedPassword, roles);
    }

    public bool CanBeManagedByAdmin(Guid adminId, IReadOnlyList<UserRole> roles)
        => Id == adminId || roles.Contains(UserRole.Admin);

    public Result UpdateInfo(string name, string email, string password, IReadOnlyList<UserRole>? roles = null)
    {
        Name = name;
        Email = email;
        HashedPassword = password;

        if (roles is not null)
            Roles = roles;
        
        return Result.Success();
    }
}
