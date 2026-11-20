using Smart_farm_Rwanda.Controllers;
using Smart_farm_Rwanda.Models;

namespace Smart_farm_Rwanda.Views;

public class RegisterForm : Form
{
    private readonly AuthController _auth = new(new UserRepository(Database.ConnectionString));
    private readonly TextBox txtFullName = new() { Left = 150, Top = 20, Width = 210 };
    private readonly TextBox txtUsername = new() { Left = 150, Top = 60, Width = 210 };
    private readonly TextBox txtPhone = new() { Left = 150, Top = 100, Width = 210 };
    private readonly TextBox txtPassword = new() { Left = 150, Top = 140, Width = 210, UseSystemPasswordChar = true };
    private readonly TextBox txtConfirm = new() { Left = 150, Top = 180, Width = 210, UseSystemPasswordChar = true };
    private readonly Button btnSave = new() { Text = "Register", Left = 150, Top = 225, Width = 100 };
    private readonly Button btnCancel = new() { Text = "Cancel", Left = 260, Top = 225, Width = 100 };

    public RegisterForm()
    {
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        Text = "Create account";
        ClientSize = new Size(390, 275);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        string[] labels = { "Full name *", "Username *", "Phone (optional)", "Password *", "Confirm password *" };
        for (int i = 0; i < labels.Length; i++)
            Controls.Add(new Label { Text = labels[i], Left = 20, Top = 23 + i * 40, Width = 125 });

        Controls.AddRange(new Control[] { txtFullName, txtUsername, txtPhone, txtPassword, txtConfirm, btnSave, btnCancel });

        AcceptButton = btnSave;
        CancelButton = btnCancel;
        btnSave.Click += btnSave_Click;
        btnCancel.Click += (s, e) => Close();
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        try
        {
            _auth.Register(txtFullName.Text, txtUsername.Text, txtPassword.Text,
                           txtConfirm.Text, txtPhone.Text);
            MessageBox.Show("Account created. You can now log in.", "Register");
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Register", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}