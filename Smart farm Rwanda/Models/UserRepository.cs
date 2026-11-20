using MySqlConnector;
using System.Data;

namespace Smart_farm_Rwanda.Models;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool UsernameExists(string username)
    {
        const string sql = "SELECT COUNT(*) FROM Users WHERE Username=@u";
        using var con = new MySqlConnection(_connectionString);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@u", MySqlDbType.VarChar, 50).Value = username;
        con.Open();
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public void Insert(User user)
    {
        const string sql = @"INSERT INTO Users (FullName, Username, PasswordHash, Role, Phone)
                             VALUES (@name, @user, @hash, @role, @phone)";
        using var con = new MySqlConnection(_connectionString);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = user.FullName;
        cmd.Parameters.Add("@user", MySqlDbType.VarChar, 50).Value = user.Username;
        cmd.Parameters.Add("@hash", MySqlDbType.VarChar, 100).Value = user.PasswordHash;
        cmd.Parameters.Add("@role", MySqlDbType.VarChar, 20).Value = user.Role;
        cmd.Parameters.Add("@phone", MySqlDbType.VarChar, 20).Value =
            string.IsNullOrWhiteSpace(user.Phone) ? (object)DBNull.Value : user.Phone;
        con.Open();
        cmd.ExecuteNonQuery();
    }

    public User? GetByUsername(string username)
    {
        const string sql = @"SELECT UserID, FullName, Username, PasswordHash, Role, Phone
                             FROM Users WHERE Username=@u";
        using var con = new MySqlConnection(_connectionString);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@u", MySqlDbType.VarChar, 50).Value = username;
        con.Open();
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return new User
        {
            UserID = Convert.ToInt32(reader["UserID"]),
            FullName = Convert.ToString(reader["FullName"]) ?? "",
            Username = Convert.ToString(reader["Username"]) ?? "",
            PasswordHash = Convert.ToString(reader["PasswordHash"]) ?? "",
            Role = Convert.ToString(reader["Role"]) ?? "Farmer",
            Phone = Convert.ToString(reader["Phone"]) ?? ""
        };
    }
}