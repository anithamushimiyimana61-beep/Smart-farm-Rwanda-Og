namespace Smart_farm_Rwanda.Models;

public class Plot
{
    public int PlotID { get; set; }
    public int FarmID { get; set; }
    public string PlotName { get; set; } = "";
    public decimal SizeHectares { get; set; }
    public string SoilNotes { get; set; } = "";
}