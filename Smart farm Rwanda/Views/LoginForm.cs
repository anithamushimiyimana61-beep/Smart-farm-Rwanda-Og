using Smart_farm_Rwanda.Controllers;
using Smart_farm_Rwanda.Models;

namespace Smart_farm_Rwanda.Views;

public class LoginForm : Form
{
    private readonly AuthController _auth = new(new UserRepository(Database.ConnectionString));
    private readonly TextBox txtUsername = new() { Left = 130, Top = 60, Width = 200 };
    private readonly TextBox txtPassword = new() { Left = 130, Top = 100, Width = 200, UseSystemPasswordChar = true };
    private readonly Button btnLogin = new() { Text = "Login", Left = 130, Top = 150, Width = 95 };
    private readonly Button btnRegister = new() { Text = "Register", Left = 235, Top = 150, Width = 95 };

    public LoginForm()
    {
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        Text = "SmartFarm Rwanda - Login";
        ClientSize = new Size(380, 220);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        Controls.Add(new Label { Text = "SmartFarm Rwanda", Left = 20, Top = 15, Width = 340, Height = 30, Font = new Font("Segoe UI", 14, FontStyle.Bold) });
        Controls.Add(new Label { Text = "Username", Left = 20, Top = 63, Width = 100 });
        Controls.Add(new Label { Text = "Password", Left = 20, Top = 103, Width = 100 });
        Controls.AddRange(new Control[] { txtUsername, txtPassword, btnLogin, btnRegister });

        AcceptButton = btnLogin;
        btnLogin.Click += btnLogin_Click;
        btnRegister.Click += btnRegister_Click;
    }

    private void btnLogin_Click(object? sender, EventArgs e)
    {
        try
        {
            Session.CurrentUser = _auth.Login(txtUsername.Text, txtPassword.Text);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void btnRegister_Click(object? sender, EventArgs e)
    {
        using var form = new RegisterForm();
        form.ShowDialog(this);
    }
}