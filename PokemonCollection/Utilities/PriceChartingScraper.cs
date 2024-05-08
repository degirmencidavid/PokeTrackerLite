using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonCollection.Utilities
{
    // Class to find pricecharting page by getting set name and card name from api

    public class PriceChartingScraper
    {
        // change to const maybe
        private static readonly string searchUrl = "https://www.pricecharting.com/api/products?t=c0b53bce27c1bdab90b1605249e600dc43dfd1d5&q=";
        private const string idUrl = "https://www.pricecharting.com/api/product?t=c0b53bce27c1bdab90b1605249e600dc43dfd1d5&id";
        private const string cardDataUrl = "https://www.pricecharting.com/game/";

        private static Dictionary<string, string> fragmentDictionary = new Dictionary<string, string>();

        public static void InitializeAll()
        {
            InitializeFragmentDictionary();
        }

        private static void InitializeFragmentDictionary()
        {
            fragmentDictionary["ungraded"] = "completed-auctions-used";
            fragmentDictionary["psa8"] = "completed-auctions-new";
            fragmentDictionary["psa9"] = "completed-auctions-graded";
            fragmentDictionary["psa10"] = "completed-auctions-manual-only";
        }

        // Pass this the pokemon name text, set name text, setno text in that order (with spaces in between)
        // Similar to DownloadPriceData, needs to return 2 dictionaries
        public static async Task<List<(Dictionary<string, string>, Dictionary<string, List<(string, string)>>)>> GeneratePriceChartingUrls(string cardName)
        {
            var cardNameParts = cardName.Split('/');
            string searchTerm = cardNameParts[0];

            string priceChartingUrl = searchUrl + searchTerm;

            //MessageBox.Show(priceChartingUrl);

            var cardDicts = await DeserializeAPIJson(priceChartingUrl);

            // change this to find the one that contains the right info
            // also there's no api data for some of the urls, so check
            var firstCardDict = cardDicts.First();

            // for japanese cards
            var mirrorDict = new Dictionary<string, string>();
            var masterDict = new Dictionary<string, string>();
            foreach (var dict in cardDicts)
            {
                // This is wrong
                if (dict["console-name"].ToLower().Contains("japanese"))
                {
                    firstCardDict = dict;
                }
                if (dict["product-name"].ToLower().Contains("reverse"))
                {
                    mirrorDict = dict;
                }
                if (dict["product-name"].ToLower().Contains("master"))
                {
                    masterDict = dict;
                }
            }

            // mirror and master conditions need to be handled
            string cardUrl = $"{cardDataUrl}{firstCardDict["console-name"].ToLower().Replace(" ", "-")}/{firstCardDict["product-name"].Replace("#", "").ToLower().Replace(" ", "-")}";

            string mirrorUrl = GenerateOtherUrl(mirrorDict);
            string masterUrl = GenerateOtherUrl(masterDict);


            //MessageBox.Show(cardUrl);
            //var cardPriceDict = await DownloadPriceData(cardUrl);

            // does this actually work tho?
            var tasks = new List<Func<Task<(Dictionary<string, string>, Dictionary<string, List<(string, string)>>)>>>
            {
                async () => await DownloadPriceData(cardUrl)
            };

            if (mirrorUrl != "")
            {
                tasks.Add(async () => await DownloadPriceData(mirrorUrl));
            }
            if (masterUrl != "")
            {
                tasks.Add(async () => await DownloadPriceData(masterUrl));
            }

            var taskResults =  await Task.WhenAll(tasks.Select(async task => await task()));
            var results = taskResults.ToList();

            return results;
        }

        // if this returns "" then it doesn't have the specified type of card
        private static string GenerateOtherUrl(Dictionary<string, string> otherDict)
        {
            if(otherDict.TryGetValue("console-name", out string cn) && otherDict.TryGetValue("product-name", out string pn))
            {
                return $"{cardDataUrl}{cn.ToLower().Replace(" ", "-")}/{pn.Replace("#", "").ToLower().Replace(" ", "-")}";
            }
            return "";
        }

        // needs to return 2 dictionaries, 1st as is and 2nd one will be grade to list of sales prices & dates
        private static async Task<(Dictionary<string, string>, Dictionary<string, List<(string, string)>>)> DownloadPriceData(string cardUrl)
        {
            if (cardUrl == null)
            {
                return (new Dictionary<string, string>(), new Dictionary<string, List<(string, string)>>());
            }
            try
            {
                var priceData = new Dictionary<string, string>();

                // Get prices for each grade
                var html = await DownloadHtmlAsync(cardUrl);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var prices = ReadPriceChartingPrices(doc);
                priceData["ungraded"] = prices[0];
                priceData["grade8"] = prices[1];
                priceData["grade9"] = prices[2];
                priceData["grade10"] = prices[3];

                //MessageBox.Show(prices[0]);

                //File.WriteAllText("C:\\Users\\Bossk\\Desktop\\htmldump.html", html);


                MessageBox.Show(cardUrl);
                // Get sales history
                var salesHistory = new Dictionary<string, List<(string, string)>>();
                foreach (var key in fragmentDictionary.Keys)
                {
                    var tableNode = doc.DocumentNode.SelectSingleNode($"//div[@class='{fragmentDictionary[key]}']");
                    var saleHistory = ExtractPriceChartingSaleHistory(tableNode);
                    salesHistory[key] = saleHistory;
                }

                return (priceData, salesHistory);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading price data: {ex.Message}");
                return (new Dictionary<string, string>(), new Dictionary<string, List<(string, string)>>());
            }
        }

        private static List<(string, string)> ExtractPriceChartingSaleHistory(HtmlAgilityPack.HtmlNode table)
        {
            var data = new List<(string, string)>();

            var rows = table.SelectNodes(".//tbody/tr");

            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var date = row.SelectSingleNode(".//td[@class='date']")?.InnerText.Trim();
                    var price = row.SelectSingleNode(".//td[@class='numeric']//span[@class='js-price']")?.InnerText.Trim();

                    if (!string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(price))
                    {
                        data.Add((date, price));
                    }
                }
            }
            else
            {
                Console.WriteLine("No price data for selected grade");
            }

            return data;
        }

        private static List<string> ReadPriceChartingPrices(HtmlAgilityPack.HtmlDocument doc)
        {
            string ungraded = doc.DocumentNode.SelectSingleNode("//td[@id='used_price']//span[@class='price js-price']").InnerText.Trim();
            string grade8 = doc.DocumentNode.SelectSingleNode("//td[@id='new_price']//span[@class='price js-price']").InnerText.Trim();
            string grade9 = doc.DocumentNode.SelectSingleNode("//td[@id='graded_price']//span[@class='price js-price']").InnerText.Trim();
            string grade10 = doc.DocumentNode.SelectSingleNode("//td[@id='manual_only_price']//span[@class='price js-price']").InnerText.Trim();

            return new List<string>() { ungraded, grade8, grade9, grade10 };
        }

        private static async Task<List<Dictionary<string, string>>> DeserializeAPIJson(string url)
        {
            string jsonString = await GetJsonFromAPI(url);
            var products = JsonConvert.DeserializeObject<ProductList>(jsonString);

            bool testing = false;
            if (testing) 
            {
                foreach (var product in products.Products)
                {
                    MessageBox.Show(product["console-name"] + product["id"] + product["product-name"]);
                }
            }
            

            return products?.Products ?? new List<Dictionary<string, string>>();
        }

        private static async Task<string> GetJsonFromAPI(string url)
        {
            try
            {
                var html = await DownloadHtmlAsync(url);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // Get JSON as a string
                return html.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting JSON from API call: {ex.Message}");
                return "";
            }
        }

        private static string ExtractJsonFromHtml(string html)
        {
            int startIndex = html.IndexOf("{\"products\":");
            int endIndex = html.IndexOf("</body>");

            if (startIndex != -1 && endIndex != -1)
            {
                string json = html.Substring(startIndex, endIndex - startIndex);
                return json;
            }

            return "";
        }

        private static async Task<string> DownloadHtmlAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }

    public class ProductList
    {
        [JsonProperty("products")]
        public List<Dictionary<string, string>> Products { get; set; }
    }
}
