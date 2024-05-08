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
    public partial class CollectionForm : Form
    {
        private int userID;
        private string sessionToken;
        
        public CollectionForm()
        {
            InitializeComponent();
            InitializeAsync();

            // Events
            dgvCollection.CellDoubleClick += DgvCollection_CellDoubleClick;
        }

        private async void InitializeAsync()
        {
            sessionToken = await SessionManager.GetSessionTokenAsync();
            userID = await SessionManager.GetUserID(sessionToken);
            await LoadUserCollectionAsync();
        }

        public DataGridView DgvCollection
        {
            get { return dgvCollection; }
        }


        private async Task LoadUserCollectionAsync()
        {
            try
            {
                dgvCollection.Rows.Clear();

                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.ReadUserCollectionByID, con))
                    {
                        cm.Parameters.AddWithValue("@userID", userID);

                        using (SqlDataReader dr = await cm.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                // Get card properties
                                string cardID = dr["CardID"].ToString();
                                var card = await CardManager.GetCardByID(cardID);

                                var conditionA = dr["ConditionA"];
                                var conditionAM = dr["ConditionAM"];
                                var conditionB = dr["ConditionB"];
                                var conditionC = dr["ConditionC"];
                                var conditionPSA10 = dr["ConditionPSA10"];
                                var conditionPSA9 = dr["ConditionPSA9"];
                                var conditionPSA8 = dr["ConditionPSA8"];

                                string ownedA = Convert.IsDBNull(conditionA) ? "0" : conditionA.ToString();
                                string ownedAM = Convert.IsDBNull(conditionAM) ? "0" : conditionAM.ToString();
                                string ownedB = Convert.IsDBNull(conditionB) ? "0" : conditionB.ToString();
                                string ownedC = Convert.IsDBNull(conditionC) ? "0" : conditionC.ToString();
                                string ownedPSA10 = Convert.IsDBNull(conditionPSA10) ? "0" : conditionPSA10.ToString();
                                string ownedPSA9 = Convert.IsDBNull(conditionPSA9) ? "0" : conditionPSA9.ToString();
                                string ownedPSA8 = Convert.IsDBNull(conditionPSA8) ? "0" : conditionPSA8.ToString();

                                dgvCollection.Rows.Add(cardID, card.cardName, card.cardLanguage, card.setNumber, card.setID, card.cardPrice, ownedA, ownedAM, ownedB, ownedC, ownedPSA10, ownedPSA9, ownedPSA8);

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

        private void DgvCollection_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvCollection.Rows[e.RowIndex];

                string cardID = selectedRow.Cells["CardID"].Value.ToString();

                CardForm cardForm = new CardForm(cardID);
                cardForm.Show();
            }
        }


    }
}