using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PokemonCollection.Utilities
{
    public static class GeneralUtilities
    {

        public static Label GenerateLabel(string text)
        {
            Label label = new Label()
            {
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
            };

            return label;
        }


        public static void CloseAllCardForms()
        {
            List<Form> formsToClose = new List<Form>();

            FormCollection openForms = Application.OpenForms;

            foreach (var form in openForms.OfType<CardForm>())
            {
                formsToClose.Add(form);
            }

            foreach (Form form in formsToClose)
            {
                form.Close();
            }
        }


    }
}
