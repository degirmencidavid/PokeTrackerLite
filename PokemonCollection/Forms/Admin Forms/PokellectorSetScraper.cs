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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PokemonCollection.Forms.Admin_Forms
{
    public partial class PokellectorSetScraper : Form
    {
        public static string setID;

        public PokellectorSetScraper()
        {
            InitializeComponent();
            DisableProgressBar();
        }


        public void EnableProgressBar()
        {
            progressBarCards.Enabled = true;
            progressBarCards.Visible = true;
            labelDC.Visible = true;
        }

        public void DisableProgressBar()
        {
            progressBarCards.Enabled = false;
            progressBarCards.Visible = false;
            labelDC.Visible = false;
            ResetProgressBar();
        }

        private void ResetProgressBar()
        {
            ProgressBarController(0, 1);
        }

        private void ProgressBarController(int current, int total)
        {
            progressBarCards.Minimum = 0;
            progressBarCards.Maximum = total;
            progressBarCards.Value = Math.Min(current, total);
        }

        private void LabelController(int current)
        {
            labelDC.Text = "Downloading Cards";

            for (int i = 0; i <= current % 3;  i++)
            {
                labelDC.Text = labelDC.Text + ".";
            }
        }

        private void DisableTextFields()
        {
            txtSetID.Enabled = false;
            txtSetUrl.Enabled = false;
        }

        private void EnableTextFields()
        {
            txtSetID.Enabled = true;
            txtSetUrl.Enabled = true;
        }


        /* Todo: (some in CardScraper)
         * 
         * Sets:
         * Get text from image from the set code
         * 
         */

        private async void btnScrape_Click(object sender, EventArgs e)
        {
            try
            {
                setID = txtSetID.Text;

                btnScrape.Enabled = false;

                string url = txtSetUrl.Text;
                CardScraper cardScraper = new CardScraper();
                var scrapedSet = await cardScraper.ScrapeCardsAsync(url);
                List<List<string>> cardDataList = scrapedSet.CardDataList;
                List<string> setInfo = scrapedSet.SetInfo;

                listBox.Items.Clear();

                listBox.Items.Add($"Set Name: {setInfo[0]}, Image: {setInfo[1]}, Code: {setInfo[2]}, Language: {setInfo[3]}, Released: {setInfo[4]}");

                foreach (var cardData in cardDataList)
                {
                    listBox.Items.Add($"Name: {cardData[2]}, Number: {cardData[1]}, URL: {cardData[0]}");
                }

                //MessageBox.Show("Scraping completed successfully");


                // Insert Set
                // when multiple are accessed from main page, get link from htm
                

                // This needs to be fixed
                if (await SetAlreadyInDBAsync(setID))
                {
                    MessageBox.Show($"Set already in DB {setID} or error");
                    return;
                }

                string setName = setInfo[0];
                int totalCards = listBox.Items.Count - 1;
                string released = setInfo[4];

                string setImageUrl = setInfo[1];
                byte[] setImageBytes = await ImageHandling.DownloadImageAsync(setImageUrl);

                await InsertSetAsync(setID, setName, totalCards, released, setImageBytes);

                // Insert Cards
                string cardLanguage = setInfo[3];

                int count = 0;
                EnableProgressBar();
                DisableTextFields();
                ProgressBarController(count, totalCards);

                async Task DownloadCard(List<string> cardData)
                {
                    LabelController(count);

                    string cardName = cardData[2];
                    string setNumber = cardData[1];

                    string cardImageUrl = cardData[0];
                    byte[] cardImageBytes = await ImageHandling.DownloadImageAsync(cardImageUrl);
                    await InsertCardAsync(cardName, cardLanguage, setNumber, setID, cardImageBytes);

                    count++;
                    ProgressBarController(count, totalCards);
                }

                await cardDataList.Select<List<string>, Func<Task>>((cardData) => (() => DownloadCard(cardData))).Pool();
                
                /*
                var tasks = new List<Func<Task>>();
                foreach (var cardData in cardDataList)
                {
                    tasks.Add(() => DownloadCard(cardData));
                }
                await tasks.Pool();
                */

                labelDC.Text = "Download Complete!";

                //MessageBox.Show("Cards successfully added to database");
                await Task.Delay(100);
                DisableProgressBar();
                EnableTextFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                btnScrape.Enabled = true;
            }
        }

        private async Task<bool> SetAlreadyInDBAsync(string sSetID)
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.ReadSetIDs, con))
                    {
                        using (SqlDataReader dr = await cm.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                string setIDFromDB = dr["SetID"].ToString();

                                if (setIDFromDB == sSetID)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking set existence: {ex.Message}");
                return true;
            }
        }

        private async Task InsertSetAsync(string setID, string setName, int totalCards, string released, byte[] setImage)
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.InsertCardSet, con))
                    {
                        cm.Parameters.Add(new SqlParameter("@setID", SqlDbType.NVarChar) { Value = setID });
                        cm.Parameters.Add(new SqlParameter("@setName", SqlDbType.NVarChar) { Value = setName });
                        cm.Parameters.Add(new SqlParameter("@totalCards", SqlDbType.Int) { Value = totalCards });
                        cm.Parameters.Add(new SqlParameter("@released", SqlDbType.NVarChar) { Value = released });
                        cm.Parameters.Add(new SqlParameter("@setImage", SqlDbType.VarBinary) { Value = setImage });

                        await cm.ExecuteNonQueryAsync();
                        //MessageBox.Show("Card Set successfully added to database");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting set: {ex.Message}");
            }
        }

        private async Task InsertCardAsync(string cardName, string cardLanguage, string setNumber, string setID, byte[] cardImage)
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.InsertCard, con))
                    {
                        cm.Parameters.Add(new SqlParameter("@cardName", SqlDbType.NVarChar) { Value = cardName });
                        cm.Parameters.Add(new SqlParameter("language", SqlDbType.NVarChar) { Value = cardLanguage });
                        cm.Parameters.Add(new SqlParameter("@setNumber", SqlDbType.NVarChar) { Value = setNumber });
                        cm.Parameters.Add(new SqlParameter("@setID", SqlDbType.NVarChar) { Value = setID });
                        cm.Parameters.Add(new SqlParameter("@cardImage", SqlDbType.VarBinary) { Value = cardImage });

                        await cm.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting card: {cardName} #{setNumber} {ex.Message}");
            }
        }

    }
}
