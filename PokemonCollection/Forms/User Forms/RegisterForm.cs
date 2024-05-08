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

namespace PokemonCollection.Forms.User_Forms
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            // if fields are empty then just dispose, otherwise confirm, but idk how without rewriting
            bool nonconfirmable = (string.IsNullOrEmpty(txtName.Text) && string.IsNullOrEmpty(txtFullName.Text) && string.IsNullOrEmpty(txtPass.Text) && string.IsNullOrEmpty(txtPassConfirm.Text) && string.IsNullOrEmpty(txtPhone.Text));
            
            if (nonconfirmable)
            {
                this.Dispose();
                return;
            }

            if (MessageBox.Show("Cancel registration?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Dispose();
            }
        }

        private void checkBoxPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPass.UseSystemPasswordChar = !checkBoxPass.Checked;
            txtPassConfirm.UseSystemPasswordChar = !checkBoxPass.Checked;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = DataAccess.GetOpenConnection())
            {
                using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.InsertUser, con))
                {
                    try
                    {
                        string username = txtName.Text;
                        string fullname = txtFullName.Text;
                        string hashedpw = HashingUtility.HashPassword(txtPass.Text);
                        string hashedpwconf = HashingUtility.HashPassword(txtPassConfirm.Text);
                        string phone = txtPhone.Text;

                        // Checks
                        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(hashedpw) || string.IsNullOrEmpty(hashedpwconf) || string.IsNullOrEmpty(phone))
                        {
                            MessageBox.Show("Please fill in all fields");
                            return;
                        }
                        if (UserModule.CheckUserExists(username))
                        {
                            MessageBox.Show("Username in use, please choose another");
                            return;
                        }
                        if (hashedpw != hashedpwconf)
                        {
                            MessageBox.Show("Passwords do not match!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        cm.Parameters.AddWithValue("@username", username);
                        cm.Parameters.AddWithValue("@fullname", fullname);
                        cm.Parameters.AddWithValue("@password", hashedpw);
                        cm.Parameters.AddWithValue("@phone", phone);

                        cm.ExecuteNonQuery();
                        MessageBox.Show("Account successfully created!");

                        this.Dispose();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private async void btnRegister_ClickAsync(object sender, EventArgs e)
        {
            using (SqlConnection con = DataAccess.GetOpenConnection())
            {
                using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.InsertUser, con))
                {
                    try
                    {
                        string username = txtName.Text;
                        string fullname = txtFullName.Text;
                        string hashedpw = HashingUtility.HashPassword(txtPass.Text);
                        string hashedpwconf = HashingUtility.HashPassword(txtPassConfirm.Text);
                        string phone = txtPhone.Text;

                        // Checks
                        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(hashedpw) || string.IsNullOrEmpty(hashedpwconf) || string.IsNullOrEmpty(phone))
                        {
                            MessageBox.Show("Please fill in all fields");
                            return;
                        }
                        if (UserModule.CheckUserExists(username))
                        {
                            MessageBox.Show("Username in use, please choose another");
                            return;
                        }
                        if (hashedpw != hashedpwconf)
                        {
                            MessageBox.Show("Passwords do not match!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        cm.Parameters.AddWithValue("@username", username);
                        cm.Parameters.AddWithValue("@fullname", fullname);
                        cm.Parameters.AddWithValue("@password", hashedpw);
                        cm.Parameters.AddWithValue("@phone", phone);

                        await cm.ExecuteNonQueryAsync();
                        MessageBox.Show("Account successfully created!");

                        this.Dispose();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }
    }
}
