namespace Smart_farm_Rwanda.Models;

public static class Database
{
    public const string ConnectionString =
        "Server=127.0.0.1;Port=3306;Database=SmartFarmDB;User ID=root;Password=;";

    // Blank text is saved as NULL
    public static object ToDb(string? text) =>
        string.IsNullOrWhiteSpace(text) ? (object)DBNull.Value : text.Trim();
}