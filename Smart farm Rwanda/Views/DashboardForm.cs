using Smart_farm_Rwanda.Models;

namespace Smart_farm_Rwanda.Views;

public class DashboardForm : Form
{
    public DashboardForm()
    {
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        Text = "SmartFarm Rwanda - Dashboard";
        ClientSize = new Size(700, 420);
        StartPosition = FormStartPosition.CenterScreen;

        var lblWelcome = new Label
        {
            Text = $"Welcome, {Session.CurrentUser?.FullName} ({Session.CurrentUser?.Role})",
            Left = 20,
            Top = 20,
            Width = 650,
            Height = 30,
            Font = new Font("Segoe UI", 14, FontStyle.Bold)
        };

        var btnFarms = new Button { Text = "My Farms and Plots", Left = 20, Top = 70, Width = 180, Height = 35 };
        btnFarms.Click += (s, e) =>
        {
            using var form = new FarmsForm();
            form.ShowDialog(this);
        };

        var btnCrops = new Button { Text = "My Crops", Left = 20, Top = 115, Width = 180, Height = 35 };
        btnCrops.Click += (s, e) =>
        {
            using var form = new CropsForm();
            form.ShowDialog(this);
        };

        var btnLogout = new Button { Text = "Logout", Left = 20, Top = 370, Width = 100 };
        btnLogout.Click += (s, e) =>
        {
            Session.CurrentUser = null;
            Close();
        };

        Controls.Add(lblWelcome);
        Controls.Add(btnFarms);
        Controls.Add(btnCrops);
        Controls.Add(btnLogout);
    }
}
