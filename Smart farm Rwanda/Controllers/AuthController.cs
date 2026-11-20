using Smart_farm_Rwanda.Models;

namespace Smart_farm_Rwanda.Controllers;

public class AuthController
{
    private readonly UserRepository _users;

    public AuthController(UserRepository users)
    {
        _users = users;
    }

    public void Register(string fullName, string username, string password,
                         string confirm, string phone)
    {
        fullName = fullName.Trim();
        username = username.Trim();

        if (fullName == "" || username == "" || password == "")
            throw new ArgumentException("Complete all required fields.");
        if (password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters.");
        if (password != confirm)
            throw new ArgumentException("Passwords do not match.");
        if (_users.UsernameExists(username))
            throw new ArgumentException("That username is already taken.");

        _users.Insert(new User
        {
            FullName = fullName,
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "Farmer",
            Phone = phone.Trim()
        });
    }

    public User Login(string username, string password)
    {
        if (username.Trim() == "" || password == "")
            throw new ArgumentException("Enter your username and password.");

        User? user = _users.GetByUsername(username.Trim());
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new ArgumentException("Wrong username or password.");

        return user;
    }
}