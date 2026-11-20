using Smart_farm_Rwanda.Models;
using Smart_farm_Rwanda.Views;

namespace Smart_farm_Rwanda;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        while (true)
        {
            using var login = new LoginForm();
            if (login.ShowDialog() != DialogResult.OK) break;

            Application.Run(new DashboardForm());
            if (Session.CurrentUser != null) break;
        }
    }
}