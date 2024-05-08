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

namespace PokemonCollection.Forms.User_Forms
{
    public partial class UserMainForm : Form
    {

        private int userID;
        private string sessionToken;

        public UserMainForm(int userID, string sessionToken)
        {
            InitializeComponent();
            this.userID = userID;
            this.sessionToken = sessionToken;
        }

    }
}
