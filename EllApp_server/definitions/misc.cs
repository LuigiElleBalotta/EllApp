using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using EllApp_server.Classes;

namespace EllApp_server.definitions
{
    class Misc
    {
        static Config_Manager conf = new Config_Manager();
        public static MySqlConnection conn = new MySqlConnection("Server=" + conf.getValue("mysql_host") + ";Database=" + conf.getValue("mysql_db") + ";Uid=" + conf.getValue("mysql_user") + ";Pwd=" + conf.getValue("mysql_password") + ";");
        public static long UnixTimeNow()
        {
            /*var timeSpan = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return (long)timeSpan;*/
            long CurrentTimestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks;
            return CurrentTimestamp;
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }

        public static string GetUsernameByID(int ID)
        {
            if (ID != 0)
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT username FROM accounts WHERE idAccount = @id", conn);
                MySqlParameter idParameter = new MySqlParameter("@id", MySqlDbType.Int32, 0);
                idParameter.Value = ID;
                cmd.Parameters.Add(idParameter);
                MySqlDataReader read = cmd.ExecuteReader();
                string uname = "";
                while (read.Read())
                    uname = read["username"].ToString();
                read.Close();
                conn.Close();
                return uname;
            }
            else
                return "Server Message";
        }

        public static string CreateChatRoomID(int param1, int param2)
        {
            if (param1 < param2)
                return param1.ToString() + "-" + param2.ToString();
            else
                return param2.ToString() + "-" + param1.ToString();
        }
    }

    public enum MessageType
    {
        MSG_TYPE_NULL                   = 0,
        MSG_TYPE_LOGIN_INFO             = 1,
        MSG_TYPE_CHAT_REQUEST_RESPONSE  = 2,
        MSG_TYPE_CHAT                   = 3
    }

    public enum ChatType
    {
        CHAT_TYPE_NULL              = 0,
        CHAT_TYPE_GLOBAL_CHAT       = 1,
        CHAT_TYPE_USER_TO_USER      = 2,
        CHAT_TYPE_GROUP_CHAT        = 3
    }

    public enum CommandType
    {
        Login           = 0,
        Message         = 1,
        ChatsRequest    = 2
    }

    public enum ResponseType
    {
        Connection  = 0,
        Disconnect  = 1,
        Message     = 2,
        NameChange  = 3,
        UserCount   = 4,
        Error       = 255
    }

    public enum FriendshipStatus
    {
        Accepted    = 0,
        Pending     = 1,
        Refused     = 2
    }
}
