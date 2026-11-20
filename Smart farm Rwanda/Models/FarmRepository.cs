using System.Data;
using MySqlConnector;

namespace Smart_farm_Rwanda.Models;

public class FarmRepository
{
    private readonly string _cs;

    public FarmRepository(string connectionString)
    {
        _cs = connectionString;
    }

    public DataTable GetByUser(int userId, string keyword)
    {
        const string sql = @"SELECT FarmID, FarmName, Location, Description
                             FROM Farms
                             WHERE UserID=@uid AND (FarmName LIKE @k OR Location LIKE @k)
                             ORDER BY FarmID";
        using var con = new MySqlConnection(_cs);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@uid", MySqlDbType.Int32).Value = userId;
        cmd.Parameters.Add("@k", MySqlDbType.VarChar, 150).Value = "%" + keyword + "%";
        con.Open();
        using var reader = cmd.ExecuteReader();
        var table = new DataTable();
        table.Load(reader);
        return table;
    }

    public void Insert(Farm farm)
    {
        const string sql = @"INSERT INTO Farms (UserID, FarmName, Location, Description)
                             VALUES (@uid, @name, @loc, @desc)";
        using var con = new MySqlConnection(_cs);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@uid", MySqlDbType.Int32).Value = farm.UserID;
        cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = farm.FarmName;
        cmd.Parameters.Add("@loc", MySqlDbType.VarChar, 150).Value = Database.ToDb(farm.Location);
        cmd.Parameters.Add("@desc", MySqlDbType.Text).Value = Database.ToDb(farm.Description);
        con.Open();
        cmd.ExecuteNonQuery();
    }

    public int Update(Farm farm)
    {
        const string sql = @"UPDATE Farms
                             SET FarmName=@name, Location=@loc, Description=@desc
                             WHERE FarmID=@id AND UserID=@uid";
        using var con = new MySqlConnection(_cs);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = farm.FarmName;
        cmd.Parameters.Add("@loc", MySqlDbType.VarChar, 150).Value = Database.ToDb(farm.Location);
        cmd.Parameters.Add("@desc", MySqlDbType.Text).Value = Database.ToDb(farm.Description);
        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = farm.FarmID;
        cmd.Parameters.Add("@uid", MySqlDbType.Int32).Value = farm.UserID;
        con.Open();
        return cmd.ExecuteNonQuery();
    }

    public int Delete(int farmId, int userId)
    {
        const string sql = "DELETE FROM Farms WHERE FarmID=@id AND UserID=@uid";
        using var con = new MySqlConnection(_cs);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = farmId;
        cmd.Parameters.Add("@uid", MySqlDbType.Int32).Value = userId;
        con.Open();
        return cmd.ExecuteNonQuery();
    }
}