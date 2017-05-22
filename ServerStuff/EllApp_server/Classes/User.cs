using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using EllApp_server.Classes.Entities;
using MySql.Data.MySqlClient;
using EllApp_server.definitions;

namespace EllApp_server.Classes
{
    public class User
    {
        public int ID { get; set; } = 0;
		public string Username { get; set; }
		public string last_ip { get; set; }
		public string email { get; set; }

        static MySqlConnection staticconn = new MySqlConnection(DB.GetConnectionString());
        public User(string _username, string _password)
        {
            Username = _username;
            byte[] passwordbyte = Encoding.ASCII.GetBytes(Username + ":" + _password);
            var sha_pass = SHA1.Create();
            byte[] bytehash = sha_pass.ComputeHash(passwordbyte);
            _password = Utility.HexStringFromBytes(bytehash);

	        _password = _password.ToUpper();
	        Username = Username.ToUpper();
            
            MySqlCommand cmd = new MySqlCommand("SELECT idAccount, email, last_ip FROM accounts WHERE UPPER(username) = @username AND UPPER(password) = @password;", DB.EllAppDB);
            MySqlParameter passwordParameter = new MySqlParameter("@password", MySqlDbType.VarChar, 0);
            MySqlParameter usernameParameter = new MySqlParameter("@username", MySqlDbType.VarChar, 0);
            passwordParameter.Value = _password;
            usernameParameter.Value = Username;
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

            return false;
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

	    public static List<ChatMessage> GetChat(int accountId, string chatRequestId)
	    {
		    staticconn.Open();
		    List<ChatMessage> chatMessages = new List<ChatMessage>();
		    MySqlCommand cmd = new MySqlCommand("SELECT ID, ChatRoom, MessageFrom, MessageToType, MessageTo, Text, Date FROM chatmessage WHERE ChatRoom = @chatroom AND MessageToType = 2 AND (MessageFrom = @id or MessageTo = @id) ORDER BY `Date` ASC;", staticconn);
		    MySqlParameter chatidParameter = new MySqlParameter("@chatroom", MySqlDbType.String, 0) {Value = chatRequestId};
		    cmd.Parameters.Add(chatidParameter);
		    MySqlParameter idParameter = new MySqlParameter("@id", MySqlDbType.Int32, 0) {Value = accountId};
		    cmd.Parameters.Add(idParameter);
		    MySqlDataReader r = cmd.ExecuteReader();
		    while(r.Read())
		    {
			    int from = Convert.ToInt32(r["from"]);
			    int to = Convert.ToInt32(r["to"]);
			    ChatMessage chatMessage = new ChatMessage
								{
									MessageToType = ChatType.CHAT_TYPE_USER_TO_USER, 
									ChatRoom = r["chatroom"].ToString(), 
									Text = r["content"].ToString(), 
									MessageFrom = from,
									FromUsername = Misc.GetUsernameByID(from), 
									MessageTo = to,
									ToUsername = Misc.GetUsernameByID(to), 
									Date = Convert.ToDateTime(r["date"].ToString())
								};
			    chatMessages.Add(chatMessage);
		    }
		    r.Close();
		    staticconn.Close();
		    return chatMessages;
	    }

        public static List<Chat> GetChats(int accountId)
        {
	        staticconn.Open();
	        List<Chat> chats = new List<Chat>();
            MySqlCommand cmd = new MySqlCommand("SELECT ChatRoom, MessageFrom, MessageToType, MessageTo, Text, Date FROM chatmessage WHERE MessageToType = 2 AND (MessageFrom = @id or MessageTo = @id) GROUP BY ChatRoom ORDER BY Date desc;", staticconn);
            MySqlParameter idParameter = new MySqlParameter("@id", MySqlDbType.Int32, 0);
            idParameter.Value = accountId;
            cmd.Parameters.Add(idParameter);
            MySqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                Chat c = new Chat
						 {
							 ChatRoom = r["ChatRoom"].ToString(),
							 Chattype = (ChatType)Convert.ToInt16(r["MessageToType"]),
							 LastMessage = r["Text"].ToString(),
							 LastMessageDate = Convert.ToDateTime(r["Date"]),
							 LastMessageUserID = Convert.ToInt16(r["MessageFrom"])
						 };
	            c.LastMessageUsername = Misc.GetUsernameByID(c.LastMessageUserID);
                chats.Add(c);
            }
            r.Close();
            staticconn.Close();
            return chats;
        }
    }
}
