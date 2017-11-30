using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using Server.Classes.Entities;
using Server.definitions;

namespace Server.Classes
{
    public class AccountMgr
    {
        public static bool CreateAccount(string username, string password, string email)
        {
	        username = username.ToUpper();
	        password = password.ToUpper();
	        email = email.ToUpper();

            byte[] passwordbyte = Encoding.ASCII.GetBytes(username + ":" + password);
            var sha_pass = SHA1.Create();
            byte[] bytehash = sha_pass.ComputeHash(passwordbyte);
            var hashedpsw = Utility.HexStringFromBytes(bytehash);

            Account acc = new Account
                          {
                              data_creazione = DateTime.Now,
                              email = email,
                              last_connection = DateTime.MinValue.ToString( Misc.DATETIME_FORMAT ),
                              username = username,
                              password = hashedpsw
                          };

            if( Utils.mysqlDB.EllAppDB.Insert<Account>( acc )) {
                Console.WriteLine("Account created.");
                return true;
            } else {
                Console.WriteLine("Failure creating account.");
                return false;
            }
        }

        public static Account GetAccount( string username, string password )
        {
            username = username;
            byte[] passwordbyte = Encoding.ASCII.GetBytes(username + ":" + password);
            var sha_pass = SHA1.Create();
            byte[] bytehash = sha_pass.ComputeHash(passwordbyte);
            password = Utility.HexStringFromBytes(bytehash);

            password = password.ToUpper();
            username = username.ToUpper();

            return Utils.mysqlDB.EllAppDB.Single<Account>( acc => acc.username == username && acc.password == password );
        }

        public static bool Validate( Account account )
        {
            if (account.idAccount > 0)
                return true;

            return false;
        }

        public static string GetUsername( Account account )
        {
            return account.username;
        }

        public static int GetID( Account account )
        {
            return account.idAccount;
        }

        public static bool IsOnline( Account account )
        {
            return Utils.mysqlDB.EllAppDB.Single<Account>( row => row.idAccount == account.idAccount ).isOnline;
        }

        public static void SetOnline( Account account )
        {
            Utils.mysqlDB.EllAppDB.Update<Account>( row => row.idAccount == account.idAccount, row => row.isOnline.Set( true ));
        }

        public static void SetOffline( Account account )
        {
            Utils.mysqlDB.EllAppDB.Update<Account>( row => row.idAccount == account.idAccount, row => row.isOnline.Set( false ));
        }

	    public static List<ChatMessage> GetChat(int AccountID, string ChatRequestID)
	    {
		    /*staticconn.Open();
		    List<ChatMessage> chatMessages = new List<ChatMessage>();
		    MySqlCommand cmd = new MySqlCommand("SELECT ChatID as 'chatroom', `from`, content, `to`, `date` FROM log_chat WHERE ChatID = @chatid AND to_type = 'CHAT_TYPE_USER_TO_USER' ORDER BY `date` ASC;", staticconn);
		    MySqlParameter chatidParameter = new MySqlParameter("@chatid", MySqlDbType.String, 0);
		    chatidParameter.Value = ChatRequestID;
		    cmd.Parameters.Add(chatidParameter);
		    MySqlDataReader r = cmd.ExecuteReader();
		    while(r.Read())
		    {
			    int from = Convert.ToInt32(r["from"]);
			    int to = Convert.ToInt32(r["to"]);
			    ChatMessage chatMessage = new ChatMessage
								{
									chattype = ChatType.CHAT_TYPE_USER_TO_USER, 
									ChatRoom = r["chatroom"].ToString(), 
									Content = r["content"].ToString(), 
									OwnerID = from,
									OwnerUsername = Misc.GetUsernameByID(from), 
									ReceiverID = to,
									ReceiverUsername = Misc.GetUsernameByID(to), 
									TimeStamp = (long)Misc.DateTimeToUnixTimestamp(Convert.ToDateTime(r["date"].ToString()))
								};
			    chatMessages.Add(chatMessage);
		    }
		    r.Close();
		    staticconn.Close();*/
		    return null;//chatMessages;
	    }

        public static List<Chat> GetChats(int AccountID)
        {
	        /*staticconn.Open();
	        List<Chat> chats = new List<Chat>();
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
            return chats;*/
            return null;
        }
    }
}
