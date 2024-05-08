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
    public partial class CreateCardSet : Form
    {
        private SqlConnection con = DataAccess.CreateConnection();
        private OpenFileDialog openFileDialog;

        public CreateCardSet()
        {
            InitializeComponent();
            
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.InsertCardSet, connection))
                    {
                        string setID = txtSetID.Text;
                        string setName = txtSetName.Text;
                        int totalCards = Convert.ToInt32(txtTotalCards.Text);
                        string released = txtDate.Text;
                        byte[] setImage = ImageHandling.GetImageBytes(picImage.ImageLocation);

                        cm.Parameters.AddWithValue("@setID", setID);
                        cm.Parameters.AddWithValue("@setName", setName);
                        cm.Parameters.AddWithValue("@totalCards", totalCards);
                        cm.Parameters.AddWithValue("@released", released);
                        cm.Parameters.AddWithValue("@setImage", setImage);

                        cm.ExecuteNonQuery();
                        MessageBox.Show("Card Set successfully added");

                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            txtSetID.Text = string.Empty;
            txtSetName.Text = string.Empty;
            txtTotalCards.Text = string.Empty;
        }


        // Set the image
        private void picImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                picImage.ImageLocation = openFileDialog.FileName;
            }
        }

    }
}
