using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;


namespace PokemonCollection.Utilities
{
    public class CardListingScraper
    {
        private static Dictionary<string, Func<string, HtmlAgilityPack.HtmlDocument, List<List<string>>>> cardShopMethods = new Dictionary<string, Func<string, HtmlAgilityPack.HtmlDocument, List<List<string>>>>();
        private static string[] cardshopArray;

        public static void InitializeAll()
        {
            InitializeCSMDictionary();
        }

        private static void InitializeCSMDictionary()
        {
            cardShopMethods["cardrush"] = GetCardRushListings;
            cardShopMethods["manasource"] = GetManaSourceListings;
            cardShopMethods["dorasuta"] = GetDoraSutaListings;
            cardShopMethods["furu1"] = GetFuru1Listings;

            cardshopArray = cardShopMethods.Keys.ToArray();
        }

        public static async Task<List<List<string>>> ScrapeListingAsync(string url, string cardshop)
        {
            List<List<string>> listingInfo = new List<List<string>>();
            
            try
            {
                var html = await DownloadHtmlAsync(url);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                //File.WriteAllText("C:\\Users\\Bossk\\Desktop\\htmldump.html", html);

                if (cardShopMethods.TryGetValue(cardshop, out var method))
                {
                    return method(url, doc);
                }
                else
                {
                    throw new KeyNotFoundException($"{cardshop} not found");
                }

            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Error getting listing data {ex.Message}");
            }

            return listingInfo;
        }

        // for these getstorelistings, change the default return to a proper list list with default data
        private static List<List<string>> GetManaSourceListings(string url, HtmlAgilityPack.HtmlDocument doc)
        {
            try
            {
                List<List<string>> allListingInfo = new List<List<string>>();

                string listingClass = "item_data flex_layout";
                var listingNodes = doc.DocumentNode.SelectNodes($".//div[contains(@class, '{listingClass}')]");

                if (listingNodes == null || listingNodes.Count == 0)
                {
                    return new List<List<string>>();
                }

                string nameClass = "item_name";
                string priceClass = "figure";
                string stockClass = "stock";
                string linkClass = "item_data_link";

                foreach (var listingNode in listingNodes)
                {
                    var nameNode = listingNode.SelectSingleNode($".//p[contains(@class, '{nameClass}')]");
                    string name = nameNode.InnerText.Trim();
                    var priceNode = listingNode.SelectSingleNode($".//span[contains(@class, '{priceClass}')]");
                    string price = priceNode != null ? priceNode.InnerText : "No Price";
                    var stockNode = listingNode.SelectSingleNode($".//p[contains(@class, '{stockClass}')]");
                    string stock = stockNode.InnerText.Any(char.IsDigit) ? string.Join("", stockNode.InnerText.Where(char.IsDigit).ToArray()) : "×";
                    var linkNode = listingNode.SelectSingleNode($".//a[contains(@class, '{linkClass}')]");
                    string link = linkNode.GetAttributeValue("href", "") ?? "No link found";

                    allListingInfo.Add(new List<string> { name, price, stock, link });
                }

                return allListingInfo;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting ms listing(s): {ex.Message}");
                return new List<List<string>>();
            }
        }
        
