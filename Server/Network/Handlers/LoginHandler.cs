using Server.Classes;
using Server.definitions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Server.Network.Alchemy.Classes;
using Server.Network.Packets;

namespace Server.Network.Handlers
{
    public class LoginHandler
    {

        public static void DoLogin(UserContext aContext, List<Session> sessions, ConcurrentDictionary<string, Connection> onlineConnections, dynamic obj, string json)
        {
	        Console.WriteLine("LOGIN REQUEST FROM " + aContext.ClientAddress);
	        Console.WriteLine(json);
	        string username = (string)obj.Username;
	        string psw = (string)obj.Psw;
	        bool wantWelcomeMessage = Convert.ToBoolean((int)obj.WantWelcomeMessage);
	        User u = new User(username.ToUpper(), psw.ToUpper());
	        if (!u.Validate())
	        {
		        Connection conn;

		        Session s = new Session(0, null, aContext);
		        MessagePacket loginInfo = new MessagePacket(MessageType.MSG_TYPE_LOGIN_INFO, 0, -1, new LoginResponse{ LoginResult = LoginResult.WrongCredentials });
		        s.SendMessage(loginInfo);

		        onlineConnections.TryRemove(aContext.ClientAddress.ToString(), out conn);
		        conn.timer.Dispose();
	        }
	        else
	        {
		        Session s = new Session(u.GetID(), u, aContext);
		        sessions.Add(s);

		        MessagePacket loginInfo = new MessagePacket(MessageType.MSG_TYPE_LOGIN_INFO, 0, s.GetUser().GetID(), new LoginResponse{ LoginResult = LoginResult.WrongCredentials, AccountID = s.GetUser().GetID() });
                            
		        s.SendMessage(loginInfo);
				
		        if(wantWelcomeMessage)
		        {
			        //Create the welcome message object
			        Chat chat = new Chat{ chattype = ChatType.CHAT_TYPE_GLOBAL_CHAT, text = "Benvenuto " + s.GetUser().GetUsername() };
			        s.GetUser().SetOnline();
			        var welcomeMessage = new MessagePacket(MessageType.MSG_TYPE_CHAT, 0, s.GetUser().GetID(), chat);
			        s.SendMessage(welcomeMessage);
		        }
	        }
        }
    }
}
