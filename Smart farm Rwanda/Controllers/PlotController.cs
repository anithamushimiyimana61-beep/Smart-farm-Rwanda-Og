using System.Data;
using Smart_farm_Rwanda.Models;

namespace Smart_farm_Rwanda.Controllers;

public class PlotController
{
    private readonly PlotRepository _repo;

    public PlotController(PlotRepository repo)
    {
        _repo = repo;
    }

    private static int CurrentUserId =>
        Session.CurrentUser?.UserID ?? throw new InvalidOperationException("Please log in first.");

    public DataTable GetPlots(int farmId) => _repo.GetByFarm(farmId, CurrentUserId);

    public void AddPlot(Plot plot)
    {
        Validate(plot);
        if (_repo.Insert(plot, CurrentUserId) == 0) throw new ArgumentException("Farm not found.");
    }

    public void EditPlot(Plot plot)
    {
        if (plot.PlotID <= 0) throw new ArgumentException("Select a plot before updating.");
        Validate(plot);
        if (_repo.Update(plot, CurrentUserId) == 0) throw new ArgumentException("Plot not found.");
    }

    public void RemovePlot(int plotId)
    {
        if (plotId <= 0) throw new ArgumentException("Select a plot before deleting.");
        if (_repo.Delete(plotId, CurrentUserId) == 0) throw new ArgumentException("Plot not found.");
    }

    private static void Validate(Plot plot)
    {
        if (plot.FarmID <= 0) throw new ArgumentException("Select a farm first.");
        plot.PlotName = plot.PlotName.Trim();
        if (plot.PlotName == "") throw new ArgumentException("Plot name is required.");
        if (plot.SizeHectares <= 0) throw new ArgumentException("Size must be greater than 0.");
        if (plot.SoilNotes.Trim().Length > 200) throw new ArgumentException("Soil notes are too long (max 200).");
    }
}