        private static List<List<string>> GetFuru1Listings(string url, HtmlAgilityPack.HtmlDocument doc)
        {
            try
            {
                List<List<string>> allListingInfo = new List<List<string>>();

                string listingClass = "o-slideitem__box";
                var listingNodes = doc.DocumentNode.SelectNodes($".//div[contains(@class, '{listingClass}')]");

                if (listingNodes == null || listingNodes.Count == 0)
                {
                    return new List<List<string>>();
                }

                string nameClass = "o-slideitem__name";
                string priceClass = "o-slideitem__price";
                string stockClass = "o-slideitem__cart cart_in_product";
                string linkClass = "o-slideitem__img";
                string conditionClass = "o-slideitem__rank";

                foreach (var listingNode in listingNodes)
                {
                    var nameNode = listingNode.SelectSingleNode($".//a[contains(@class, '{nameClass}')]");
                    string name  = nameNode.InnerText.Trim();
                    var priceNode = listingNode.SelectSingleNode($".//span[contains(@class, '{priceClass}')]");
                    string price = priceNode != null ? priceNode.InnerText : "No Price";
                    price = Regex.Replace(price, @"\([^)]*\)|【[^】]*】", "");
                    var stockNode = listingNode.SelectSingleNode($".//a[contains(@class, '{stockClass}')]");
                    string stock = stockNode != null ? "1" : "×";
                    var linkNode = listingNode.SelectSingleNode($".//a[contains(@class, '{linkClass}')]");
                    string link = linkNode.GetAttributeValue("href", "") ?? "No link found";
                    var conditionNode = listingNode.SelectSingleNode($".//figure[contains(@class, '{conditionClass}')]/img");
                    string conditionLink = conditionNode.GetAttributeValue("src", "");

                    allListingInfo.Add(new List<string> { conditionLink, price, stock, link, name });
                }

                allListingInfo = allListingInfo.OrderBy(item => item.FirstOrDefault()).ToList();

                return allListingInfo;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error getting f1 listing(s): {ex.Message}");
                return new List<List<string>>();
            }
        }

        // Dorasuta blocks it gurr, do some other method
        private static List<List<string>> GetDoraSutaListings(string url, HtmlAgilityPack.HtmlDocument doc)
        {
            try
            {
                List<List<string>> allListingInfo = new List<List<string>>();

                string listingClass = "element";
                var listingNodes = doc.DocumentNode.SelectNodes($".//div[contains(@class, '{listingClass}')]");

                if (listingNodes == null || listingNodes.Count == 0)
                {
                    MessageBox.Show("amoguys");
                    return new List<List<string>>();
                }
                return new List<List<string>>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting ds listing(s): {ex.Message}");
                return new List<List<string>>();
            }
        }

        // The first element in this list should be the important data (A grade), the rest are other cards, can do whatever with later
        private static List<List<string>> GetCardRushListings(string url, HtmlAgilityPack.HtmlDocument doc)
        {
            try
            {
                List<List<string>> allListingInfo = new List<List<string>>();

                string listingClass = "item_data";
                var listingNodes = doc.DocumentNode.SelectNodes($".//div[contains(@class, '{listingClass}')]");
                
                if (listingNodes == null || listingNodes.Count == 0)
                {
                    return new List<List<string>>();
                }

                if (listingNodes != null)
                {
                    string nameClass = "goods_name";
                    string priceClass = "figure";
                    string stockClass = "stock";
                    string linkClass = "item_data_link";
                    foreach (var listingNode in listingNodes)
                    {
                        var nameNode = listingNode.SelectSingleNode($".//span[contains(@class, '{nameClass}')]");
                        string name = nameNode.InnerText ?? "No card name found";
                        var priceNode = listingNode.SelectSingleNode($".//span[contains(@class, '{priceClass}')]");
                        string price = priceNode.InnerText ?? "No price found";
                        var stockNode = listingNode.SelectSingleNode($".//p[contains(@class, '{stockClass}')]");
                        string stock = stockNode.InnerText ?? "×";
                        var linkNode = listingNode.SelectSingleNode($".//a[contains(@class, '{linkClass}')]");
                        string link = linkNode.GetAttributeValue("href", "") ?? "No link found";

                        //MessageBox.Show(link);

                        if (!SearchTermGenerator.exclusionDictionary["cardrush"].Any(name.Contains))
                        {
                            allListingInfo.Insert(0, new List<string> { name, price, stock, link });
                        }
                        else
                        {
                            allListingInfo.Add(new List<string> { name, price, stock, link });
                        }
                    }
                }

                // just the first one, this will be changed in future
                return allListingInfo;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting cr listing(s): {ex.Message}");
                return new List<List<string>>();
            }

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
}
