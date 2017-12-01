using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using ServerWebSocket.Classes;
using ServerWebSocket.definitions;
using NLog;

namespace ServerWebSocket.Network.Handlers
{
	public class MessageHandler
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public List<string> HandleMessage(ClientContext aContext, dynamic obj, List<Session> sessions)
		{

            List<string> ret = new List<string>();

			logger.Info($"MESSAGE PACKET FROM {aContext.IPAddress}:{aContext.Port}");
			string messagecontent = obj.Message;
			ChatType toType = obj.ToType;
			int from = obj.From;
			int to = obj.To;

			MakeLog(from, to, toType, messagecontent);

			switch (toType)
			{
				case ChatType.CHAT_TYPE_GLOBAL_CHAT: //Send message to all connected clients (that we have stored in sessions)
					ret = HandleGlobalChat(sessions, from, to, toType, messagecontent);
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

            return ret;
		}

		private void MakeLog(int from, int to, ChatType toType, string messagecontent)
		{
			new Log_Manager
					  {
						  ChatID = Misc.CreateChatRoomID(from, to),
						  content = messagecontent,
						  to_type = toType,
						  from = from,
						  to = to
					  }.SaveLog();
		}

		private List<string> HandleGlobalChat(List<Session> sessions, int from, int to, ChatType toType, string messagecontent)
		{
            List<string> ret = new List<string>();
			var stCLog = new Log_Manager();
			var o = 1;
			foreach (var session in sessions)
			{
				if (session.user.idAccount != from) //Do not send message to ourselves
				{
					Chat chat = new Chat(ChatType.CHAT_TYPE_GLOBAL_CHAT, Misc.CreateChatRoomID(from, session.user.idAccount), messagecontent, Misc.GetUsernameByID(from), Misc.GetUsernameByID(session.user.idAccount));
					ret.Add(session.CreateMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT, from, session.user.idAccount, chat)));
					stCLog.content = messagecontent;
					stCLog.to_type = toType;
					stCLog.from = from;
					stCLog.to = session.user.idAccount;
					stCLog.SaveLog();
					o++;
				}
			}
			logger.Info("Message sent to {0} users", (o - 1));
            return ret;
		}

		private void HandleUserChat(List<Session> sessions, dynamic obj)
		{
			logger.Info("Received MSG_TYPE_CHAT_WITH_USER packet.");
			//If the receiving User is online, we can send the message to him, otherwise he will load everything at next login
			if (sessions.Any(s => s.user.idAccount == (int)obj.To))
			{
				Session singleOrDefault = sessions.SingleOrDefault(s => s.user.idAccount == (int)obj.To);
				if (singleOrDefault != null && AccountMgr.IsOnline( singleOrDefault.user ))
				{
					Chat chat = new Chat(ChatType.CHAT_TYPE_USER_TO_USER, Misc.CreateChatRoomID(obj.To, obj.From), obj.Message, obj.From, obj.To);
					Session session = sessions.SingleOrDefault(s => s.user.idAccount == (int)obj.To);
					session?.CreateMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT, obj.From, obj.To, chat));
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
