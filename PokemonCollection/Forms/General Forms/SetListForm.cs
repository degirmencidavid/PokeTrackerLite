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

namespace PokemonCollection.Forms.General_Forms
{
    public partial class SetListForm : Form
    {
        private string setID;
        private int setCardMax;

        public SetListForm(string setID, int totalCards, Image setImage)
        {
            InitializeComponent();
            this.setID = setID;
            setCardMax = totalCards;

            pbSet.Visible = true;
            pbSet.Image = setImage;

            InitializeDefaultAsync();

            // Events
            dgvCards.CellDoubleClick += DgvCards_CellDoubleClick;
        }

        private async void InitializeDefaultAsync()
        {
            btnBack.Enabled = false;
            await LoadSetsAsync();
            btnBack.Enabled = true;
        }

        private async Task LoadSetsAsync()
        {
            try
            {
                dgvCards.Rows.Clear();
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.ReadCardsBySetID, con))
                    {
                        cm.Parameters.AddWithValue("@setID", setID);

                        using (SqlDataReader dr = await cm.ExecuteReaderAsync())
                        {
                            ParentFormManager.MainForm.EnableProgressBar();
                            int rowCounter = 0;
                 
                            while (await dr.ReadAsync())
                            {
                                string cardID = dr["CardID"].ToString();
                                string cardName = dr["CardName"].ToString();
                                string language = dr["CardLanguage"].ToString();
                                string setNumber = dr["SetNumber"].ToString();

                                dgvCards.Rows.Add(cardID, cardName, language, setNumber, setID);
                                
                                rowCounter++;
                                ParentFormManager.MainForm.ProgressBarController(rowCounter, setCardMax);
                            }
                            ParentFormManager.MainForm.DisableProgressBar();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (dgvCards.Columns.Count != 0) 
                {
                    MessageBox.Show($"Error loading sets: {ex.Message}");
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }     
            }
        }

        // When going back, 
        private void btnBack_Click(object sender, EventArgs e)
        {
            ParentFormManager.MainForm.DisableProgressBar();
            ParentFormManager.OpenChildForm(new SetForm());
        }

        private void DgvCards_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvCards.Rows[e.RowIndex];

                string cardID = selectedRow.Cells["CardID"].Value.ToString();

                Form form;
                if (!CheckCardFormOpen(cardID, out form))
                {
                    CardForm cardForm = new CardForm(cardID);
                    cardForm.Show();
                }
                else
                {
                    form.BringToFront();
                }   
            }
        }

        private bool CheckCardFormOpen(string cardID, out Form cardForm)
        {
            FormCollection openForms = Application.OpenForms;

            foreach (var form in openForms.OfType<CardForm>())
            {
                if (form.cardID == cardID)
                {
                    cardForm = form;
                    return true;
                }
            }

            cardForm = null;
            return false;
        }

        private void btnCloseCards_Click(object sender, EventArgs e)
        {
            GeneralUtilities.CloseAllCardForms();
        }


        // Search generated

        private string sName;
        private int sSetNum;
        private string sSetName;
        private string sSetID;
        private string[] sLanguagesSelected;
        private bool[] sExplicit;

        public SetListForm(string name, int setNum, string setName, string setID, string[] languagesSelected, bool[] explicitS)
        {
            InitializeComponent();

            pbSet.Visible = false;

            sName = name;
            sSetNum = setNum;
            sSetName = setName;
            sSetID = setID;
            sLanguagesSelected = languagesSelected;
            sExplicit = explicitS;

            InitializeSearchedAsync();

            // Events
            dgvCards.CellDoubleClick += DgvCards_CellDoubleClick;
        }

        private async void InitializeSearchedAsync()
        {
            btnBack.Enabled = false;
            await LoadSearchedCardsAsync();
            btnBack.Enabled = true;
        }

        private async Task LoadSearchedCardsAsync()
        {
            try
            {
                dgvCards.Rows.Clear();

                // ugghghgh
                int rowCounter = 0;
                for (int i = 0; i < sLanguagesSelected.Length; i++)
                {
                    using (SqlConnection con = DataAccess.GetOpenConnection())
                    {
                        using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.CardSearchQuery(sExplicit), con))
                        {
                            
                            cm.Parameters.AddWithValue("@sCardName", sName);
                            cm.Parameters.AddWithValue("@sSetNumber", sSetNum);
                            cm.Parameters.AddWithValue("@sSetID", sSetID);
                            cm.Parameters.AddWithValue("@sCardLanguage", sLanguagesSelected[i]);

                            using (SqlDataReader dr = await cm.ExecuteReaderAsync())
                            {
                                ParentFormManager.MainForm.EnableProgressBar();

                                while (await dr.ReadAsync())
                                {
                                    string cardID = dr[0].ToString();
                                    string cardName = dr[1].ToString();
                                    string language = dr[3].ToString();
                                    string setNumber = dr[4].ToString();
                                    string setID = dr[5].ToString();

                                    if (language == sLanguagesSelected[i])
                                    {
                                        dgvCards.Rows.Add(cardID, cardName, language, setNumber, setID);
                                    }

                                    rowCounter++;
                                    ParentFormManager.MainForm.ProgressBarController(rowCounter, setCardMax);
                                }
                            }
                        }
                    }
                }
                ParentFormManager.MainForm.DisableProgressBar();
                if (dgvCards.Rows.Count == 0)
                {
                    MessageBox.Show($"No results for search term {sName} {sSetNum} {sSetName} {sSetID} in Language(s): {string.Join(", ", sLanguagesSelected)}");
                }
            }
            catch (Exception ex)
            {
                if (dgvCards.Columns.Count != 0)
                {
                    MessageBox.Show($"Error loading search: {ex.Message}");
                }
                else
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }


        // CollectionForm implement it here maybe


    }
}
