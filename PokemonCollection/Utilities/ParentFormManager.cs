using PokemonCollection.Forms.User_Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonCollection.Utilities
{
    public static class ParentFormManager
    {

        private static MainForm mainForm;
        private static Form currentChildForm;

        public static MainForm MainForm { 
            get { return mainForm; }
            set
            {
                if (mainForm == null)
                {
                    mainForm = value;
                }
                else
                {
                    throw new InvalidOperationException("MainForm can only be set once");
                }
            }
        }

        public static Form CurrentChildForm 
        {
            get { return currentChildForm; } 
        }

        // hhhhhhhhhhhh
        public static CollectionForm CurrentCollectionForm
        {
            get
            {
                return currentChildForm as CollectionForm;
            }
        }

        public static void OpenChildForm(Form childForm)
        {
            if (mainForm == null)
            {
                throw new InvalidOperationException("MainForm has not been set");
            }

            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }

            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            mainForm.panel.Controls.Add(childForm);
            mainForm.panel.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        public static void CloseCurrentChildForm()
        {
            if (currentChildForm != null)
            {
                currentChildForm.Close();
                currentChildForm = null;
            }
        }

    }
}
