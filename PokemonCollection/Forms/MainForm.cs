using PokemonCollection.Forms.Admin_Forms;
using PokemonCollection.Forms.General_Forms;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace PokemonCollection
{
    public partial class MainForm : Form
    {

        private static Form activeForm = null;
        public Panel panel;
        private static PokellectorSetScraper setScraper;

        public MainForm()
        {
            InitializeComponent();
            ParentFormManager.MainForm = this;
            panel = panelMain;
            progressBar1.Visible = false;
            InitializeSearchProperties();
        }

        public void EnableProgressBar()
        {
            progressBar1.Enabled = true;
            progressBar1.Visible = true;
        }

        public void DisableProgressBar()
        {
            progressBar1.Enabled = false;
            progressBar1.Visible = false;
            ResetProgressBar();
        }

        private void ResetProgressBar()
        {
            ProgressBarController(0, 1);
        }

        // Make read current from the other form and do it async
        public void ProgressBarController(int current, int total)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = total;
            progressBar1.Value = Math.Min(current, total);
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            ParentFormManager.OpenChildForm(new UserForm());
        }

        private void btnSets_Click(object sender, EventArgs e)
        {
            ParentFormManager.OpenChildForm(new SetForm());
        }

        private void btnCollection_Click(object sender, EventArgs e)
        {
            ParentFormManager.OpenChildForm(new CollectionForm());
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            ParentFormManager.MainForm.DisableProgressBar();
            ParentFormManager.CloseCurrentChildForm();
        }


        // Search //

        // Search propeties
        private bool cardSetChecked = false;
        private bool cardChecked = false;

        private bool jpChecked = false;
        private bool engChecked = false;

        private string languagesSelected;
        private string[] languages;
        private string name;
        private int setNum;
        private string setName;
        private string setID;

        private Dictionary<string, string> cachedSetDict = new Dictionary<string, string>();


        private void InitializeSearchProperties()
        {
            txtName.Enabled = false;
            txtSetNum.Enabled = false;
            cbSet.Enabled = false;
            cbSetID.Enabled = false;

            btnSearch.Enabled = false;

            InitializeComboAsync();
        }

        private async void InitializeComboAsync()
        {
            await PopulateSetLists();
        }

        private void UpdateTicksSF()
        {
            txtName.Enabled = cardChecked;
            txtSetNum.Enabled = cardChecked;

            cbSet.Enabled = cardChecked || cardSetChecked;
            cbSetID.Enabled = cardChecked || cardSetChecked;

            jpChecked = checkJp.Checked;
            engChecked = checkEng.Checked;

            btnSearch.Enabled = (cardSetChecked || cardChecked) && (jpChecked || engChecked);
           }

        // Card Set or Card
        private void checkCardSet_CheckedChanged(object sender, EventArgs e)
        {
            cardSetChecked = checkCardSet.Checked;
            if (cardSetChecked) 
            {
                checkCard.Checked = !checkCardSet.Checked;
                cardChecked = checkCard.Checked;
            }
            UpdateTicksSF();
        }

        private void checkCard_CheckedChanged(object sender, EventArgs e)
        {
            cardChecked = checkCard.Checked;
            if (cardChecked)
            {
                checkCardSet.Checked = !checkCard.Checked;
                cardSetChecked = checkCardSet.Checked;
            }
            UpdateTicksSF();
        }

        // Languages (will change what is available in drop down box)
        private void checkJp_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTicksSF();
        }

        private void checkEng_CheckedChanged(object sender, EventArgs e)
        { 
            UpdateTicksSF();
        }


        // Set number only allow numbers
        private void txtSetNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private async Task PopulateSetLists()
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.ReadAllCardSets, con))
                    {
                        using (SqlDataReader dr = await cm.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                string currentSetID = dr["SetID"].ToString();
                                string currentSetName = dr["SetName"].ToString();

                                cbSetID.Items.Add(currentSetID);
                                cbSet.Items.Add(currentSetName);

                                // Cache the sets to set one to the other when the list is changed
                                cachedSetDict[currentSetID] = currentSetName;
                                cachedSetDict[currentSetName] = currentSetID;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating Set combo boxes: {ex.Message}");
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            name = txtName.Text;
            int.TryParse(txtSetNum.Text, out setNum);
            setName = cbSet.Text;
            setID = cbSetID.Text;

            languagesSelected = "Japanese English";
            if (!(jpChecked && engChecked))
            {
                languagesSelected = jpChecked ? "Japanese" : "English";
            }
            languages = languagesSelected.Split(' ');

            SearchAsync();
        }

        private void SearchAsync()
        {
            if (cardChecked)
            {
                CardSearch();
            }
        }

        private void CardSearch()
        {
            bool[] explicitSearch = { !string.IsNullOrEmpty(name), setNum!=0, !string.IsNullOrEmpty(setID) };
            ParentFormManager.OpenChildForm(new SetListForm(name, setNum, setName, setID, languages, explicitSearch));
        }

        private void btnAddSet_Click(object sender, EventArgs e)
        {
            if (setScraper == null || setScraper.IsDisposed)
            {
                setScraper = new PokellectorSetScraper();
                setScraper.Show();
            }
            else
            {
                setScraper.BringToFront();
            }   
        }

        private bool cbSetEventEnabled = true;
        private bool cbSetIDEventEnabled = true;

        private async void cbSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSetEventEnabled)
            {
                cbSetIDEventEnabled = false;
                cbSetID.SelectedIndex = -1;
                await Task.Delay(1);
                cbSetID.SelectedIndex = cbSetID.Items.IndexOf(cachedSetDict[cbSet.SelectedItem.ToString()]);
                cbSetIDEventEnabled = true;
            }
        }

        private async void cbSetID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSetIDEventEnabled)
            {
                cbSetEventEnabled = false;
                cbSet.SelectedIndex = -1;
                await Task.Delay(1);
                cbSet.SelectedIndex = cbSet.Items.IndexOf(cachedSetDict[cbSetID.SelectedItem.ToString()]);
                cbSetEventEnabled = true;
            }
        }

        private void btnCards_Click(object sender, EventArgs e)
        {
            CreateCard createCard = new CreateCard();
            createCard.Show();
        }

        private void btnValuation_Click(object sender, EventArgs e)
        {

        }
    }
}
