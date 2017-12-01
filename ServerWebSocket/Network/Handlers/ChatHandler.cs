using System;
using System.Collections.Generic;
using System.Linq;
using ServerWebSocket.Classes;
using ServerWebSocket.definitions;
using ServerWebSocket.Network.Packets;
using ServerWebSocket.Network.Packets.Client;

namespace ServerWebSocket.Network.Handlers
{
	public class ChatHandler
	{
		public string ChatRequest(List<Session> Sessions, ChatsRequestPacket obj)
		{
			//Console.WriteLine("Received CHAT REQUEST from AccountID = " + obj.accid + ".");
			int accountID = obj.AccID;
			string ChatRequestID = "";
			if (obj.ChatRequestID != null)
				ChatRequestID = obj.ChatRequestID;
			var chats = AccountMgr.GetChat(accountID, ChatRequestID);
			string ret = Sessions.First(s => s.user.idAccount == accountID).CreateMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT_REQUEST_RESPONSE, 0, accountID, chats));
            return ret;
		}

		public string ChatRequestList(List<Session> Sessions, dynamic obj)
		{
			Console.WriteLine("Received CHAT LIST REQUEST from AccountID = " + obj.accid + ".");
			int accountid = obj.accid;
			var chatlist = AccountMgr.GetChats(accountid);
			return Sessions.First(s => s.user.idAccount == accountid).CreateMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT_REQUEST_LIST_RESPONSE, 0, accountid, new ChatRequestListResponse{ ChatList = chatlist }));
		}
	}
}
