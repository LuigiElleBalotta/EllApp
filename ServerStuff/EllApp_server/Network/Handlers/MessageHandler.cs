using System.Collections.Generic;
using System.Linq;
using Alchemy.Classes;
using EllApp_server.Classes;
using EllApp_server.Classes.Entities;
using EllApp_server.definitions;
using NLog;

namespace EllApp_server.Network.Handlers
{
	public class MessageHandler
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public void HandleMessage(UserContext aContext, dynamic obj, List<Session> sessions)
		{
			logger.Info("MESSAGE PACKET FROM " + aContext.ClientAddress);
			string messagecontent = obj.Message;
			ChatType toType = obj.ToType;
			int from = obj.From;
			int to = obj.To;

			SaveChat(from, to, toType, messagecontent);

			switch (toType)
			{
				case ChatType.CHAT_TYPE_GLOBAL_CHAT: //Send message to all connected clients (that we have stored in sessions)
					HandleGlobalChat(sessions, from, to, toType, messagecontent);
					break;
				case ChatType.CHAT_TYPE_USER_TO_USER:
					HandleUserChat(sessions, obj);
					break;
				case ChatType.CHAT_TYPE_GROUP_CHAT:
					HandleGroupChat();
					break;
				case ChatType.CHAT_TYPE_NULL:
					HandleChatNull();
					break;
			}
		}

		private void SaveChat(int from, int to, ChatType toType, string messagecontent)
		{
			new ChatManager
					  {
						  ChatRoom = Misc.CreateChatRoomID(from, to),
						  Text = messagecontent,
						  MessageToType = toType,
						  MessageFrom = from,
						  MessageTo = to
					  }.Save();
		}

		private void HandleGlobalChat(List<Session> sessions, int from, int to, ChatType toType, string messagecontent)
		{
			var stCLog = new ChatManager();
			var o = 1;
			foreach (var session in sessions)
			{
				if (session.GetUser().ID != from) //Do not send message to ourselves
				{
					ChatMessage chat = new ChatMessage{ MessageToType = ChatType.CHAT_TYPE_GLOBAL_CHAT, ChatRoom = Misc.CreateChatRoomID(from, session.GetUser().ID), Text = messagecontent, FromUsername = Misc.GetUsernameByID(from), ToUsername = Misc.GetUsernameByID(session.GetUser().ID)};
					session.SendMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT, from, session.GetUser().ID, chat));
					stCLog.Text = messagecontent;
					stCLog.MessageToType = toType;
					stCLog.MessageFrom = from;
					stCLog.MessageTo = session.GetUser().ID;
					stCLog.Save();
					o++;
				}
			}
			logger.Info("Message sent to {0} users", (o - 1));
		}

		private void HandleUserChat(List<Session> sessions, dynamic obj)
		{
			logger.Info("Received MSG_TYPE_CHAT_WITH_USER packet.");
			//If the receiving User is online, we can send the message to him, otherwise he will load everything at next login
			if (sessions.Any(s => s.GetUser().ID == (int)obj.To))
			{
				Session singleOrDefault = sessions.SingleOrDefault(s => s.GetUser().ID == (int)obj.To);
				if (singleOrDefault != null && singleOrDefault.GetUser().IsOnline())
				{
					ChatMessage chat = new ChatMessage{ MessageToType = ChatType.CHAT_TYPE_USER_TO_USER, ChatRoom = Misc.CreateChatRoomID(obj.To, obj.From), Text = obj.Message, FromUsername = obj.From, ToUsername = obj.To};
					Session session = sessions.SingleOrDefault(s => s.GetUser().ID == (int)obj.To);
					session?.SendMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT, obj.From, obj.To, chat));
				}
			}
			else
				logger.Info("The receiver was not online, message will be read at next login");
		}

		private void HandleGroupChat()
		{
			logger.Warn("MSG_TYPE_CHAT_WITH_GROUP NOT YET IMPLEMENTED");
		}

		private void HandleChatNull()
		{
			logger.Error("Message Type is NULL.");
		}
	}
}
