using PokemonCollection.Forms.User_Forms;
using PokemonCollection.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using YourProjectName.DataAccess;

namespace PokemonCollection
{
    public partial class CardForm : Form
    {
        public string cardID;

        private string cardName;
        private string cardLanguage;
        private string cardPrice;
        private string setNumber;
        private string setID;

        private int userID;

        private Dictionary<string, Label> conditionDictionary = new Dictionary<string, Label>();
        private Dictionary<string, string> conditionComboBoxDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> conditionColumnsDictionary = new Dictionary<string, string>();

        public CardForm(string cardID)
        {
            InitializeComponent();          

            this.cardID = cardID;
            
            InitializeDetails();

            InitializeConditionComboBox();
            
            InitializeConditionDictionary();

            InitializeConditionColumnsDictionary();

            InitializeChart();

            // Errors when closing while this is running
            InitializeAsync();
        }

        private async void InitializeDetails()
        {
            try
            {
                string sessionToken = await SessionManager.GetSessionTokenAsync();
                this.userID = await SessionManager.GetUserID(sessionToken);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing user details: {ex.Message}");
            }
        }

        private void InitializeChart()
        {
            chartPriceChartingHistory.ChartAreas[0].AxisX.Title = "Date";
            chartPriceChartingHistory.ChartAreas[0].AxisY.Title = "Price USD";
            // Clear chart
            chartPriceChartingHistory.Series.Clear();
        }

        private void InitializeConditionColumnsDictionary()
        {
            conditionColumnsDictionary["A"] = "OwnedA";
            conditionColumnsDictionary["A-"] = "OwnedAM";
            conditionColumnsDictionary["B"] = "OwnedB";
            conditionColumnsDictionary["C"] = "OwnedC";
            conditionColumnsDictionary["PSA10"] = "OwnedPSA10";
            conditionColumnsDictionary["PSA9"] = "OwnedPSA9";
            conditionColumnsDictionary["PSA8"] = "OwnedPSA8";
        }

        private void InitializeConditionComboBox()
        {
            cbCondition.Items.Add("A");
            conditionComboBoxDictionary["A"] = "ConditionA";
            cbCondition.Items.Add("A-");
            conditionComboBoxDictionary["A-"] = "ConditionAM";
            cbCondition.Items.Add("B");
            conditionComboBoxDictionary["B"] = "ConditionB";
            cbCondition.Items.Add("C");
            conditionComboBoxDictionary["C"] = "ConditionC";
            cbCondition.Items.Add("PSA10");
            conditionComboBoxDictionary["PSA10"] = "ConditionPSA10";
            cbCondition.Items.Add("PSA9");
            conditionComboBoxDictionary["PSA9"] = "ConditionPSA9";
            cbCondition.Items.Add("PSA8");
            conditionComboBoxDictionary["PSA8"] = "ConditionPSA8";
        }

        private void InitializeConditionDictionary()
        {
            conditionDictionary["A"] = ownedA;
            conditionDictionary["A-"] = ownedAM;
            conditionDictionary["B"] = ownedB;
            conditionDictionary["C"] = ownedC;
            conditionDictionary["PSA10"] = ownedPSA10;
            conditionDictionary["PSA9"] = ownedPSA9;
            conditionDictionary["PSA8"] = ownedPSA8;
        }

        private async void InitializeAsync()
        {
            await LoadCardAsync();

            var tasks = new List<Func<Task>>()
            {
                () => UpdateOwnedAsync(),
                () => GetJPPricesAsync(),
                () => GetPriceChartingPricesAsync()
            };
            await tasks.Pool();
        }


        // Under construction
        private async Task GetPriceChartingPricesAsync()
        {
            // must be this order
            string searchString = pokeName.Text + " " + pokeSetName.Text + " " + pokeSetNumber.Text;
            //MessageBox.Show(teststring);
            var scrapedData = await PriceChartingScraper.GeneratePriceChartingUrls(searchString);
            
            // Set price labels
            labelPCUngraded.Text = scrapedData[0].Item1["ungraded"];
            labelPCGrade8.Text = scrapedData[0].Item1["grade8"];
            labelPCGrade9.Text = scrapedData[0].Item1["grade9"];
            labelPCGrade10.Text = scrapedData[0].Item1["grade10"];

            var testData = scrapedData[0].Item2;

            for (int i = 0; i < testData.Keys.Count; i++)
            {
                PlotLineFromData(testData[testData.ElementAt(i).Key], i, testData.ElementAt(i).Key);
            }

            ChartArea chartArea = chartPriceChartingHistory.ChartAreas[0];
            chartArea.AxisX.LabelStyle.Format = "yyyy-MM-dd";
            chartArea.AxisX.IntervalType = DateTimeIntervalType.Days;

            Series series = chartPriceChartingHistory.Series[0];
            series.ChartType = SeriesChartType.Line;
        }

