using System.Data;
using MySqlConnector;

namespace Smart_farm_Rwanda.Models;

public class PlotRepository
{
    private readonly string _cs;

    public PlotRepository(string connectionString)
    {
        _cs = connectionString;
    }

    public DataTable GetByFarm(int farmId, int userId)
    {
        const string sql = @"SELECT p.PlotID, p.FarmID, p.PlotName, p.SizeHectares, p.SoilNotes
                             FROM Plots p
                             INNER JOIN Farms f ON p.FarmID = f.FarmID
                             WHERE p.FarmID=@fid AND f.UserID=@uid
                             ORDER BY p.PlotID";
        using var con = new MySqlConnection(_cs);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@fid", MySqlDbType.Int32).Value = farmId;
        cmd.Parameters.Add("@uid", MySqlDbType.Int32).Value = userId;
        con.Open();
        using var reader = cmd.ExecuteReader();
        var table = new DataTable();
        table.Load(reader);
        return table;
    }

    // Returns 0 if the farm does not belong to this user
    public int Insert(Plot plot, int userId)
    {
        const string sql = @"INSERT INTO Plots (FarmID, PlotName, SizeHectares, SoilNotes)
                             SELECT @fid, @name, @size, @soil
                             FROM Farms WHERE FarmID=@fid AND UserID=@uid";
        using var con = new MySqlConnection(_cs);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@fid", MySqlDbType.Int32).Value = plot.FarmID;
        cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = plot.PlotName;
        cmd.Parameters.Add("@size", MySqlDbType.Decimal).Value = plot.SizeHectares;
        cmd.Parameters.Add("@soil", MySqlDbType.VarChar, 200).Value = Database.ToDb(plot.SoilNotes);
        cmd.Parameters.Add("@uid", MySqlDbType.Int32).Value = userId;
        con.Open();
        return cmd.ExecuteNonQuery();
    }

    public int Update(Plot plot, int userId)
    {
        const string sql = @"UPDATE Plots p
                             INNER JOIN Farms f ON p.FarmID = f.FarmID
                             SET p.PlotName=@name, p.SizeHectares=@size, p.SoilNotes=@soil
                             WHERE p.PlotID=@id AND f.UserID=@uid";
        using var con = new MySqlConnection(_cs);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = plot.PlotName;
        cmd.Parameters.Add("@size", MySqlDbType.Decimal).Value = plot.SizeHectares;
        cmd.Parameters.Add("@soil", MySqlDbType.VarChar, 200).Value = Database.ToDb(plot.SoilNotes);
        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = plot.PlotID;
        cmd.Parameters.Add("@uid", MySqlDbType.Int32).Value = userId;
        con.Open();
        return cmd.ExecuteNonQuery();
    }

    public int Delete(int plotId, int userId)
    {
        const string sql = @"DELETE p FROM Plots p
                             INNER JOIN Farms f ON p.FarmID = f.FarmID
                             WHERE p.PlotID=@id AND f.UserID=@uid";
        using var con = new MySqlConnection(_cs);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = plotId;
        cmd.Parameters.Add("@uid", MySqlDbType.Int32).Value = userId;
        con.Open();
        return cmd.ExecuteNonQuery();
    }
}