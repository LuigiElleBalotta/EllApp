using MySql.Data.MySqlClient;
using System.Configuration;

namespace EllApp_server.Classes
{
    public class DB
    {
        public static MySqlConnection EllAppDB = null;
        public static void Instantiate()
        {
            EllAppDB = new MySqlConnection(GetConnectionString());
            EllAppDB.Open();
        }

        public static string GetConnectionString()
        {
            return $"server={ConfigurationManager.AppSettings["mysql_host"]};uid={ConfigurationManager.AppSettings["mysql_user"]};pwd={ConfigurationManager.AppSettings["mysql_password"]};database={ConfigurationManager.AppSettings["mysql_db"]};";
        }
    }
}
