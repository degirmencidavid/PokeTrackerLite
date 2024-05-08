using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;
using HtmlAgilityPack;
using PokemonCollection.Forms.Admin_Forms;

namespace PokemonCollection.Utilities
{
    public class CardScraper
    {

        private const string CardClass = "card ";
        private const string PlaqueClass = "plaque";
        private const string SetClass = "icon set";
        private const string SymbolClass = "icon symbol";

        public async Task<(List<string> SetInfo, List<List<string>> CardDataList)> ScrapeCardsAsync(string url)
        {
            List<List<string>> cardDataList = new List<List<string>>();
            List<string> setInfo = new List<string>();

            try
            {
                var html = await DownloadHtmlAsync(url);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // Set data (Name, Image, SetID) //
                var setNode = doc.DocumentNode.SelectSingleNode($"//h1[contains(@class, '{SetClass}')]");
                var setData = GetSetData(setNode);

                var codeNode = doc.DocumentNode.SelectSingleNode($"//h1[contains(@class, '{SymbolClass}')]");
                string codeUrl = GetCodeUrl(codeNode);

                var extractedText = await ImageHandling.ExtractTextFromImageUrlAsync(codeUrl);

                // Language
                string language = "English";

                if (url.Contains("jp.p"))
                {
                    language = "Japanese";
                }

                // Release date
                var releaseNode = doc.DocumentNode.SelectSingleNode("//div[contains(span, 'Released')]");
                string releaseDate = GetReleaseDate(releaseNode);

                // Set limit (i.e. /nnn)
                var setLimitNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'cards')]/span[2]");
                string setLimit = setLimitNode.InnerText ?? "N/A";

                // if promo then change setnumber to be the set id (e.g. SM-P)
                if (PokellectorSetScraper.setID.Contains("-P"))
                {
                    setLimit = PokellectorSetScraper.setID;
                }

                // Put into setInfo
                setInfo.Add(setData.SetName);
                setInfo.Add(setData.ImageUrl);
                setInfo.Add(extractedText);
                setInfo.Add(language);
                setInfo.Add(releaseDate);

                //          //          //           //

                // Card data
                var cardNodes = doc.DocumentNode.SelectNodes($"//div[contains(@class, '{CardClass}')]");

                if (cardNodes != null)
                {
                    foreach (var cardNode in cardNodes)
                    {
                        var imageUrl = GetImageData(cardNode);
                        var plaqueData = GetPlaqueData(cardNode);

                        List<string> cardData = new List<string>
                        {
                            imageUrl,
                            $"{plaqueData.Number}/{setLimit}",
                            plaqueData.Name
                        };
                        cardDataList.Add(cardData);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return (setInfo, cardDataList);
        }

        // put this in some other utility class and make it static because I use it multiple times
        private async Task<string> DownloadHtmlAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
        }

        private (string Number, string Name) GetPlaqueData(HtmlNode cardNode)
        {
            var plaqueNode = cardNode.SelectSingleNode($".//div[contains(@class, '{PlaqueClass}')]");

            if (plaqueNode == null)
            {
                Console.WriteLine("Plaque node not found!");
                return (string.Empty, string.Empty);
            }

            var plaqueText = plaqueNode?.InnerText.Trim() ?? string.Empty;
            Console.WriteLine("Plaque Text: " + plaqueText);

            // #number - name
            var parts = plaqueText.Split(new[] { '-' }, 2);
            var number = parts.Length > 0 ? parts[0].TrimStart('#') : string.Empty;
            var name = parts.Length > 1 ? parts[1].Trim() : string.Empty;

            return (number.Trim(), name.Trim());
        }

        private string GetImageData(HtmlNode cardNode)
        {
            var imgNodes = cardNode.Descendants("img");

            foreach (var imgNode in imgNodes)
            {
                string srcUrl = imgNode.GetAttributeValue("data-src", string.Empty);

                if (srcUrl.StartsWith("https://den-cards.pokellector.com/"))
                {
                    return srcUrl.Replace(".thumb", "");
                }
            }

            return "nah g";
        }

        private (string SetName, string ImageUrl) GetSetData(HtmlNode setNode)
        {
            if (setNode != null)
            {
                var setName = setNode.InnerText.Trim();
                var imgNode = setNode.SelectSingleNode(".//img");
                var imgUrl = imgNode?.GetAttributeValue("src", string.Empty);

                return (setName, imgUrl);
            }
            return ("name", "you are L");
        }

        // Generalise this to use in the GetSetData method
        private string GetCodeUrl(HtmlNode codeNode)
        {
            if (codeNode != null)
            {
                var imgNode = codeNode.SelectSingleNode(".//img");
                string imgUrl = imgNode?.GetAttributeValue("src", string.Empty);
                return imgUrl;
            }
            return "code :(";
        }

        private string GetReleaseDate(HtmlNode releaseNode)
        {
            if (releaseNode != null) 
            {
                var spanNodes = releaseNode.SelectNodes(".//span");
                var citeNode = releaseNode.SelectSingleNode(".//cite");

                if (spanNodes != null && spanNodes.Count == 2 &&  citeNode != null)
                {
                    var monthDay = spanNodes[1].InnerText.Trim();

                    // find a better way
                    monthDay = RemoveDateSuffix(monthDay);

                    var year = citeNode.InnerText.Trim();

                    string date = $"{monthDay} {year}";

                    DateTime parsedDate = DateTime.ParseExact(date, "MMM d yyyy", CultureInfo.InvariantCulture);

                    return parsedDate.ToString("dd/MM/yy");
                }

            }

            return "oopsie";
        }

        public static string RemoveDateSuffix(string date)
        {
            string pattern = @"(?<=[0-9])(?:st|nd|rd|th)\b";

            return Regex.Replace(date, pattern, "");
        }

    }
}
