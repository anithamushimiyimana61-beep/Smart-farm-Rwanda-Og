namespace Smart_farm_Rwanda.Models;

public class Crop
{
    public int CropID { get; set; }
    public int PlotID { get; set; }
    public string CropName { get; set; } = "";
    public string Variety { get; set; } = "";
    public DateTime PlantingDate { get; set; } = DateTime.Today;
    public DateTime ExpectedHarvestDate { get; set; } = DateTime.Today;
    public string Status { get; set; } = "Planned";
}
