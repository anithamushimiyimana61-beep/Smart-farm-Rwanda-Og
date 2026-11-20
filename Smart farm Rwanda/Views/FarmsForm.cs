using System.Data;
using Smart_farm_Rwanda.Controllers;
using Smart_farm_Rwanda.Models;

namespace Smart_farm_Rwanda.Views;

public class FarmsForm : Form
{
    private readonly FarmController _farmController = new(new FarmRepository(Database.ConnectionString));
    private readonly PlotController _plotController = new(new PlotRepository(Database.ConnectionString));
    private int _selectedFarmId = 0;
    private int _selectedPlotId = 0;

    // Farm controls
    private readonly TextBox txtSearch = new() { Left = 15, Top = 15, Width = 200 };
    private readonly Button btnSearch = new() { Text = "Search", Left = 225, Top = 13, Width = 80 };
    private readonly Button btnRefresh = new() { Text = "Refresh", Left = 315, Top = 13, Width = 80 };
    private readonly DataGridView dgvFarms = new() { Left = 15, Top = 50, Width = 520, Height = 230 };
    private readonly TextBox txtFarmName = new() { Left = 120, Top = 302, Width = 290 };
    private readonly TextBox txtLocation = new() { Left = 120, Top = 337, Width = 290 };
    private readonly TextBox txtDescription = new() { Left = 120, Top = 372, Width = 290 };
    private readonly Button btnSaveFarm = new() { Text = "Save", Left = 15, Top = 420, Width = 90 };
    private readonly Button btnUpdateFarm = new() { Text = "Update", Left = 115, Top = 420, Width = 90 };
    private readonly Button btnDeleteFarm = new() { Text = "Delete", Left = 215, Top = 420, Width = 90 };
    private readonly Button btnClearFarm = new() { Text = "Clear", Left = 315, Top = 420, Width = 90 };

