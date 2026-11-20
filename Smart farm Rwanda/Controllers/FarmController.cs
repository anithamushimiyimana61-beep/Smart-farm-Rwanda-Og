using System.Data;
using Smart_farm_Rwanda.Models;

namespace Smart_farm_Rwanda.Controllers;

public class FarmController
{
    private readonly FarmRepository _repo;

    public FarmController(FarmRepository repo)
    {
        _repo = repo;
    }

    private static int CurrentUserId =>
        Session.CurrentUser?.UserID ?? throw new InvalidOperationException("Please log in first.");

    public DataTable GetFarms(string keyword = "") => _repo.GetByUser(CurrentUserId, keyword.Trim());

    public void AddFarm(Farm farm)
    {
        Validate(farm);
        farm.UserID = CurrentUserId;
        _repo.Insert(farm);
    }

    public void EditFarm(Farm farm)
    {
        if (farm.FarmID <= 0) throw new ArgumentException("Select a farm before updating.");
        Validate(farm);
        farm.UserID = CurrentUserId;
        if (_repo.Update(farm) == 0) throw new ArgumentException("Farm not found.");
    }

    public void RemoveFarm(int farmId)
    {
        if (farmId <= 0) throw new ArgumentException("Select a farm before deleting.");
        if (_repo.Delete(farmId, CurrentUserId) == 0) throw new ArgumentException("Farm not found.");
    }

    private static void Validate(Farm farm)
    {
        farm.FarmName = farm.FarmName.Trim();
        if (farm.FarmName == "") throw new ArgumentException("Farm name is required.");
        if (farm.FarmName.Length > 100) throw new ArgumentException("Farm name is too long (max 100).");
        if (farm.Location.Trim().Length > 150) throw new ArgumentException("Location is too long (max 150).");
    }
}