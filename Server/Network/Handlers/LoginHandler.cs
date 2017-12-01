﻿using Server.Classes;
using Server.definitions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Server.Classes.Entities;
using Server.Network.Packets;

namespace Server.Network.Handlers
{
    public class LoginHandler
    {

        public static void DoLogin(ClientContext aContext, List<Session> sessions, ConcurrentDictionary<string, Connection> onlineConnections, dynamic obj, string json)
        {
	        Console.WriteLine("LOGIN REQUEST FROM " + aContext.IPAddress);
	        Console.WriteLine(json);
	        string username = (string)obj.Username;
	        string psw = (string)obj.Psw;
	        bool wantWelcomeMessage = Convert.ToBoolean((int)obj.WantWelcomeMessage);
	        Account u = AccountMgr.GetAccount(username.ToUpper(), psw.ToUpper());
	        if (!AccountMgr.Validate( u ))
	        {
		        Connection conn;

		        Session s = new Session("tmp_" + Program.Server.Sessions.Count + 1, null, aContext);
		        MessagePacket loginInfo = new MessagePacket(MessageType.MSG_TYPE_LOGIN_INFO, 0, -1, new LoginResponse{ LoginResult = LoginResult.WrongCredentials });
		        s.SendMessage(loginInfo);

		        onlineConnections.TryRemove(aContext.IPAddress, out conn);
		        conn.timer.Dispose();
	        }
	        else
	        {
		        Session s = new Session(u.idAccount.ToString(), u, aContext);
		        sessions.Add(s);

		        MessagePacket loginInfo = new MessagePacket(MessageType.MSG_TYPE_LOGIN_INFO, 0, s.user.idAccount, new LoginResponse{ LoginResult = LoginResult.WrongCredentials, AccountID = s.user.idAccount });
                            
		        s.SendMessage(loginInfo);
				
		        if(wantWelcomeMessage)
		        {
			        //Create the welcome message object
			        Chat chat = new Chat{ chattype = ChatType.CHAT_TYPE_GLOBAL_CHAT, text = "Benvenuto " + s.user.username };
			        AccountMgr.SetOnline( s.user );
			        var welcomeMessage = new MessagePacket(MessageType.MSG_TYPE_CHAT, 0, s.user.idAccount, chat);
			        s.SendMessage(welcomeMessage);
		        }
	        }
        }
    }
}
