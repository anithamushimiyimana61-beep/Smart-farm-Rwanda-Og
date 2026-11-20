namespace Smart_farm_Rwanda.Models;

public class Farm
{
    public int FarmID { get; set; }
    public int UserID { get; set; }
    public string FarmName { get; set; } = "";
    public string Location { get; set; } = "";
    public string Description { get; set; } = "";
}