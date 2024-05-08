using PokemonCollection.Forms.User_Forms;
using PokemonCollection.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourProjectName.DataAccess;

namespace PokemonCollection
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_ClickAsync(null, EventArgs.Empty);
            }
        }

        // Clear label
        private void lblClear_Click(object sender, EventArgs e)
        {
            txtName.Clear();
            txtPass.Clear();
        }

        private void checkBoxPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPass.UseSystemPasswordChar = !checkBoxPass.Checked;
        }


        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Close Application?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();

            registerForm.Show();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtName.Text;
                string hashedpw = HashingUtility.HashPassword(txtPass.Text);

                var (authenticated, userID, sessionToken) = AuthenticateUser(username, hashedpw);

                if (authenticated)
                {
                    MessageBox.Show("Login Success!");

                    this.Hide();

                    string userRole = GetUserRole(username);

                    if (userRole.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    {
                        MainForm mainForm = new MainForm();
                        mainForm.Show();
                    }
                    else
                    {
                        UserMainForm userMainForm = new UserMainForm(userID, sessionToken);
                        userMainForm.Show();
                    }                 
                }
                else
                {
                    MessageBox.Show("Invalid username/password!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            } 
        }

        private async void btnLogin_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                await LoginAsync();
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Error logging in {ex.Message}");
            }
        }

        private async Task LoginAsync()
        {
            string username = txtName.Text;
            string hashedpw = HashingUtility.HashPassword(txtPass.Text);

            var (authenticated, userID, sessionToken) = await AuthenticateUserAsync(username, hashedpw);

            if (authenticated)
            {
                MessageBox.Show("Login Success!");

                this.Hide();

                string userRole = await GetUserRoleAsync(username);

                if (userRole.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    MainForm mainForm = new MainForm();
                    mainForm.Show();
                }
                else
                {
                    UserMainForm userMainForm = new UserMainForm(userID, sessionToken);
                    userMainForm.Show();
                }
            }
            else
            {
                MessageBox.Show("Invalid username/password!");
            }
        }

        private async Task<string> GetUserRoleAsync(string username)
        {
            using (SqlConnection con = DataAccess.GetOpenConnection())
            {
                using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.ReadUserRole, con))
                {
                    cm.Parameters.AddWithValue("@username", username);
                    var result = await cm.ExecuteScalarAsync();
                    return result?.ToString();
                }
            }
        }

        private string GetUserRole(string username)
        {
            using (SqlConnection con = DataAccess.GetOpenConnection())
            {
                using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.ReadUserRole, con))
                {
                    cm.Parameters.AddWithValue("@username", username);
                    return cm.ExecuteScalar()?.ToString();
                }
            }
        }

        private int IDFromUsername(string username)
        {
            using (SqlConnection con = DataAccess.GetOpenConnection())
            {
                using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.IDFromUsername, con))
                {
                    cm.Parameters.AddWithValue("@username", username);
                    return (int)cm.ExecuteScalar();
                }
            }
        }

        private (bool, int, string) AuthenticateUser(string username, string hashedpw)
        {
            using (SqlConnection con = DataAccess.GetOpenConnection())
            {
                using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.ReadUserAuth, con))
                {
                    cm.Parameters.AddWithValue("@username", username);
                    cm.Parameters.AddWithValue("@password", hashedpw);

                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            int userID = Convert.ToInt32(dr["UserID"]);
                            SessionManager.UpdateSessionTokenInDB(userID.ToString());

                            string sessionToken = dr["SessionToken"].ToString();
                            SessionManager.StartSession(userID.ToString());

                            return (true, userID, sessionToken);
                        }
                        else
                        {
                            return (false, 0, null);
                        }   
                    }
                }
            }
        }

        private async Task<(bool, int, string)> AuthenticateUserAsync(string username, string hashedpw)
        {
            using (SqlConnection con = DataAccess.GetOpenConnection())
            {
                using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.ReadUserAuth, con))
                {
                    cm.Parameters.AddWithValue("@username", username);
                    cm.Parameters.AddWithValue("@password", hashedpw);

                    using (SqlDataReader dr = await cm.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            int userID = Convert.ToInt32(dr["UserID"]);
                            SessionManager.UpdateSessionTokenInDB(userID.ToString());

                            string sessionToken = dr["SessionToken"].ToString();
                            SessionManager.StartSession(userID.ToString());

                            return (true, userID, sessionToken);
                        }
                        else
                        {
                            return (false, 0, null);
                        }
                    }
                }
            }
        }

        private void autoLogin_Click(object sender, EventArgs e)
        {
            txtName.Text = "admin";
            txtPass.Text = "a";
            btnLogin_ClickAsync(null, EventArgs.Empty);
        }
    }
}
