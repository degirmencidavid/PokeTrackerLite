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
using PokemonCollection.Utilities;
using System.IO;

namespace PokemonCollection.Forms.Admin_Forms
{
    public partial class CreateCard : Form
    {

        private bool isImageDownloaded = false;
        private byte[] downloadedImageData;

        public CreateCard()
        {
            InitializeComponent();

            // Apparently this is better than just putting PopulateSetIDs in here :/
            this.Load += new System.EventHandler(this.CreateCard_Load);

            txtImgUrl.TextChanged += TxtImgUrl_TextChanged;
         
            btnAdd.Enabled = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try 
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.InsertCard, con))
                    {
                        string setID = cmbSetID.SelectedItem.ToString(); ;
                        string cardName = txtCardName.Text;
                        string setNumber = txtSetNumber.Text;
                        string language = txtLanguage.Text;

                        if (isImageDownloaded && downloadedImageData != null)
                        {
                            cm.Parameters.AddWithValue("@setId", setID);
                            cm.Parameters.AddWithValue("@cardName", cardName);
                            cm.Parameters.AddWithValue("@setNumber", setNumber);
                            cm.Parameters.AddWithValue("@language", language);
                            cm.Parameters.AddWithValue("@cardImage", downloadedImageData);

                            cm.ExecuteNonQuery();

                            MessageBox.Show("Card saved successfully");

                            ClearForm();
                        }
                        else
                        {
                            MessageBox.Show("Please download an image before adding the card.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void CreateCard_Load(object sender, EventArgs e)
        {
            PopulateSetIDs();
        }

        private void PopulateSetIDs()
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.ReadAllCardSets, con))
                    {
                        using (SqlDataReader dr = cm.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                cmbSetID.Items.Add(dr["SetID"].ToString());
                            }
                        }
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
            cmbSetID.SelectedIndex = -1;
            txtCardName.Text = string.Empty;
            txtSetNumber.Text = string.Empty;
            txtLanguage.Text = string.Empty;
            txtImgUrl.Text = string.Empty;
            picImage.Image = null;
            isImageDownloaded = false;
            btnAdd.Enabled = false;
        }
        
        private void TxtImgUrl_TextChanged(object sender, EventArgs e)
        {
            // enable button when url is not empty and image is downloaded
            btnAdd.Enabled = !string.IsNullOrWhiteSpace(txtImgUrl.Text);
        }

        private void HandleDownloadedImage(byte[] imageData)
        {
            ImageHandling.HandleDownloadedImage(imageData, picImage);
            downloadedImageData = imageData;
            isImageDownloaded = imageData != null;
            TxtImgUrl_TextChanged(null, EventArgs.Empty);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            // Download the image and display it
            ImageHandling.DownloadImage(txtImgUrl.Text, HandleDownloadedImage);
        }
    }
}