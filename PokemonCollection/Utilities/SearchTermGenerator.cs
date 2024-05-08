using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonCollection.Utilities
{
    public class SearchTermGenerator
    {
        public static Dictionary<string, string[]> exclusionDictionary = new Dictionary<string, string[]>();
        private static Dictionary<string, string> siteUrlDictionary = new Dictionary<string, string>();

        public static void InitializeAll()
        {
            InitializeExclusionDictionary();
            InitializeSiteUrlDictionary();
        }

        private static void InitializeExclusionDictionary()
        {
            exclusionDictionary["dorasuta"] = new string[]{ "プレイ用" };
            exclusionDictionary["cardrush"] = new string[] { "状態", "PSA", "ミラー", "BGS", "ACE", "CGC", "ARS", "SALE", "封筒付き", "未開封" };
            exclusionDictionary["manasource"] = new string[] { "ミラー仕様" };

            // For reference, this needs to be changed, add other sites etc
            // exclusionDictionary["furu1"] = "B";
        }

        private static void InitializeSiteUrlDictionary()
        {
            siteUrlDictionary["cardrush"] = "https://www.cardrush-pokemon.jp/product-list?keyword=";
            siteUrlDictionary["dorasuta"] = "https://dorasuta.jp/pokemon-card/product-list?kw=";
            siteUrlDictionary["manasource"] = "https://www.manasource.net/product-list?keyword=";
            siteUrlDictionary["furu1"] = "https://www.furu1.online/search?category=&kw=";
        }

        public static Dictionary<string, string> GenerateCardDictionary(string name, string language, string setNo, string setID)
        {
            var dict = new Dictionary<string, string>();
            dict["Name"] = name;
            dict["Language"] = language;
            dict["SetNo"] = setNo;
            // Incase of promo set, don't want to dupe the setID (189/S-P S-P will be problematic)
            dict["SetID"] = setID;
            return dict;
        }

        private static string GenerateSearchTermEnglish(string input)
        {
            // add stuff here
            return input;
        }

        private static string GenerateSearchTermJapanese(Dictionary<string, string> cardProperties)
        {
            string translatedSt = TranslationUtility.Translate(cardProperties["Name"]);

            // make this less crap
            if (translatedSt == cardProperties["Name"])
            {
                translatedSt = "";
            }

            string serializedNo = cardProperties["SetNo"];

            string[] numberParts = serializedNo.Split('/');
            numberParts[0] = int.Parse(numberParts[0]).ToString("D3");
            if (!serializedNo.Contains("-P"))
            {
                numberParts[1] = int.Parse(numberParts[1]).ToString("D3");
            }
            serializedNo = string.Join(" ", numberParts);

            translatedSt = $"{translatedSt} {serializedNo}".Trim();

            //MessageBox.Show(translatedSt);

            return translatedSt;
        }

        private static string GenerateSearchTerm(Dictionary<string, string> cardProperties, bool useSetID)
        {
            if (cardProperties["Language"] == "Japanese")
            {
                // Absolutely ridiculously strict cardrush search, trim probably isnt needed but whatever
                string result = Regex.Replace(GenerateSearchTermJapanese(cardProperties), @"&", "").Replace("  ", " ").Trim();
                return useSetID ? $"{cardProperties["SetID"]} {result}".Trim() : result.Trim();
            }
            else
            {
                return GenerateSearchTermEnglish($"{cardProperties["Name"]} {cardProperties["SetNo"]}");
            }
        }

        public static string GenerateSearchUrl(string cardshop, Dictionary<string, string> cardProperties, bool useSetID)
        {
            if (cardProperties["Language"] == "Japanese")
            {
                return siteUrlDictionary[cardshop] + GenerateSearchTerm(cardProperties, useSetID);
            }
            else
            {
                // update for english cards later
                return siteUrlDictionary[cardshop] + GenerateSearchTerm(cardProperties, false);
            }
        }

        // Only base url + search term matters here
        public static string GenerateSearchUrl(string cardshop, string searchTerm)
        {
            searchTerm = TranslationUtility.AddSpaceToKey(searchTerm);
            return siteUrlDictionary[cardshop] + searchTerm;
        }

    }
}
