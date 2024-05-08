using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonCollection.Utilities
{
    public static class SQLCommands
    {
        // USER MANAGEMENT //

        public const string ReadUserRole = "SELECT UserRole FROM tbUser WHERE username = @username";

        public const string IDFromSessionToken = "SELECT UserID FROM tbUser WHERE SessionToken = @sessionToken";

        public const string IDFromUsername = "Select UserID FROM tbUser WHERE username = @username";

        public const string ReadUserAuth = "SELECT * FROM tbUser WHERE username = @username AND password = @password";
        
        public const string UpdateSessionToken = "UPDATE tbUser SET SessionToken = @sessionToken WHERE UserID = @userID";
        
        public const string InsertUser = "INSERT INTO tbUser(username, fullname, password, phone)"
                                       + "VALUES(@username, @fullname, @password, @phone)";

        public const string UpdateUser = "UPDATE tbUSER SET fullname = @fullname, password = @password, " +
                                         "phone = @phone WHERE username LIKE @unValue";

        public const string ReadAllUsers = "SELECT * FROM tbUser";

        public const string ClearSessionToken = "UPDATE tbUser SET SessionToken = NULL WHERE UserID = @userID";

        public static string ReadNUsers (int N)
        {
            return "SELECT " + N + "FROM tbUser";
        }

        public static string DeleteUser(string property)
        {
            return "DELETE FROM tbUser WHERE " + property + " LIKE @propertyValue";
        }

        // Count how many of each property exists
        public static string CountProperties(string property)
        {
            return "SELECT COUNT(*) FROM tbUser WHERE " + property + " = @value";
        }

        // END OF USER MANAGEMENT //


        // SET MANAGEMENT //

        public const string InsertCardSet = "INSERT INTO tbCardSets(SetID, SetName, TotalCards, Released, SetImage) " +
                                            "VALUES (@setID, @setName, @totalCards, @released, @setImage)";

        public const string ReadAllCardSets = "SELECT * FROM tbCardSets";

        public const string ReadSetBySetID = "SELECT SetName FROM tbCardSets WHERE SetID = @setID";

        public const string ReadSetIDs = "SELECT SetID FROM tbCardSets";


        // END OF SET MANAGEMENT //


        // CARD MANAGMENT //

        public const string InsertCard = "INSERT INTO tbCards(SetID, CardName, SetNumber, CardLanguage, CardImage)" +
                                         " VALUES (@setID, @cardName, @setNumber, @language, @cardImage)";

        public const string ReadAllCards = "SELECT * FROM tbCards";

        public const string ReadCardsBySetID = "SELECT * FROM tbCards WHERE SetID = @setID";

        public const string ReadCardsByCardID = "SELECT * FROM tbCards WHERE CardID = @cardID";


        // Awful way to do this but whatever

        public const string CardsByNameComponent = "\nCardName LIKE '%' + @sCardName + '%'";
        public const string CardsByLanguageComponent = "\nCardLanguage = @sCardLanguage";
        public const string CardsBySetNumberComponent = "\nSetNumber = @sSetNumber";
        public const string CardsBySetIDComponent = "\nSetID = @sSetID";
        public const string CardsBySetNameComponent = "\bSetName = @sSetName";

        public static string[] componentArray = { CardsByNameComponent, CardsBySetNumberComponent, CardsBySetIDComponent };

        // Yes jank
        public static string CardSearchQuery(bool[] explicitS)
        {
            string baseQuery = "SELECT * FROM tbCards WHERE";

            return "SELECT * FROM tbCards WHERE " + string.Join(" AND ", componentArray.SelectMany((component, i) => explicitS[i] ? new string[] { component } : new string[0]));
            for (int i = 0; i < explicitS.Length; i++)
            {
                if (explicitS[i])
                {
                    baseQuery += componentArray[i] + "\nAND";
                }
            }
            baseQuery += CardsByLanguageComponent;
            return baseQuery;
        }

        // END OF CARD MANAGEMENT //


        // COLLECTION MANAGMENT //

        public const string ReadUserCollectionByID = "SELECT * FROM tbUserCollection WHERE UserID = @userID";

        public const string CountCardsInCollectionO = "SELECT Quantity FROM tbUserCollection WHERE UserID = @userID AND CardID = @cardID";

        public const string UpdateCardCountO = "UPDATE tbUserCollection SET Quantity = @quantity WHERE UserID = @userID AND CardID = @cardID";

        public const string InsertCardCountO = "INSERT INTO tbUserCollection(UserID, CardID, Quantity) VALUES (@userID, @cardID, @quantity)";

        // updated
        public const string CountCardsInCollection = "SELECT ConditionA, ConditionAM, ConditionB, ConditionC, ConditionPSA10, ConditionPSA9, ConditionPSA8 FROM tbUserCollection WHERE UserID = @userID AND CardID = @cardID";

        public static string UpdateCardCount(string columnName)
        {
            return $"UPDATE tbUserCollection SET {columnName} = @quantity WHERE UserID = @userID AND CardID = @cardID";
        }

        public static string InsertCardCount(string columnName)
        {
            return $"INSERT INTO tbUserCollection(UserID, CardID, {columnName}) VALUES (@userID, @cardID, @quantity)";
        }

        public static string CountCardConditionInCollection(string condition)
        {
            return $"SELECT {condition} FROM tbUserCollection WHERE UserID = @userID AND CardID = @cardID";
        }

    // END OF COLLECTION MANAGEMENT //


    // JANK SESSION MANAGEMENT //

    public const string GetSessionToken = "SELECT SessionToken FROM tbUser WHERE UserID = @userID";

        // END OF JANK SESSION MANAGEMENT //
    }
}
