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
    public partial class UserForm : Form
    {

        public UserForm()
        {
            InitializeComponent();
            LoadUser();
        }

        public void LoadUser()
        {
            // Clear the rows
            dgvUser.Rows.Clear();

            using (SqlConnection connection = DataAccess.CreateConnection()) 
            {
                using (SqlCommand command = DataAccess.CreateCommand(SQLCommands.ReadAllUsers, connection))
                {
                    connection.Open();
                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        int i = 0;
                        while (dr.Read())
                        {
                            i++;
                            dgvUser.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString());
                        }
                    }
                }
            }
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            UserModule userModule = new UserModule();
            userModule.txtUsername.Enabled = true;
            userModule.btnSave.Enabled = true;
            userModule.btnUpdate.Enabled = false;
            userModule.ShowDialog();

            LoadUser();
        }

        private void dgvUser_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvUser.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                UserModule userModule = new UserModule();
                
                userModule.txtUsername.Text = dgvUser.Rows[e.RowIndex].Cells["Username"].Value.ToString();
                userModule.txtFullName.Text = dgvUser.Rows[e.RowIndex].Cells["FullName"].Value.ToString();
                userModule.txtPhone.Text = dgvUser.Rows[e.RowIndex].Cells["Phone"].Value.ToString();

                userModule.txtUsername.Enabled = false;
                userModule.btnSave.Enabled = false;
                userModule.btnUpdate.Enabled = true;
                userModule.ShowDialog();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this user?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (SqlConnection con = DataAccess.GetOpenConnection())
                    {
                        using (SqlCommand cmd = DataAccess.CreateCommand(SQLCommands.DeleteUser("username"), con))
                        {
                            string user = dgvUser.Rows[e.RowIndex].Cells["Username"].Value.ToString();
                            cmd.Parameters.AddWithValue("@propertyValue", user);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("User has been successfully deleted");
                        }
                    }
                }
            }
            LoadUser();
        }
    }
}
