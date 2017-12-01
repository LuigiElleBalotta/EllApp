using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
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
            var hashedpsw = Utility.HexStringFromBytes(bytehash).ToUpper();

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
            byte[] passwordbyte = Encoding.ASCII.GetBytes(username + ":" + password);
            var sha_pass = SHA1.Create();
            byte[] bytehash = sha_pass.ComputeHash(passwordbyte);
            password = Utility.HexStringFromBytes(bytehash);

            password = password.ToUpper();
            username = username.ToUpper();

            Account ret = Utils.mysqlDB.EllAppDB.Single<Account>( acc => acc.username == username && acc.password == password );

            return ret;
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

        public static List<ChatRoomForApp> GetChats(int AccountID)
        {
            List<ChatRoomForApp> ret = new List<ChatRoomForApp>();

            //Tutte le chatroom dove è presente l'utente loggato
            List<ChatRoomUsers> accountsChatRoom = Utils.mysqlDB.EllAppDB.Select<ChatRoomUsers>( x => x.IDAccount == AccountID );

            foreach( ChatRoomUsers chatroomuser in accountsChatRoom ) {
                ChatRoom chatroomInfo = Utils.mysqlDB.EllAppDB.Single<ChatRoom>( x => x.ID == chatroomuser.IDChatRoom );
                ChatRoomUsers cru = Utils.mysqlDB.EllAppDB.Single<ChatRoomUsers>( x => x.IDChatRoom == chatroomuser.IDChatRoom && x.IDAccount != chatroomuser.IDAccount );
                ChatRoomForApp crfa = null;

                switch( chatroomInfo.Type ) {
                    case ChatType.CHAT_TYPE_USER_TO_USER:

                        Account destinatario = Utils.mysqlDB.EllAppDB.Single<Account>( x => x.idAccount == cru.IDAccount );

                        crfa = new ChatRoomForApp
                                              {
                                                  ChatRoomID = chatroomInfo.ID,
                                                  ChatRoomName = destinatario.username,
                                                  Destinatario = destinatario.username,
                                                  DestinatarioID = destinatario.idAccount,
                                                  Type = chatroomInfo.Type
                                              };

                        ret.Add( crfa );
                        break;
                    case ChatType.CHAT_TYPE_GROUP_CHAT:
                    case ChatType.CHAT_TYPE_GLOBAL_CHAT: //This have to be inserted manually to db with the correct TYPE.
                        crfa = new ChatRoomForApp
                               {
                                   ChatRoomID = chatroomInfo.ID,
                                   ChatRoomName = chatroomInfo.Name,
                                   Type = chatroomInfo.Type
                               };

                        ret.Add( crfa );
                        break;
                }

            }
            return ret;
        }
    }
}