        private void PlotLineFromData(List<(string, string)> data, int i, string condition) 
        {
            ChartArea chartArea = chartPriceChartingHistory.ChartAreas[0];
            chartArea.AxisX.LabelStyle.Format = "yyyy-MM-dd";
            chartArea.AxisX.IntervalType = DateTimeIntervalType.Days;

            chartPriceChartingHistory.Series.Add(condition);
            Series series = chartPriceChartingHistory.Series[i];
            series.ChartType = SeriesChartType.Line;

            foreach (var (date, price) in data)
            {
                DateTime dateTime = DateTime.ParseExact(date, "yyyy-MM-dd", null);

                double numericPrice = double.Parse(price.Replace("$", ""));

                series.Points.AddXY(dateTime, numericPrice);
            }
        }


        private async Task LoadCardAsync()
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
                                cardName = dr["CardName"].ToString().Replace("&", "&&");
                                cardLanguage = dr["CardLanguage"].ToString();
                                cardPrice = dr["CardPrice"].ToString();
                                setNumber = dr["SetNumber"].ToString();
                                setID = dr["SetID"].ToString();

                                pokeName.Text = cardName;
                                pokeLang.Text = cardLanguage;
                                pokePrice.Text = cardPrice;
                                pokeSetNumber.Text = setNumber;
                                pokeSetID.Text = setID;

                                // Set Name
                                pokeSetName.Text = DataAccess.GetSetNameFromID(setID);

