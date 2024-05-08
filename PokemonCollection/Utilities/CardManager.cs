using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourProjectName.DataAccess;

namespace PokemonCollection.Utilities
{
    public static class CardManager
    {

        public static async Task<(string cardID, string cardName, string cardPrice, string cardLanguage, string setNumber, string setID)> GetCardByID(string cardID)
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.ReadCardsByCardID, con))
                    {
                        cm.Parameters.AddWithValue("@cardID", cardID);

                        using (SqlDataReader dr = await cm.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync())
                            {
                                cardID = dr["CardID"].ToString();
                                string cardName = dr["CardName"]?.ToString() ?? "N/A";
                                string cardPrice = dr["CardPrice"]?.ToString() ?? "N/A";
                                string cardLanguage = dr["CardLanguage"]?.ToString() ?? "N/A";
                                string setNumber = dr["SetNumber"]?.ToString() ?? "N/A";
                                string setID = dr["SetID"]?.ToString() ?? "N/A";

                                return (cardID, cardName, cardPrice, cardLanguage, setNumber, setID);
                            }
                            else
                            {
                                MessageBox.Show("No data found for the specified CardID.");
                                return (null, null, null, null, null, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return (null, null, null, null, null, null);
            }
        }


    }
}
