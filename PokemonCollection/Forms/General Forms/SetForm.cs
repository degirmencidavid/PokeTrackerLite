using PokemonCollection.Utilities;
using PokemonCollection.Forms.General_Forms;
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
using System.Threading;

namespace PokemonCollection.Forms.General_Forms
{
    public partial class SetForm : Form
    {
        public SetForm()
        {
            InitializeComponent();
            dgvSets.CellDoubleClick += DgvSets_CellDoubleClick;
        }
        private async Task LoadSetsAsync()
        {
            dgvSets.Rows.Clear();
            // hhhhhhhhhh
            dgvSets.Columns["SetImage"].DefaultCellStyle.NullValue = null; // Set default null value
            ((DataGridViewImageColumn)dgvSets.Columns["SetImage"]).ImageLayout = DataGridViewImageCellLayout.Zoom;

            using (SqlConnection con = DataAccess.GetOpenConnection())
            {
                using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.ReadAllCardSets, con))
                {
                    using (SqlDataReader dr = await cm.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            string setID = dr[0].ToString();
                            string setName = dr[1].ToString();
                            string totalCards = dr[2].ToString();
                            string releaseDate = dr[3].ToString();
                            byte[] setImageData = (byte[])dr[4];

                            Image setImage = ImageHandling.ByteArrayToImage(setImageData);


                            // problem when clicking before all sets have loaded, but only if going from home tab to setlist
                            dgvSets.Invoke(new Action(() => dgvSets.Rows.Add(setID, setName, totalCards, releaseDate, setImage)));
                        }
                    }
                }
            }
        }

        private async void SetForm_Load(object sender, EventArgs e)
        {
            await LoadSetsAsync();
        }

        private void DgvSets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvSets.Rows[e.RowIndex];

                string setID = selectedRow.Cells["SetID"].Value.ToString();
                int totalCards = int.Parse(selectedRow.Cells["TotalCards"].Value.ToString());
                Image setImage = (Image)selectedRow.Cells["SetImage"].Value;

                ParentFormManager.OpenChildForm(new SetListForm(setID, totalCards, setImage));
            }
        }
    }
}
