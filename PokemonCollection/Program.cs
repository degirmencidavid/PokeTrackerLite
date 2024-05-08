using PokemonCollection.Forms.Admin_Forms;
using PokemonCollection.Forms.General_Forms;
using PokemonCollection.Forms.User_Forms;
using PokemonCollection.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PokemonCollection
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            EventManagerInit();
            InitializeDataComponents();

            Application.Run(new LoginForm());
        }

        private static void InitializeDataComponents()
        {
            // Just run the initialization methods
            TranslationUtility.InitializeAll();
            SearchTermGenerator.InitializeAll();
            CardListingScraper.InitializeAll();
            PriceChartingScraper.InitializeAll();
        }

        private static void EventManagerInit()
        {
            Application.ApplicationExit += OnApplicationExit;
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            PokemonCollection.Utilities.SessionManager.ClearSession();
        }
    }
}