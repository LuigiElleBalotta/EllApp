using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alchemy;
using Alchemy.Classes;
using EllApp_server.Classes;
using EllApp_server.definitions;
using Newtonsoft.Json;

namespace EllApp_server.Network.Handlers
{
	public class MessageHandler
	{
		[System.Diagnostics.Conditional("DEBUG")]
		public void HandleMessage(UserContext aContext, dynamic obj, List<Session> Sessions)
		{
			Console.WriteLine("MESSAGE PACKET FROM " + aContext.ClientAddress);
			string messagecontent = obj.Message;
			ChatType to_type = obj.ToType;
			int from = obj.From;
			int to = obj.To;

			MakeLog(from, to, to_type, messagecontent);

			switch (to_type)
			{
				case ChatType.CHAT_TYPE_GLOBAL_CHAT: //Send message to all connected clients (that we have stored in sessions)
					HandleGlobalChat(Sessions, from, to, to_type, messagecontent);
					break;
				case ChatType.CHAT_TYPE_USER_TO_USER:
					HandleUserChat(Sessions, obj);
					break;
				case ChatType.CHAT_TYPE_GROUP_CHAT:
					HandleGroupChat();
					break;
				case ChatType.CHAT_TYPE_NULL:
					HandleChatNull();
					break;
			}
		}

		private void MakeLog(int from, int to, ChatType to_type, string messagecontent)
		{
			var log = new Log_Manager();
			log.ChatID = Misc.CreateChatRoomID(from, to);
			log.content = messagecontent;
			log.to_type = to_type;
			log.from = from;
			log.to = to;
			log.SaveLog();
		}

		private void HandleGlobalChat(List<Session> Sessions, int from, int to, ChatType to_type, string messagecontent)
		{
			var StCLog = new Log_Manager();
			var o = 1;
			foreach (var session in Sessions)
			{
				if (session.GetUser().GetID() != from) //Do not send message to ourselves
				{
					Chat c = new Chat(ChatType.CHAT_TYPE_GLOBAL_CHAT, Misc.CreateChatRoomID(from, session.GetUser().GetID()), messagecontent, Misc.GetUsernameByID(from), Misc.GetUsernameByID(session.GetUser().GetID()));
					session.SendMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT, from, session.GetUser().GetID(), JsonConvert.SerializeObject(c)));
					StCLog.content = messagecontent;
					StCLog.to_type = to_type;
					StCLog.from = from;
					StCLog.to = session.GetUser().GetID();
					StCLog.SaveLog();
					o++;
				}
			}
			Console.WriteLine("Message sent to {0} users", (o - 1));
		}

		private void HandleUserChat(List<Session> Sessions, dynamic obj)
		{
			Console.WriteLine("Received MSG_TYPE_CHAT_WITH_USER packet.");
			//If the receiving User is online, we can send the message to him, otherwise he will load everything at next login
			if (Sessions.Any(s => s.GetUser().GetID() == (int)obj.To))
			{
				if (Sessions.SingleOrDefault(s => s.GetUser().GetID() == (int)obj.To).GetUser().IsOnline())
				{
					Chat c = new Chat(ChatType.CHAT_TYPE_USER_TO_USER, Misc.CreateChatRoomID(obj.To, obj.From), obj.Message, obj.From, obj.To);
					Session session = Sessions.SingleOrDefault(s => s.GetUser().GetID() == (int)obj.To);
					session.SendMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT, obj.From, obj.To, JsonConvert.SerializeObject(c)));
				}
			}
			else
				Console.WriteLine("DEBUG: The receiver was not online, message will be read at next login");
		}

		private void HandleGroupChat()
		{
			Console.WriteLine("MSG_TYPE_CHAT_WITH_GROUP NOT YET IMPLEMENTED");
		}

		private void HandleChatNull()
		{
			Console.WriteLine("Message Type is NULL.");
		}
	}
}
