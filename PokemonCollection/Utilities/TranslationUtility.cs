using CsvHelper;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonCollection.Utilities
{
    public class TranslationUtility
    {
        public static readonly string[] SupportedLanguages = { "English", "Japanese" };

        private static string[] cardTypes;
        private static readonly string translationFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug\\", "Resources\\translations.csv"));

        private static Dictionary<string, string> cardType = new Dictionary<string, string>();
        private static Dictionary<string, string> nameTranslation = new Dictionary<string, string>();

        public TranslationUtility()
        {
            InitializeAllAsyncTasks();
        }

        public static void InitializeAll()
        {
            InitializeAllAsyncTasks();
        }

        private static async void InitializeAllAsyncTasks()
        {
            InitializeCardTypeDictionary();
            await InitializeNameDictionaryAsync();
        }

        private static void InitializeCardTypeDictionary()
        {
            // When using this, if there is no key, it is because the type is named the same in both languages
            // Need to add character's pokemon and other titles etc, put in a file and do the same as other method
            cardType["Radiant"] = "かがやく";
            cardType["かがやく"] = "Radiant";

            cardType["Shining"] = "ひかる";
            cardType["ひかる"] = "Shining";

            cardType["Ultra"] = "ウルトラ";
            cardType["ウルトラ"] = "Ultra";

            cardType["Galarian"] = "ガラル";
            cardType["ガラル"] = "Galarian";

            cardType["Paldean"] = "パルデア";
            cardType["パルデア"] = "Paldean";

            cardType["Hisuian"] = "ヒスイ";
            cardType["ヒスイ"] = "Hisuian";

            // Populate cardTypes with all of these
            cardTypes = cardType.Keys.ToArray();
        }

        // Also in the wrong class but I want the dicts to be private
        // Basically for manasource searches, stuff like galarian moltres will not come up without the space, but best for all sites
        public static string AddSpaceToKey(string searchTerm)
        {
            List<string> cardTypes = cardType.Keys.ToList();
            foreach (var type in cardTypes)
            {
                if (searchTerm.Contains(type))
                {
                    return searchTerm.Replace(type, type + " ");
                }
            }
            return searchTerm;
        }

        private static string currentKey;
        private static async Task InitializeNameDictionaryAsync()
        {
            try
            {
                using (var reader = new StreamReader(translationFilePath))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        while (await csv.ReadAsync())
                        {
                            string key = CapitalizeWords(csv.GetField<string>(0));
                            string value = csv.GetField<string>(1);

                            nameTranslation[key] = value;
                            nameTranslation[value] = key;

                            currentKey = key;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error reading CSV file: {ex.Message} \n on: {currentKey}");
            }
        }

        private static string CapitalizeWords(string input)
        {
            return string.Join(" ", input.Split(' ').Select((word) => char.ToUpper(word[0]) + word.Substring(1)));

            /*
            string[] words = input.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                string currentWord = words[i];
                currentWord = char.ToUpper(currentWord[0]) + currentWord.Substring(1);
                words[i] = currentWord;
            }

            return string.Join(" ", words);
            */
        }

        public static string Translate(string name)
        {

            /*
            return string.Join(" ", name.Split(' ').Select((part) =>
            {
                if (cardType.ContainsKey(part))
                {
                    return cardType[part];
                }
                else if (nameTranslation.ContainsKey(part))
                {
                    return nameTranslation[part];
                }
                else
                {
                    return part;
                }
            }));
            */

            var parts = name.Split(' ');     

            List<string> translated = new List<string>();

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];

                if (cardType.ContainsKey(part))
                {
                    translated.Add(cardType[part]);
                }
                else if (nameTranslation.ContainsKey(part))
                {
                    translated.Add(nameTranslation[part]);
                }
                else if (i + 1 < parts.Length)
                {
                    string combined = $"{part} {parts[i + 1]}";
                    if(nameTranslation.ContainsKey(combined))
                    {
                        translated.Add(nameTranslation[combined]);
                    }
                }
                else
                {
                    translated.Add(part);
                }
            }
            return string.Join(" ", translated);

        }


        // Non Async Initialization (redundant) //

        private static void InitializeAllNonAsync()
        {
            InitializeCardTypeDictionary();
            InitializeNameDictionary();
        }

        private static void InitializeNameDictionary()
        {
            try
            {
                using (var reader = new StreamReader(translationFilePath))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        while (csv.Read())
                        {
                            string key = csv.GetField<string>(0);
                            string value = csv.GetField<string>(1);

                            nameTranslation[key] = value;
                            nameTranslation[value] = key;

                            currentKey = key;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error reading CSV file: {ex.Message} \n on: {currentKey}");
            }
        }
    }
}
