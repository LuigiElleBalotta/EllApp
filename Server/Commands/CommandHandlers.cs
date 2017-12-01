using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NLog;
using Server.Classes;
using Server.definitions;

namespace Server.Commands
{
    public class CommandHandlers
    {
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public void Online(List<Session> sessions)
		{
			Console.WriteLine("Online Users: " + Utility.GetOnlineUsers(sessions));
			foreach(var session in sessions)
			{
				Console.WriteLine("User ID: " + session.user.idAccount);
				Console.WriteLine("User Name: " + session.user.username);
				Console.WriteLine("Connected from: " + session.context.Socket.RemoteEndPoint.Serialize().ToString());
				Console.WriteLine("-------------------------------------");
			}
		}

		public void Serverinfo(ServerContext aServer)
		{
            Console.WriteLine("Listening on: " + aServer.IPAddress);
            Console.WriteLine("Connected Client: " + Program.Server.OnlineConnections.Count );
            Console.WriteLine("Active sessions: " + Program.Server.Sessions.Count );
		}

		public void Gsm(List<Session> sessions) //Global Server Message
		{
			foreach (var session in sessions)
			{
				Console.WriteLine("Message to send: ");
				var msg = Console.ReadLine();
				Chat c = new Chat(ChatType.CHAT_TYPE_GLOBAL_CHAT, "", msg, "Server Message", session.user.username);
				var message = new MessagePacket(MessageType.MSG_TYPE_CHAT, 0, session.user.idAccount, JsonConvert.SerializeObject(c));
				session.CreateResponse(message);
			}
		}

		public void Createaccount()
		{
			Console.WriteLine("Insert username:");
			string username = Console.ReadLine();
			Console.WriteLine("Insert password:");
			string password = Console.ReadLine();
			Console.WriteLine("Insert email:");
			string email = Console.ReadLine();
			AccountMgr.CreateAccount(username, password, email);
		}

		public void Fakemessage(List<Session> sessions)
		{
			Console.WriteLine("Insert the username: ");
			string readLine = Console.ReadLine();
			if (readLine != null)
			{
				string uname = readLine.ToUpper();
				bool validChoice = false;
				do
				{
					Console.WriteLine("Single chat or Group Chat? (1 or 2)");
					int choice = Convert.ToInt16(readLine);
					switch (choice)
					{
						case 1:
							validChoice = true;
							Console.WriteLine("Insert the destinatary username:");
							string duname = readLine.ToUpper();
							Console.WriteLine("Insert your message: ");
							string tmpmessage = readLine;

							int from = Misc.GetUserIDByUsername(uname);
							int to = Misc.GetUserIDByUsername(duname);
							string chatroomid = Misc.CreateChatRoomID(to, from);

							Chat c = new Chat(ChatType.CHAT_TYPE_USER_TO_USER, chatroomid, tmpmessage, uname, duname);
							var msg = new MessagePacket(MessageType.MSG_TYPE_CHAT, from, to, JsonConvert.SerializeObject(c));

							logger.Info($"Sending message to {to} - {duname}");
							if (sessions.Any(s => s.user.idAccount == to))
							{
								logger.Info("L'utente è nella lista delle sessioni");
								if (sessions.First(s => s.user.idAccount == to).user.isOnline)
								{
									logger.Info("L'utente è online");
									Session session = sessions.SingleOrDefault(s => s.user.idAccount == to);
									logger.Info("Sending message to user");
									session?.CreateResponse(msg);
								}
							}

							var log = new Log_Manager();
							log.ChatID = chatroomid;
							log.content = tmpmessage;
							log.to_type = ChatType.CHAT_TYPE_USER_TO_USER;
							log.from = from;
							log.to = to;
							log.SaveLog();
							Console.WriteLine(JsonConvert.SerializeObject(msg));
							Console.WriteLine("Done.");
							break;
						case 2:
							validChoice = true;
							logger.Warn("Not yet implemented.");
							break;
						default:
							logger.Warn("Invalid choice");
							break;
					}
				} while (!validChoice);
			}
		}

		public void Clearconsole()
		{
			Console.Clear();
		}

		public void Commands()
		{
			Console.WriteLine("- clearconsole -> clears console");
			Console.WriteLine("- fakemessage -> send a fake message");
			Console.WriteLine("- gsm -> Global Server message");
			Console.WriteLine("- online -> How many person are connected");
			Console.WriteLine("- serverinfo -> self explained.");
			Console.WriteLine("- createaccount -> create account");
			Console.WriteLine("- exit");
		}
    }
}