                                // Set Image
                                byte[] cardImageData = (byte[])dr["CardImage"];
                                Image cardImage = ImageHandling.ByteArrayToImage(cardImageData);
                                imgCard.Image = cardImage;
                            }
                            else
                            {
                                MessageBox.Show("Error: Card not found");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cards: {ex.Message}");
            }
            
        }

        private async Task GetJPPricesAsync()
        {
            try
            {
                // Set up card dictionary
                var cardDict = SearchTermGenerator.GenerateCardDictionary(cardName, cardLanguage, setNumber, setID);
                string newSearchTerm = await GetCardRushPricesAsync(cardDict);

                var tasks = new List<Func<Task>>()
                {
                    () => GetManaSourcePricesAsync(newSearchTerm),
                    () => GetFuru1PricesAsync(newSearchTerm)

                    // Dorasuta blocks it grr
                    //() => GetDoraSutaPricesAsync(newSearchTerm)
                };
                await tasks.Pool();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting prices: {ex.Message}");
            }
        }

        private async Task GetFuru1PricesAsync(string searchTerm)
        {
            try
            {
                // VMAX, VSTAR sometimes are V MAX and V STAR, but don't want to create double spaces after a regular V 
                string f1Url = SearchTermGenerator.GenerateSearchUrl("furu1", searchTerm.Replace("V", "V ").Replace("  ", " "));
                labelSearchTermF1.Text = f1Url;
                labelSearchTermF1.Click += (s, e) => { Process.Start(f1Url); };

                // Get Manasource listings, there should only be one but cba to change ScrapeListingAsync so just take 0th one
                List<List<string>> furu1Listings = await CardListingScraper.ScrapeListingAsync(f1Url, "furu1");
                Furu1Condition(furu1Listings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting furu1 data: {ex.Message}");
            }
        }

        private void Furu1Condition(List<List<string>> listings)
        {
            foreach (var listing in listings)
            {
                if (listing[4].Contains("BM2"))
                {
                    labelMasterF1.Visible = true;
                    var furu1MirrorMasterLabels = new List<Label>() { labelFuru1MirrorMasterA, labelFuru1MirrorMasterB };
                    Furu1Auxiliary(listing, furu1MirrorMasterLabels);
                }
                else if (listing[4].Contains("BM"))
                {
                    labelReverseF1.Visible = true;
                    var furu1MirrorMasterLabels = new List<Label>() { labelFuru1MirrorA, labelFuru1MirrorB };
                    Furu1Auxiliary(listing, furu1MirrorMasterLabels);
                }
                else
                {
                    var furu1Labels = new List<Label>() { labelFuru1PriceA, labelFuru1PriceB };
                    Furu1Auxiliary(listing, furu1Labels);
                }
            }
        }

        private void Furu1Auxiliary(List<string> listing, List<Label> labels)
        {
            if (listing[0].Contains("rank_a"))
            {
                SetupPriceLabel(labels[0], listing, "A: ");
            }
            else if (listing[0].Contains("rank_b"))
            {
                SetupPriceLabel(labels[1], listing, "B: ");
            }
        }

        //blocked
        private async Task GetDoraSutaPricesAsync(string searchTerm)
        {
            try
            {
                string dsUrl = SearchTermGenerator.GenerateSearchUrl("dorasuta", searchTerm);
                //labelSearchTermDS.Text = dsUrl;

                List<List<string>> dorasutaListings = await CardListingScraper.ScrapeListingAsync(dsUrl, "dorasuta");
                List<string> dorasutaListing = dorasutaListings[0];

                SetupPriceLabel(labelDoraSutaPriceA, dorasutaListing, "A:");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting dorasuta data: {ex.Message}");
            }
        }

        private async Task GetManaSourcePricesAsync(string searchTerm)
        {
            try
            {
                string msUrl = SearchTermGenerator.GenerateSearchUrl("manasource", searchTerm);
                labelSearchTermMS.Text = msUrl;
                labelSearchTermMS.Click += (s, e) => { Process.Start(msUrl); };

                // Get Manasource listings, there should only be one but cba to change ScrapeListingAsync so just take 0th one
                List<List<string>> manasourceListings = await CardListingScraper.ScrapeListingAsync(msUrl, "manasource");
                ManaSourceAuxiliary(manasourceListings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting manasource data: {ex.Message}");
            }
        }

        private void ManaSourceAuxiliary(List<List<string>> listings)
        {
            foreach (var listing in listings)
            {
                if (new[] { " ミラー仕様】", "【ミラー" }.Any(c => listing[0].Contains(c)))
                {
                    SetupPriceLabel(labelManaSourceMirror, listing, "Mirror: ");
                }
                else if (listing[0].Contains("マスターボールミラー"))
                {
                    SetupPriceLabel(labelManaSourceMirrorMaster, listing, "Master Ball: ");
                }
                else
                {
                    SetupPriceLabel(labelManaSourcePrice, listing, "A: ");
                }
            }
        }

        private async Task<string> GetCardRushPricesAsync(Dictionary<string, string> cardDict)
        {
            try
            {
                // Cardrush (automate this with the dict key array in whatever place it is at)
                // Cardrush has set in the title of cards, so disambiguation is easier, can generate a new search term for other sites
                string crUrl = SearchTermGenerator.GenerateSearchUrl("cardrush", cardDict, false);
                labelSearchTermCR.Text = crUrl;
                labelSearchTermCR.Click += (s, e) => { Process.Start(crUrl); };

                //Console.WriteLine(crUrl);

                // Get CardRush listings, grade A will have been pushed to the front of the list
                List<List<string>> cardRushListings = await CardListingScraper.ScrapeListingAsync(crUrl, "cardrush");
                List<string> cardRushGradeA = cardRushListings[0];

                SetupPriceLabel(labelCardRushPriceA, cardRushGradeA, "A: ");
                CardRushCondition(cardRushListings);

                string newSearchTerm = CleanCardRushTitle(cardRushGradeA[0]);
                Console.WriteLine(newSearchTerm);
                return newSearchTerm;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting cardrush data: {ex.Message}");
                return "";
            }
        }

        private void SetupPriceLabel(Label label, List<string> listing, string flavor)
        {
            label.Visible = true;
            label.Text = $"{flavor}{listing[1]}";
            label.ForeColor = listing[2] != "×" ? Color.Blue : Color.Gray;
            label.Click += (s, e) => { Process.Start(listing[3]); };
        }

        private void CardRushCondition(List<List<string>> listings)
        {
            // Populate the rest of the prices
            foreach (List<string> listing in listings)
            {
                if (listing[0].Contains("(ミラー)"))
                {
                    labelMirrorCR.Visible = true;
                    var mirrorLabelList = new List<Label> { labelCardRushMirrorPSA10, labelCardRushMirrorPSA9, labelCardRushMirrorAMinus, labelCardRushMirrorB, labelCardRushMirrorA};
                    CardRushConditionAuxiliary(listing, mirrorLabelList);
                }
                else if (listing[0].Contains("(マスターボールミラー)"))
                {
                    labelMasterCR.Visible = true;
                    var mirrorMasterLabelList = new List<Label> { labelCardRushMirrorMasterPSA10, labelCardRushMirrorMasterPSA9, labelCardRushMirrorMasterAMinus, labelCardRushMirrorMasterB, labelCardRushMirrorMasterA };
                    CardRushConditionAuxiliary(listing, mirrorMasterLabelList);
                }
                else
                {
                    var baseLabelList = new List<Label> { labelCardRushPSA10, labelCardRushPSA9, labelCardRushPriceAMinus, labelCardRushPriceB, labelCardRushPriceA };
                    CardRushConditionAuxiliary(listing, baseLabelList);
                }
                
                if (listing[0].Contains("※状態難"))
                {
                    Console.WriteLine(listing[0]);
                }
            }
        }

        private void CardRushConditionAuxiliary(List<string> listing, List<Label> labels)
        {
            // try catch to deal with labels not being long enough :3
            try
            {
                if (listing[0].Contains("PSA10"))
                {
                    SetupPriceLabel(labels[0], listing, "PSA10: ");
                }
                else if (listing[0].Contains("PSA9"))
                {
                    SetupPriceLabel(labels[1], listing, "PSA9: ");
                }
                else if (listing[0].Contains("状態A-"))
                {
                    SetupPriceLabel(labels[2], listing, "A-: ");
                }
                else if (listing[0].Contains("状態B"))
                {
                    SetupPriceLabel(labels[3], listing, "B: ");
                }
                /* For C and D, but doesn't matter, also add "difficult condition psa"
                else if (cardRushListing[0].Contains("状態C"))
                {
                    SetupPriceLabel(labelCardRushPriceC, listing);
                }
                */
                else if (!listing[0].Contains("状態"))
                {
                    SetupPriceLabel(labels[4], listing, "A: ");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Passed List<Label> doesn't have the correct size {ex.Message}");
            }
            
        }

        private string CleanCardRushTitle(string title)
        {
            string searchTerm = Regex.Replace(title, @"\([^)]*\)|【[^】]*】", "");
            searchTerm = Regex.Replace(searchTerm, @"[^\w\s/]+", " ");
            searchTerm =Regex.Replace(searchTerm, @"\s+", " ").Trim();

            return searchTerm;
        }

        private async Task UpdateOwnedAsync()
        {
            string sessionToken = await SessionManager.GetSessionTokenAsync();
            int userID = await SessionManager.GetUserID(sessionToken);
            this.userID = userID;
            
            // Needs to be changed to get the owned count
            // query db for each one, set the labels

            if (await CheckForCard(userID) == 0)
            {
                foreach (var key in conditionDictionary.Keys)
                {
                    conditionDictionary[key].Text = "0";
                }
                return;
            }

            await UpdateCondition();
        }

        private async Task UpdateCondition()
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.CountCardsInCollection, con))
                    {
                        cm.Parameters.AddWithValue("@userID", userID);
                        cm.Parameters.AddWithValue("@cardID", cardID);

                        using (SqlDataReader dr = await cm.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync())
                            {
                                var conditionA = dr["ConditionA"];
                                var conditionAM = dr["ConditionAM"];
                                var conditionB = dr["ConditionB"];
                                var conditionC = dr["ConditionC"];
                                var conditionPSA10 = dr["ConditionPSA10"];
                                var conditionPSA9 = dr["ConditionPSA9"];
                                var conditionPSA8 = dr["ConditionPSA8"];

                                ownedA.Text = Convert.IsDBNull(conditionA) ? "A: 0" : $"A: {conditionA}";
                                ownedAM.Text = Convert.IsDBNull(conditionAM) ? "A-: 0" : $"A-: {conditionAM}";
                                ownedB.Text = Convert.IsDBNull(conditionB) ? "B: 0" : $"B: {conditionB}";
                                ownedC.Text = Convert.IsDBNull(conditionC) ? "C: 0" : $"C: {conditionC}";
                                ownedPSA10.Text = Convert.IsDBNull(conditionPSA10) ? "PSA 10: 0" : $"PSA 10: {conditionPSA10}";
                                ownedPSA9.Text = Convert.IsDBNull(conditionPSA9) ? "PSA 9: 0" : $"PSA 9: {conditionPSA9}";
                                ownedPSA8.Text = Convert.IsDBNull(conditionPSA8) ? "PSA 8: 0" : $"PSA 8: {conditionPSA8}";
                            }
                            else
                            {
                                MessageBox.Show("Error: Card not found");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cards: {ex.Message}");
            }
        }

        private void UpdateOwnedInUI(int newCount, string condition)
        {
            conditionDictionary[condition].Text = $"{condition}: {newCount.ToString()}";
        }

        private void txtCollAdd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                int added = int.Parse(txtCollAdd.Text);
                string sessionToken = await SessionManager.GetSessionTokenAsync();

                // Something needs fixing here (session token is broken) not anymore maybe hhh
                int userID = await SessionManager.GetUserID(sessionToken);

                bool cardInDB = await CheckForCard(userID) > 0;
                int currentCount = cardInDB ? await CheckForCardCondition(conditionComboBoxDictionary[cbCondition.Text]) : 0;

                string selectedCondition = cbCondition.Text;
                // change this to take the condition - don
                UpdateOwnedInUI(added + currentCount, selectedCondition);
                txtCollAdd.Text = null;

                if (cardInDB)
                {
                    await UpdateCardCountAsync(added, currentCount, userID, selectedCondition);
                    UpdateCardRowInUI(added, selectedCondition);
                    // MessageBox.Show($"{selectedCondition} card count updated");
                    return;
                }

                // else
                await InsertCardCountAsync(added, userID, selectedCondition);
                MessageBox.Show($"{added} {selectedCondition} cards added to collection");                       
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in btnAdd_Click: {ex.Message}");
            }
        }

        private async Task<int> CheckForCardCondition(string condition)
        {
            using (SqlConnection con = DataAccess.GetOpenConnection())
            {
                using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.CountCardConditionInCollection(condition), con))
                {
                    cm.Parameters.AddWithValue("@userID", userID);
                    cm.Parameters.AddWithValue("@cardID", cardID);
                    var count = await cm.ExecuteScalarAsync();
                    //MessageBox.Show(count.GetType().ToString());
                    return Convert.IsDBNull(count) ? 0 : (int)count;
                }
            }
        }

        // change these back to how they were before
        private async Task<int> CheckForCard(int userID)
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.CountCardsInCollection, con))
                    {
                        cm.Parameters.Add(new SqlParameter("@cardID", SqlDbType.VarChar));
                        cm.Parameters["@cardID"].Value = cardID;

                        cm.Parameters.Add(new SqlParameter("@userID", SqlDbType.Int));
                        cm.Parameters["@userID"].Value = userID;

                        using (SqlDataReader reader = await cm.ExecuteReaderAsync()) 
                        {
                            int sum = 0;
                            while (reader.Read())
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    sum += !reader.IsDBNull(i) ? reader.GetInt32(i) : 0;
                                }
                            }
                            return sum;
                        }                       
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Querying DB CHECK: {ex.Message}");
                return 0;
            }
        }

        // change these back to how they were before
        private async Task UpdateCardCountAsync(int added, int current, int userID, string condition)
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.UpdateCardCount(conditionComboBoxDictionary[condition]), con))
                    {
                        cm.Parameters.Add(new SqlParameter("@quantity", SqlDbType.Int) { Value = added + current });

                        cm.Parameters.Add(new SqlParameter("@cardID", SqlDbType.VarChar));
                        cm.Parameters["@cardID"].Value = cardID;

                        cm.Parameters.Add(new SqlParameter("@userID", SqlDbType.Int));
                        cm.Parameters["@userID"].Value = userID;

                        await cm.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Querying DB UPDATE: {ex.Message}");
            }
        }

        public async Task InsertCardCountAsync(int quantity, int userID, string condition)
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.InsertCardCount(conditionComboBoxDictionary[condition]), con))
                    {
                        cm.Parameters.AddWithValue("@userID", userID);
                        cm.Parameters.AddWithValue("@cardID", cardID);
                        cm.Parameters.AddWithValue("@quantity", quantity);

                        await cm.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Inserting Card Count: {ex.Message}");
            }
        }

        private void UpdateCardRowInUI(int added, string condition)
        {
            try
            {
                if (!(ParentFormManager.CurrentCollectionForm is CollectionForm collectionForm))
                {
                    Console.WriteLine("Current child form is not of type CollectionForm or is not set.");
                    return;
                }

                var existingRow = collectionForm.DgvCollection.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row => row.Cells["CardID"].Value?.ToString() == cardID);

                existingRow.Cells[conditionColumnsDictionary[condition]].Value = int.TryParse(existingRow?.Cells[conditionColumnsDictionary[condition]].Value?.ToString(), out var currentQuantity) ? currentQuantity + added : added;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in UpdateCardRowInUI: {ex.Message}");
            }
        }

        private void btnTranslate_Click(object sender, EventArgs e)
        {
            string translatedName = TranslationUtility.Translate(cardName);
            MessageBox.Show($"Translated name: {translatedName}");
        }
    }
}
