using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
