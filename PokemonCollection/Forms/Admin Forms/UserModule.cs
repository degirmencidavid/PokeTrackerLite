using PokemonCollection.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourProjectName.DataAccess;

namespace PokemonCollection
{
    public partial class UserModule : Form
    {

        private SqlConnection con = DataAccess.CreateConnection();
        private SqlCommand cm;

        public UserModule()
        {
            InitializeComponent();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = DataAccess.GetOpenConnection())
            {
                using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.InsertUser, con))
                {
                    try
                    {
                        if (MessageBox.Show("Save this user?", "Saving record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {                         
                            string _username = txtUsername.Text;
                            string _fullname = txtFullName.Text;
                            string _hashedpw = HashingUtility.HashPassword(txtPass.Text);
                            string _hashedpwconf = HashingUtility.HashPassword(txtPassConfirm.Text);
                            string _phone = txtPhone.Text;

                            // Checks
                            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_fullname) || string.IsNullOrEmpty(_hashedpw) || string.IsNullOrEmpty(_hashedpwconf) || string.IsNullOrEmpty(_phone))
                            {
                                MessageBox.Show("Please fill in all the fields");
                                return;
                            }
                            if (CheckUserExists(_username))
                            {
                                MessageBox.Show("Username in use, please choose another");
                                return;
                            }

                            if (_hashedpw != _hashedpwconf)
                            {
                                MessageBox.Show("Passwords do not match!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            cm.Parameters.AddWithValue("@username", _username);
                            cm.Parameters.AddWithValue("@fullname", _fullname);
                            cm.Parameters.AddWithValue("@password", _hashedpw);
                            cm.Parameters.AddWithValue("@phone", _phone);
                            cm.ExecuteNonQuery();
                            MessageBox.Show("User has been successfully saved");
                            ClearAllTextFields();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        public static bool CheckUserExists(string username)
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using(SqlCommand com = DataAccess.CreateCommand(SQLCommands.CountProperties("username"), con))
                {
                        com.Parameters.AddWithValue("@value", username);
                        int count = (int)com.ExecuteScalar();
                        return count != 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return true;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllTextFields();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
        }

        public void ClearAllTextFields()
        {
            foreach (Control control in Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Text = string.Empty;
                }
            }
        }
        
        
        // needs some work
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.UpdateUser, con))
                    {
                        cm.Parameters.AddWithValue("@unValue", txtUsername.Text);

                        string warningMsg = "";
                        if (MessageBox.Show("Update this user?", "Saving record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            string _fullname = txtFullName.Text;
                            string _hashedpw = HashingUtility.HashPassword(txtPass.Text);
                            string _hashedpwconf = HashingUtility.HashPassword(txtPassConfirm.Text);
                            string _phone = txtPhone.Text;

                            if (_hashedpw != _hashedpwconf)
                            {
                                warningMsg += "\nPasswords do not match!";
                            }
                            else if (!string.IsNullOrEmpty(_hashedpw))
                            {
                                cm.Parameters.AddWithValue("@password", _hashedpw);
                            }


                            if (!string.IsNullOrEmpty(_fullname))
                            {
                                cm.Parameters.AddWithValue("@fullname", _fullname);
                            }

                            if (!string.IsNullOrEmpty(_phone))
                            {
                                cm.Parameters.AddWithValue("@phone", _phone);
                            }

                            if (!string.IsNullOrEmpty(warningMsg))
                            {
                                MessageBox.Show(warningMsg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            cm.ExecuteNonQuery();
                            MessageBox.Show("User has been successfully updated");
                            this.Dispose();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            
        }
    }
}
