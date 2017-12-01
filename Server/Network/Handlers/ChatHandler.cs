using System;
using System.Collections.Generic;
using System.Linq;
using Server.Classes;
using Server.definitions;
using Server.Network.Packets;
using Server.Network.Packets.Client;
using Server.Network.Packets.Server;

namespace Server.Network.Handlers
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
			Sessions.First(s => s.user.idAccount == accountID).CreateResponse(new MessagePacket(MessageType.MSG_TYPE_CHAT_REQUEST_RESPONSE, 0, accountID, chats));
		}

		public void ChatRequestList(List<Session> Sessions, ChatRequestListPacket packet)
		{
			Console.WriteLine("Received CHAT LIST REQUEST from AccountID = " + packet.AccountID + ".");
			int accountid = packet.AccountID;
			List<Chat> chatlist = AccountMgr.GetChats( accountid );
			Sessions.First(s => s.user.idAccount == accountid).CreateResponse(new MessagePacket(MessageType.MSG_TYPE_CHAT_REQUEST_LIST_RESPONSE, 0, accountid, new ChatRequestListResponse{ ChatList = chatlist }));
		}
	}
}