    // Plot controls
    private readonly Label lblPlots = new() { Text = "Plots: select a farm first", Left = 560, Top = 18, Width = 520, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
    private readonly DataGridView dgvPlots = new() { Left = 560, Top = 50, Width = 525, Height = 230 };
    private readonly TextBox txtPlotName = new() { Left = 670, Top = 302, Width = 290 };
    private readonly TextBox txtSize = new() { Left = 670, Top = 337, Width = 120 };
    private readonly TextBox txtSoil = new() { Left = 670, Top = 372, Width = 290 };
    private readonly Button btnAddPlot = new() { Text = "Add plot", Left = 560, Top = 420, Width = 90 };
    private readonly Button btnUpdatePlot = new() { Text = "Update", Left = 660, Top = 420, Width = 90 };
    private readonly Button btnDeletePlot = new() { Text = "Delete", Left = 760, Top = 420, Width = 90 };
    private readonly Button btnClearPlot = new() { Text = "Clear", Left = 860, Top = 420, Width = 90 };

    public FarmsForm()
    {
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        Text = "My Farms and Plots";
        ClientSize = new Size(1100, 500);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        ConfigureGrid(dgvFarms);
        ConfigureGrid(dgvPlots);

        AddLabel("Farm name *", 15, 305);
        AddLabel("Location", 15, 340);
        AddLabel("Description", 15, 375);
        AddLabel("Plot name *", 560, 305);
        AddLabel("Size (ha) *", 560, 340);
        AddLabel("Soil notes", 560, 375);

        Controls.AddRange(new Control[]
        {
            txtSearch, btnSearch, btnRefresh, dgvFarms,
            txtFarmName, txtLocation, txtDescription,
            btnSaveFarm, btnUpdateFarm, btnDeleteFarm, btnClearFarm,
            lblPlots, dgvPlots, txtPlotName, txtSize, txtSoil,
            btnAddPlot, btnUpdatePlot, btnDeletePlot, btnClearPlot
        });

        Load += (s, e) => LoadFarms();
        btnSearch.Click += (s, e) => LoadFarms(txtSearch.Text);
        btnRefresh.Click += (s, e) => { txtSearch.Clear(); LoadFarms(); };
        dgvFarms.CellClick += dgvFarms_CellClick;
        dgvPlots.CellClick += dgvPlots_CellClick;

        btnSaveFarm.Click += btnSaveFarm_Click;
        btnUpdateFarm.Click += btnUpdateFarm_Click;
        btnDeleteFarm.Click += btnDeleteFarm_Click;
        btnClearFarm.Click += (s, e) => ClearFarmForm();

        btnAddPlot.Click += btnAddPlot_Click;
        btnUpdatePlot.Click += btnUpdatePlot_Click;
        btnDeletePlot.Click += btnDeletePlot_Click;
        btnClearPlot.Click += (s, e) => ClearPlotForm();
    }

    // ---------- helpers ----------
    private void AddLabel(string text, int x, int y) =>
        Controls.Add(new Label { Text = text, Left = x, Top = y, Width = 105 });

    private static void ConfigureGrid(DataGridView grid)
    {
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.RowHeadersVisible = false;
    }

    private static void HideColumn(DataGridView grid, string name)
    {
        var column = grid.Columns[name];
        if (column != null) column.Visible = false;
    }

    private void ShowError(string title, Exception ex) =>
        MessageBox.Show(ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

    // ---------- farms ----------
    private void LoadFarms(string keyword = "")
    {
        try
        {
            dgvFarms.DataSource = _farmController.GetFarms(keyword);
            HideColumn(dgvFarms, "FarmID");
            dgvFarms.ClearSelection();
        }
        catch (Exception ex) { ShowError("Load farms", ex); }
    }

    private Farm ReadFarm() => new Farm
    {
        FarmID = _selectedFarmId,
        FarmName = txtFarmName.Text,
        Location = txtLocation.Text,
        Description = txtDescription.Text
    };

    private void dgvFarms_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        DataGridViewRow row = dgvFarms.Rows[e.RowIndex];
        _selectedFarmId = Convert.ToInt32(row.Cells["FarmID"].Value);
        txtFarmName.Text = Convert.ToString(row.Cells["FarmName"].Value);
        txtLocation.Text = Convert.ToString(row.Cells["Location"].Value);
        txtDescription.Text = Convert.ToString(row.Cells["Description"].Value);
        lblPlots.Text = "Plots of: " + txtFarmName.Text;
        ClearPlotForm();
        LoadPlots();
    }

    private void btnSaveFarm_Click(object? sender, EventArgs e)
    {
        try
        {
            _selectedFarmId = 0;
            _farmController.AddFarm(ReadFarm());
            MessageBox.Show("Farm saved.");
            LoadFarms();
            ClearFarmForm();
        }
        catch (Exception ex) { ShowError("Save farm", ex); }
    }

    private void btnUpdateFarm_Click(object? sender, EventArgs e)
    {
        try
        {
            _farmController.EditFarm(ReadFarm());
            MessageBox.Show("Farm updated.");
            LoadFarms();
            ClearFarmForm();
        }
        catch (Exception ex) { ShowError("Update farm", ex); }
    }

    private void btnDeleteFarm_Click(object? sender, EventArgs e)
    {
        if (_selectedFarmId <= 0)
        {
            MessageBox.Show("Select a farm first.");
            return;
        }
        if (MessageBox.Show("Delete this farm? Its plots, crops, tasks, expenses and harvests will also be deleted.",
                "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;
        try
        {
            _farmController.RemoveFarm(_selectedFarmId);
            MessageBox.Show("Farm deleted.");
            LoadFarms();
            ClearFarmForm();
        }
        catch (Exception ex) { ShowError("Delete farm", ex); }
    }

    private void ClearFarmForm()
    {
        _selectedFarmId = 0;
        txtFarmName.Clear();
        txtLocation.Clear();
        txtDescription.Clear();
        lblPlots.Text = "Plots: select a farm first";
        ClearPlotForm();
        dgvPlots.DataSource = null;
        dgvFarms.ClearSelection();
        txtFarmName.Focus();
    }

    // ---------- plots ----------
    private void LoadPlots()
    {
        try
        {
            if (_selectedFarmId <= 0)
            {
                dgvPlots.DataSource = null;
                return;
            }
            dgvPlots.DataSource = _plotController.GetPlots(_selectedFarmId);
            HideColumn(dgvPlots, "PlotID");
            HideColumn(dgvPlots, "FarmID");
            dgvPlots.ClearSelection();
        }
        catch (Exception ex) { ShowError("Load plots", ex); }
    }

    private Plot ReadPlot()
    {
        if (!decimal.TryParse(txtSize.Text.Trim(), out decimal size))
            throw new ArgumentException("Enter a valid size in hectares.");
        return new Plot
        {
            PlotID = _selectedPlotId,
            FarmID = _selectedFarmId,
            PlotName = txtPlotName.Text,
            SizeHectares = size,
            SoilNotes = txtSoil.Text
        };
    }

    private void dgvPlots_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        DataGridViewRow row = dgvPlots.Rows[e.RowIndex];
        _selectedPlotId = Convert.ToInt32(row.Cells["PlotID"].Value);
        txtPlotName.Text = Convert.ToString(row.Cells["PlotName"].Value);
        txtSize.Text = Convert.ToString(row.Cells["SizeHectares"].Value);
        txtSoil.Text = Convert.ToString(row.Cells["SoilNotes"].Value);
    }

    private void btnAddPlot_Click(object? sender, EventArgs e)
    {
        try
        {
            _selectedPlotId = 0;
            _plotController.AddPlot(ReadPlot());
            MessageBox.Show("Plot saved.");
            LoadPlots();
            ClearPlotForm();
        }
        catch (Exception ex) { ShowError("Add plot", ex); }
    }

    private void btnUpdatePlot_Click(object? sender, EventArgs e)
    {
        try
        {
            _plotController.EditPlot(ReadPlot());
            MessageBox.Show("Plot updated.");
            LoadPlots();
            ClearPlotForm();
        }
        catch (Exception ex) { ShowError("Update plot", ex); }
    }

    private void btnDeletePlot_Click(object? sender, EventArgs e)
    {
        if (_selectedPlotId <= 0)
        {
            MessageBox.Show("Select a plot first.");
            return;
        }
        if (MessageBox.Show("Delete this plot? Its crops will also be deleted.",
                "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;
        try
        {
            _plotController.RemovePlot(_selectedPlotId);
            MessageBox.Show("Plot deleted.");
            LoadPlots();
            ClearPlotForm();
        }
        catch (Exception ex) { ShowError("Delete plot", ex); }
    }

    private void ClearPlotForm()
    {
        _selectedPlotId = 0;
        txtPlotName.Clear();
        txtSize.Clear();
        txtSoil.Clear();
        dgvPlots.ClearSelection();
    }
}
