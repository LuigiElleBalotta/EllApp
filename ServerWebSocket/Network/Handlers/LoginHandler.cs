using ServerWebSocket.Classes;
using ServerWebSocket.definitions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using ServerWebSocket.Classes.Entities;
using ServerWebSocket.Network.Packets;

namespace ServerWebSocket.Network.Handlers
{
    public class LoginHandler
    {

        public static string DoLogin(ClientContext aContext, List<Session> sessions, ConcurrentDictionary<string, Connection> onlineConnections, dynamic obj, string json)
        {
            string ret = "";
	        Console.WriteLine($"LOGIN REQUEST FROM {aContext.IPAddress}:{aContext.Port}");
	        Console.WriteLine(json);
	        string username = (string)obj.Username;
	        string psw = (string)obj.Psw;
	        bool wantWelcomeMessage = Convert.ToBoolean((int)obj.WantWelcomeMessage);
	        Account u = AccountMgr.GetAccount(username.ToUpper(), psw.ToUpper());
	        if (!AccountMgr.Validate( u ))
	        {
		        Session s = new Session(0, null, aContext);
		        MessagePacket loginInfo = new MessagePacket(MessageType.MSG_TYPE_LOGIN_INFO, 0, -1, new LoginResponse{ LoginResult = LoginResult.WrongCredentials });
		        ret = s.CreateMessage(loginInfo);

                //@todo:remove websocketconnection by key
		        //onlineConnections.TryRemove(aContext.ClientAddress.ToString(), out conn);
		        //conn.timer.Dispose();
	        }
	        else
	        {
                
		        Session s = new Session(u.idAccount, u, aContext);
		        sessions.Add(s);

		        MessagePacket loginInfo = new MessagePacket(MessageType.MSG_TYPE_LOGIN_INFO, 0, s.user.idAccount, new LoginResponse{ LoginResult = LoginResult.Success, AccountID = s.user.idAccount });
                            
		        ret = s.CreateMessage(loginInfo);
				
		        if(wantWelcomeMessage)
		        {
			        //Create the welcome message object
			        Chat chat = new Chat{ chattype = ChatType.CHAT_TYPE_GLOBAL_CHAT, text = "Benvenuto " + s.user.username };
			        AccountMgr.SetOnline( s.user );
			        var welcomeMessage = new MessagePacket(MessageType.MSG_TYPE_CHAT, 0, s.user.idAccount, chat);
			        ret = s.CreateMessage(welcomeMessage);
		        }
	        }

            return ret;
        }
    }
}
