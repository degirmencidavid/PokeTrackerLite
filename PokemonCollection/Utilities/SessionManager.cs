using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourProjectName.DataAccess;

namespace PokemonCollection.Utilities
{
    public class SessionManager
    {
        private static bool isSessionStarted = false;

        private static int sessionRefreshInterval = 20;
        private static Timer sessionRefreshTimer;

        private static string sessionToken;
        private static DateTime sessionStartTime;

        // temporary until session is fixed
        public static string sUserID;

        public static bool LoggedIn { get; private set; }

        public static string CreateSessionToken(string userID)
        {
            string tokenData = userID + DateTime.Now.Ticks;
            string sessionToken = HashingUtility.ComputeHash(tokenData);

            SetSessionToken(sessionToken);
            sessionStartTime = DateTime.Now;

            return sessionToken;
        }

        // ughghg
        public static void StartSession(string userID)
        {
            sUserID = userID;
            LoggedIn = true;

            if (!isSessionStarted)
            {
                sessionToken = CreateSessionToken(userID);
                StartSessionRefreshTimer();
                isSessionStarted = true;
            }
        }

        public static void SetSessionToken(string token)
        {
            sessionToken = token;
        }


        // I mean, it works :3
        public static async Task<string> GetSessionTokenAsync()
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.GetSessionToken, con))
                    {
                        cm.Parameters.AddWithValue("@userID", sUserID);

                        object result = await cm.ExecuteScalarAsync();

                        return result != null ? result.ToString() : null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting session token: {ex.Message}");
                return null;
            }
        }

        public static async Task<int> GetUserID(string sessionToken)
        {
            try
            {
                using (SqlConnection con = DataAccess.GetOpenConnection())
                {
                    using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.IDFromSessionToken, con))
                    {
                        cm.Parameters.Add(new SqlParameter("@sessionToken", SqlDbType.NVarChar) { Value = sessionToken });
                        object result = await cm.ExecuteScalarAsync();

                        if (result != null && result != DBNull.Value)
                        {
                            return (int)result;
                        }
                        else
                        {
                            MessageBox.Show($"Session Token: {sessionToken}");
                            MessageBox.Show("Error: User ID not found");
                            return 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error querying DB for UserID: {ex.Message}");
                return 0;
            }
        }


        // Change to only apply after login form
        public static void ClearSession()
        {
            if (!SessionManager.LoggedIn)
            {
                return;
            }

            sessionToken = null;
            sessionStartTime = DateTime.MinValue;
            StopSessionRefreshTimer();

            using (SqlConnection con = DataAccess.GetOpenConnection())
            {
                using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.ClearSessionToken, con))
                {
                    cm.Parameters.AddWithValue("@userID", sUserID);
                    cm.ExecuteNonQuery();
                }
            }
            sUserID = null;
        }

        private static void StartSessionRefreshTimer()
        {
            sessionRefreshTimer = new Timer();
            sessionRefreshTimer.Interval = 1000 * 60 * sessionRefreshInterval;
            sessionRefreshTimer.Tick += SessionRefreshTimer_Tick;
            sessionRefreshTimer.Start();
        }

        private static void StopSessionRefreshTimer()
        {
            sessionRefreshTimer?.Stop();
            sessionRefreshTimer?.Dispose();
        }

        private static void SessionRefreshTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = DateTime.Now - sessionStartTime;

            if (elapsedTime.TotalMinutes >= sessionRefreshInterval)
            {
                UpdateSessionTokenInDB(sUserID);
            }
        }

        public static void UpdateSessionTokenInDB(string userID)
        {
            using (SqlConnection con = DataAccess.GetOpenConnection())
            {
                using (SqlCommand cm = DataAccess.CreateCommand(SQLCommands.UpdateSessionToken, con))
                {
                    string sessionToken = CreateSessionToken(userID.ToString());
                    cm.Parameters.AddWithValue("@sessionToken", sessionToken);
                    cm.Parameters.AddWithValue("@userID", userID);

                    cm.ExecuteNonQuery();
                }
            }
        }

    }
}
