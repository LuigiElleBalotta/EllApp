using System;
using System.Collections.Generic;
using System.Linq;
using ServerWebSocket.Classes;
using ServerWebSocket.definitions;
using ServerWebSocket.Network.Packets;

namespace ServerWebSocket.Network.Handlers
{
	public class ChatHandler
	{
		public void ChatRequest(List<Session> Sessions, dynamic obj)
		{
			Console.WriteLine("Received CHAT REQUEST from AccountID = " + obj.accid + ".");
			int accountID = obj.accid;
			string ChatRequestID = "";
			if (obj.ChatRequestID != null)
				ChatRequestID = obj.ChatRequestID;
			var chats = AccountMgr.GetChat(accountID, ChatRequestID);
			Sessions.First(s => s.user.idAccount == accountID).SendMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT_REQUEST_RESPONSE, 0, accountID, chats));
		}

		public void ChatRequestList(List<Session> Sessions, dynamic obj)
		{
			Console.WriteLine("Received CHAT LIST REQUEST from AccountID = " + obj.accid + ".");
			int accountid = obj.accid;
			var chatlist = AccountMgr.GetChats(accountid);
			Sessions.First(s => s.user.idAccount == accountid).SendMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT_REQUEST_LIST_RESPONSE, 0, accountid, new ChatRequestListResponse{ ChatList = chatlist }));
		}
	}
}
