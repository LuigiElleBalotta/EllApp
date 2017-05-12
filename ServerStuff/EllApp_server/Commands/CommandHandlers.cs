using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy;
using EllApp_server.Classes;
using EllApp_server.definitions;
using Newtonsoft.Json;
using NLog;

namespace EllApp_server.Commands
{
	public class CommandHandlers
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public void Online(List<Session> sessions)
		{
			Console.WriteLine("Online Users: " + Utility.GetOnlineUsers(sessions));
			foreach(var session in sessions)
			{
				Console.WriteLine("User ID: " + session.GetUser().GetID());
				Console.WriteLine("User Name: " + session.GetUser().GetUsername());
				Console.WriteLine("Connected from: " + session.GetContext().ClientAddress);
				Console.WriteLine("-------------------------------------");
			}
		}

		public void Serverinfo(WebSocketServer aServer)
		{
			Console.WriteLine("Listening on: " + aServer.ListenAddress);
			Console.WriteLine("Port: " + aServer.Port);
		}

		public void Gsm(List<Session> sessions) //Global Server Message
		{
			foreach (var session in sessions)
			{
				Console.WriteLine("Message to send: ");
				var msg = Console.ReadLine();
				Chat c = new Chat(ChatType.CHAT_TYPE_GLOBAL_CHAT, "", msg, "Server Message", session.GetUser().GetUsername());
				var message = new MessagePacket(MessageType.MSG_TYPE_CHAT, 0, session.GetUser().GetID(), JsonConvert.SerializeObject(c));
				session.SendMessage(message);
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
			Account.CreateAccount(username, password, email);
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
							if (sessions.Any(s => s.GetUser().GetID() == to))
							{
								logger.Info("L'utente è nella lista delle sessioni");
								if (sessions.First(s => s.GetUser().GetID() == to).GetUser().IsOnline())
								{
									logger.Info("L'utente è online");
									Session session = sessions.SingleOrDefault(s => s.GetUser().GetID() == to);
									logger.Info("Sending message to user");
									session?.SendMessage(msg);
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
	}
}
