using System.Data;
using MySqlConnector;

namespace Smart_farm_Rwanda.Models;

public class CropRepository
{
    private readonly string _cs;

    public CropRepository(string connectionString)
    {
        _cs = connectionString;
    }

    private static DataTable ReadTable(MySqlConnection con, MySqlCommand cmd)
    {
        con.Open();
        using var reader = cmd.ExecuteReader();
        var table = new DataTable();
        table.Load(reader);
        return table;
    }

    // Plots of this user, shown as "Farm - Plot" in the drop-down list
    public DataTable GetPlotChoices(int userId)
    {
        const string sql = @"SELECT p.PlotID, CONCAT(f.FarmName, ' - ', p.PlotName) AS PlotLabel
                             FROM Plots p
                             INNER JOIN Farms f ON p.FarmID = f.FarmID
                             WHERE f.UserID=@uid
                             ORDER BY f.FarmName, p.PlotName";
        using var con = new MySqlConnection(_cs);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@uid", MySqlDbType.Int32).Value = userId;
        return ReadTable(con, cmd);
    }

    public DataTable GetByUser(int userId, string keyword)
    {
        const string sql = @"SELECT c.CropID, c.PlotID,
                                    CONCAT(f.FarmName, ' - ', p.PlotName) AS Plot,
                                    c.CropName, c.Variety, c.PlantingDate,
                                    c.ExpectedHarvestDate, c.Status
                             FROM Crops c
                             INNER JOIN Plots p ON c.PlotID = p.PlotID
                             INNER JOIN Farms f ON p.FarmID = f.FarmID
                             WHERE f.UserID=@uid
                               AND (c.CropName LIKE @k OR c.Variety LIKE @k OR c.Status LIKE @k)
                             ORDER BY c.CropID";
        using var con = new MySqlConnection(_cs);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@uid", MySqlDbType.Int32).Value = userId;
        cmd.Parameters.Add("@k", MySqlDbType.VarChar, 150).Value = "%" + keyword + "%";
        return ReadTable(con, cmd);
    }

    private static void AddCropParameters(MySqlCommand cmd, Crop crop)
    {
        cmd.Parameters.Add("@pid", MySqlDbType.Int32).Value = crop.PlotID;
        cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = crop.CropName;
        cmd.Parameters.Add("@variety", MySqlDbType.VarChar, 100).Value = Database.ToDb(crop.Variety);
        cmd.Parameters.Add("@plant", MySqlDbType.Date).Value = crop.PlantingDate;
        cmd.Parameters.Add("@harvest", MySqlDbType.Date).Value = crop.ExpectedHarvestDate;
        cmd.Parameters.Add("@status", MySqlDbType.VarChar, 20).Value = crop.Status;
    }

    // Returns 0 if the plot does not belong to this user
    public int Insert(Crop crop, int userId)
    {
        const string sql = @"INSERT INTO Crops (PlotID, CropName, Variety, PlantingDate, ExpectedHarvestDate, Status)
                             SELECT p.PlotID, @name, @variety, @plant, @harvest, @status
                             FROM Plots p
                             INNER JOIN Farms f ON p.FarmID = f.FarmID
                             WHERE p.PlotID=@pid AND f.UserID=@uid";
        using var con = new MySqlConnection(_cs);
        using var cmd = new MySqlCommand(sql, con);
        AddCropParameters(cmd, crop);
        cmd.Parameters.Add("@uid", MySqlDbType.Int32).Value = userId;
        con.Open();
        return cmd.ExecuteNonQuery();
    }

    public int Update(Crop crop, int userId)
    {
        const string sql = @"UPDATE Crops c
                             INNER JOIN Plots p ON c.PlotID = p.PlotID
                             INNER JOIN Farms f ON p.FarmID = f.FarmID
                             SET c.PlotID=@pid, c.CropName=@name, c.Variety=@variety,
                                 c.PlantingDate=@plant, c.ExpectedHarvestDate=@harvest, c.Status=@status
                             WHERE c.CropID=@id AND f.UserID=@uid
                               AND EXISTS (SELECT 1 FROM Plots p2
                                           INNER JOIN Farms f2 ON p2.FarmID = f2.FarmID
                                           WHERE p2.PlotID=@pid AND f2.UserID=@uid)";
        using var con = new MySqlConnection(_cs);
        using var cmd = new MySqlCommand(sql, con);
        AddCropParameters(cmd, crop);
        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = crop.CropID;
        cmd.Parameters.Add("@uid", MySqlDbType.Int32).Value = userId;
        con.Open();
        return cmd.ExecuteNonQuery();
    }

    public int Delete(int cropId, int userId)
    {
        const string sql = @"DELETE c FROM Crops c
                             INNER JOIN Plots p ON c.PlotID = p.PlotID
                             INNER JOIN Farms f ON p.FarmID = f.FarmID
                             WHERE c.CropID=@id AND f.UserID=@uid";
        using var con = new MySqlConnection(_cs);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = cropId;
        cmd.Parameters.Add("@uid", MySqlDbType.Int32).Value = userId;
        con.Open();
        return cmd.ExecuteNonQuery();
    }
}
