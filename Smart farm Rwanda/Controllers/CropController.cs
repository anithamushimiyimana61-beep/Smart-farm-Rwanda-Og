using System.Data;
using Smart_farm_Rwanda.Models;

namespace Smart_farm_Rwanda.Controllers;

public class CropController
{
    public static readonly string[] Statuses = { "Planned", "Growing", "Harvested", "Failed" };

    private readonly CropRepository _repo;

    public CropController(CropRepository repo)
    {
        _repo = repo;
    }

    private static int CurrentUserId =>
        Session.CurrentUser?.UserID ?? throw new InvalidOperationException("Please log in first.");

    public DataTable GetPlotChoices() => _repo.GetPlotChoices(CurrentUserId);

    public DataTable GetCrops(string keyword = "") => _repo.GetByUser(CurrentUserId, keyword.Trim());

    public void AddCrop(Crop crop)
    {
        Validate(crop);
        if (_repo.Insert(crop, CurrentUserId) == 0) throw new ArgumentException("Plot not found.");
    }

    public void EditCrop(Crop crop)
    {
        if (crop.CropID <= 0) throw new ArgumentException("Select a crop before updating.");
        Validate(crop);
        if (_repo.Update(crop, CurrentUserId) == 0) throw new ArgumentException("Crop not found.");
    }

    public void RemoveCrop(int cropId)
    {
        if (cropId <= 0) throw new ArgumentException("Select a crop before deleting.");
        if (_repo.Delete(cropId, CurrentUserId) == 0) throw new ArgumentException("Crop not found.");
    }

    private static void Validate(Crop crop)
    {
        if (crop.PlotID <= 0) throw new ArgumentException("Select a plot.");

        crop.CropName = crop.CropName.Trim();
        crop.Variety = crop.Variety.Trim();

        if (crop.CropName == "") throw new ArgumentException("Crop name is required.");
        if (crop.CropName.Length > 100) throw new ArgumentException("Crop name is too long (max 100).");
        if (crop.Variety.Length > 100) throw new ArgumentException("Variety is too long (max 100).");
        if (crop.ExpectedHarvestDate.Date < crop.PlantingDate.Date)
            throw new ArgumentException("Expected harvest date cannot be before the planting date.");
        if (Array.IndexOf(Statuses, crop.Status) < 0)
            throw new ArgumentException("Choose a valid status.");
    }
}
