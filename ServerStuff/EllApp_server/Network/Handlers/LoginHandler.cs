using Alchemy.Classes;
using EllApp_server.Classes;
using EllApp_server.definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using EllApp_server.Classes.Entities;
using EllApp_server.Network.Packets;

namespace EllApp_server.Network.Handlers
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
		        Session s = new Session(u.ID, u, aContext);
		        sessions.Add(s);

		        MessagePacket loginInfo = new MessagePacket(MessageType.MSG_TYPE_LOGIN_INFO, 0, s.GetUser().ID, new LoginResponse{ LoginResult = LoginResult.WrongCredentials, AccountID = s.GetUser().ID });
                            
		        s.SendMessage(loginInfo);
				
		        if(wantWelcomeMessage)
		        {
			        User utente = s.GetUser();
			        //Create the welcome message object
			        ChatMessage chat = new ChatMessage{ MessageToType = ChatType.CHAT_TYPE_GLOBAL_CHAT, Text = "Benvenuto " + utente.Username, ChatRoom = "GLOBAL", Date = DateTime.Now, MessageFrom = 0, MessageTo = utente.ID };
			        utente.SetOnline();
			        var welcomeMessage = new MessagePacket(MessageType.MSG_TYPE_CHAT, 0, utente.ID, chat);
			        s.SendMessage(welcomeMessage);
		        }
	        }
        }
    }
}
