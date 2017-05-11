using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using EllApp_server.definitions;

namespace EllApp_server.Classes
{
    public class User
    {
        int ID = 0;
        string username, last_ip, email;
        static Config_Manager conf = new Config_Manager();
        static MySqlConnection staticconn = new MySqlConnection("Server=" + conf.getValue("mysql_host") + ";Database=" + conf.getValue("mysql_db") + ";Uid=" + conf.getValue("mysql_user") + ";Pwd=" + conf.getValue("mysql_password") + ";");
        public User(string _username, string _password)
        {
            username = _username;
            byte[] passwordbyte = Encoding.ASCII.GetBytes(username + ":" + _password);
            var sha_pass = SHA1.Create();
            byte[] bytehash = sha_pass.ComputeHash(passwordbyte);
            _password = Utility.HexStringFromBytes(bytehash);

            
            MySqlCommand cmd = new MySqlCommand("SELECT idAccount, email, last_ip FROM accounts WHERE username = @username AND password = @password;", DB.EllAppDB);
            MySqlParameter passwordParameter = new MySqlParameter("@password", MySqlDbType.VarChar, 0);
            MySqlParameter usernameParameter = new MySqlParameter("@username", MySqlDbType.VarChar, 0);
            passwordParameter.Value = _password.ToUpper();
            usernameParameter.Value = username.ToUpper();
            cmd.Parameters.Add(usernameParameter);
            cmd.Parameters.Add(passwordParameter);
            MySqlDataReader row = cmd.ExecuteReader();
            while(row.Read())
            {
                ID = Convert.ToInt32(row["idAccount"]);
                last_ip = row["last_ip"].ToString();
                email = row["email"].ToString();
            }
            row.Close();
        }

        public bool Validate()
        {
            if (ID > 0)
                return true;
            else
                return false;
        }

        public string GetUsername()
        {
            return username;
        }

        public int GetID()
        {
            return ID;
        }

        public bool IsOnline()
        {
            bool online = false;
            MySqlCommand cmd = new MySqlCommand("SELECT isOnline FROM accounts WHERE idAccount = @id;", DB.EllAppDB);
            MySqlParameter idParameter = new MySqlParameter("@id", MySqlDbType.Int32, 0);
            idParameter.Value = ID;
            cmd.Parameters.Add(idParameter);
            MySqlDataReader r = cmd.ExecuteReader();
            while(r.Read())
            {
                Console.WriteLine(Convert.ToInt32(r["isOnline"]));
                if (Convert.ToInt32(r["isOnline"]) == 1)
                    online = true;
            }
            return online;
        }

        public void SetOnline()
        {
            MySqlCommand cmd = new MySqlCommand("UPDATE accounts SET isOnline = 1 WHERE idAccount = @id;", DB.EllAppDB);
            MySqlParameter idParameter = new MySqlParameter("@id", MySqlDbType.Int32, 0);
            idParameter.Value = ID;
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
        }

        public void SetOffline()
        {
            MySqlCommand cmd = new MySqlCommand("UPDATE accounts SET isOnline = 0 WHERE idAccount = @id;", DB.EllAppDB);
            MySqlParameter idParameter = new MySqlParameter("@id", MySqlDbType.Int32, 0);
            idParameter.Value = ID;
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
        }

        public static List<Chat> GetChats(int AccountID, string ChatRequestID = "")
        {
            staticconn.Open();
            List<Chat> chats = new List<Chat>();
            if (ChatRequestID == "") //I am requesting only all open chat, not the messages
            {
                MySqlCommand cmd = new MySqlCommand("SELECT ChatID as 'chatroom', `from`, content, `to`, `date` FROM log_chat WHERE to_type = 'CHAT_TYPE_USER_TO_USER' AND (`from` = @id or `to` = @id) AND `date` IN (SELECT MAX(`date`) FROM log_chat WHERE ChatID <> '' GROUP BY ChatID) ORDER BY `date` desc;", staticconn);
                MySqlParameter idParameter = new MySqlParameter("@id", MySqlDbType.Int32, 0);
                idParameter.Value = AccountID;
                cmd.Parameters.Add(idParameter);
                MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Chat c = new Chat(ChatType.CHAT_TYPE_USER_TO_USER, r["chatroom"].ToString(), r["content"].ToString(), Misc.GetUsernameByID(Convert.ToInt32(r["from"])).ToString(), Misc.GetUsernameByID(Convert.ToInt32(r["to"])).ToString(), (long)Misc.DateTimeToUnixTimestamp(Convert.ToDateTime(r["date"].ToString())));
                    chats.Add(c);
                }
                r.Close();
                staticconn.Close();
                return chats;
            }
            else
            {
                MySqlCommand cmd = new MySqlCommand("SELECT ChatID as 'chatroom', `from`, content, `to`, `date` FROM log_chat WHERE ChatID = @chatid AND to_type = 'CHAT_TYPE_USER_TO_USER' ORDER BY `date` ASC;", staticconn);
                MySqlParameter chatidParameter = new MySqlParameter("@chatid", MySqlDbType.String, 0);
                chatidParameter.Value = ChatRequestID;
                cmd.Parameters.Add(chatidParameter);
                MySqlDataReader r = cmd.ExecuteReader();
                while(r.Read())
                {
                    Chat c = new Chat(ChatType.CHAT_TYPE_USER_TO_USER, r["chatroom"].ToString(), r["content"].ToString(), Misc.GetUsernameByID(Convert.ToInt32(r["from"])).ToString(), Misc.GetUsernameByID(Convert.ToInt32(r["to"])).ToString(), (long)Misc.DateTimeToUnixTimestamp(Convert.ToDateTime(r["date"].ToString())));
                    chats.Add(c);
                }
                r.Close();
                staticconn.Close();
                return chats;
            }
        }
    }
}
