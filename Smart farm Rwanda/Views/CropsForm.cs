using System.Data;
using Smart_farm_Rwanda.Controllers;
using Smart_farm_Rwanda.Models;

namespace Smart_farm_Rwanda.Views;

public class CropsForm : Form
{
    private readonly CropController _controller = new(new CropRepository(Database.ConnectionString));
    private int _selectedCropId = 0;

    private readonly TextBox txtSearch = new() { Left = 15, Top = 15, Width = 200 };
    private readonly Button btnSearch = new() { Text = "Search", Left = 225, Top = 13, Width = 80 };
    private readonly Button btnRefresh = new() { Text = "Refresh", Left = 315, Top = 13, Width = 80 };
    private readonly DataGridView dgvCrops = new() { Left = 15, Top = 50, Width = 970, Height = 230 };

    private readonly ComboBox cmbPlot = new() { Left = 150, Top = 302, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox txtCropName = new() { Left = 150, Top = 337, Width = 250 };
    private readonly TextBox txtVariety = new() { Left = 150, Top = 372, Width = 250 };
    private readonly DateTimePicker dtpPlanting = new() { Left = 690, Top = 302, Width = 200, Format = DateTimePickerFormat.Short };
    private readonly DateTimePicker dtpHarvest = new() { Left = 690, Top = 337, Width = 200, Format = DateTimePickerFormat.Short };
    private readonly ComboBox cmbStatus = new() { Left = 690, Top = 372, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

    private readonly Button btnSave = new() { Text = "Save", Left = 15, Top = 420, Width = 90 };
    private readonly Button btnUpdate = new() { Text = "Update", Left = 115, Top = 420, Width = 90 };
    private readonly Button btnDelete = new() { Text = "Delete", Left = 215, Top = 420, Width = 90 };
    private readonly Button btnClear = new() { Text = "Clear", Left = 315, Top = 420, Width = 90 };

    public CropsForm()
    {
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        Text = "My Crops";
        ClientSize = new Size(1000, 480);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        dgvCrops.ReadOnly = true;
        dgvCrops.AllowUserToAddRows = false;
        dgvCrops.AllowUserToDeleteRows = false;
        dgvCrops.MultiSelect = false;
        dgvCrops.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCrops.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvCrops.RowHeadersVisible = false;

        cmbStatus.Items.AddRange(CropController.Statuses);

        AddLabel("Plot *", 15, 305);
        AddLabel("Crop name *", 15, 340);
        AddLabel("Variety", 15, 375);
        AddLabel("Planting date *", 540, 305);
        AddLabel("Expected harvest *", 540, 340);
        AddLabel("Status *", 540, 375);

        Controls.AddRange(new Control[]
        {
            txtSearch, btnSearch, btnRefresh, dgvCrops,
            cmbPlot, txtCropName, txtVariety, dtpPlanting, dtpHarvest, cmbStatus,
            btnSave, btnUpdate, btnDelete, btnClear
        });

        Load += (s, e) =>
        {
            LoadPlotChoices();
            ResetInputs();
            LoadCrops();
        };
        btnSearch.Click += (s, e) => LoadCrops(txtSearch.Text);
        btnRefresh.Click += (s, e) => { txtSearch.Clear(); LoadCrops(); };
        dgvCrops.CellClick += dgvCrops_CellClick;
        btnSave.Click += btnSave_Click;
        btnUpdate.Click += btnUpdate_Click;
        btnDelete.Click += btnDelete_Click;
        btnClear.Click += (s, e) => ClearForm();
    }

    private void AddLabel(string text, int x, int y) =>
        Controls.Add(new Label { Text = text, Left = x, Top = y, Width = 130 });

    private void ShowError(string title, Exception ex) =>
        MessageBox.Show(ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private void LoadPlotChoices()
    {
        try
        {
            DataTable plots = _controller.GetPlotChoices();
            cmbPlot.DisplayMember = "PlotLabel";
            cmbPlot.ValueMember = "PlotID";
            cmbPlot.DataSource = plots;
            if (plots.Rows.Count == 0)
                MessageBox.Show("Add a farm and a plot first (My Farms and Plots), then come back to register crops.");
        }
        catch (Exception ex) { ShowError("Load plots", ex); }
    }

    private void LoadCrops(string keyword = "")
    {
        try
        {
            dgvCrops.DataSource = _controller.GetCrops(keyword);
            SetColumn("CropID", visible: false);
            SetColumn("PlotID", visible: false);
            SetColumn("CropName", header: "Crop");
            SetColumn("PlantingDate", header: "Planting date", format: "yyyy-MM-dd");
            SetColumn("ExpectedHarvestDate", header: "Expected harvest", format: "yyyy-MM-dd");
            dgvCrops.ClearSelection();
        }
        catch (Exception ex) { ShowError("Load crops", ex); }
    }

    private void SetColumn(string name, bool visible = true, string? header = null, string? format = null)
    {
        DataGridViewColumn? column = dgvCrops.Columns[name];
        if (column == null) return;
        column.Visible = visible;
        if (header != null) column.HeaderText = header;
        if (format != null) column.DefaultCellStyle.Format = format;
    }

    private Crop ReadCrop()
    {
        if (cmbPlot.SelectedValue == null) throw new ArgumentException("Select a plot.");
        return new Crop
        {
            CropID = _selectedCropId,
            PlotID = Convert.ToInt32(cmbPlot.SelectedValue),
            CropName = txtCropName.Text,
            Variety = txtVariety.Text,
            PlantingDate = dtpPlanting.Value.Date,
            ExpectedHarvestDate = dtpHarvest.Value.Date,
            Status = cmbStatus.Text
        };
    }

    private void dgvCrops_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        DataGridViewRow row = dgvCrops.Rows[e.RowIndex];
        _selectedCropId = Convert.ToInt32(row.Cells["CropID"].Value);
        cmbPlot.SelectedValue = Convert.ToInt32(row.Cells["PlotID"].Value);
        txtCropName.Text = Convert.ToString(row.Cells["CropName"].Value);
        txtVariety.Text = Convert.ToString(row.Cells["Variety"].Value);
        dtpPlanting.Value = Convert.ToDateTime(row.Cells["PlantingDate"].Value);
        dtpHarvest.Value = Convert.ToDateTime(row.Cells["ExpectedHarvestDate"].Value);
        cmbStatus.SelectedItem = Convert.ToString(row.Cells["Status"].Value);
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        try
        {
            _selectedCropId = 0;
            _controller.AddCrop(ReadCrop());
            MessageBox.Show("Crop saved.");
            LoadCrops();
            ClearForm();
        }
        catch (Exception ex) { ShowError("Save crop", ex); }
    }

    private void btnUpdate_Click(object? sender, EventArgs e)
    {
        try
        {
            _controller.EditCrop(ReadCrop());
            MessageBox.Show("Crop updated.");
            LoadCrops();
            ClearForm();
        }
        catch (Exception ex) { ShowError("Update crop", ex); }
    }

    private void btnDelete_Click(object? sender, EventArgs e)
    {
        if (_selectedCropId <= 0)
        {
            MessageBox.Show("Select a crop first.");
            return;
        }
        if (MessageBox.Show("Delete this crop? Its tasks and harvest records will also be deleted.",
                "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;
        try
        {
            _controller.RemoveCrop(_selectedCropId);
            MessageBox.Show("Crop deleted.");
            LoadCrops();
            ClearForm();
        }
        catch (Exception ex) { ShowError("Delete crop", ex); }
    }

    private void ResetInputs()
    {
        _selectedCropId = 0;
        txtCropName.Clear();
        txtVariety.Clear();
        dtpPlanting.Value = DateTime.Today;
        dtpHarvest.Value = DateTime.Today.AddDays(90);
        cmbStatus.SelectedIndex = 0;
        if (cmbPlot.Items.Count > 0) cmbPlot.SelectedIndex = 0;
    }

    private void ClearForm()
    {
        ResetInputs();
        dgvCrops.ClearSelection();
        txtCropName.Focus();
    }
}
