using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EllApp_server.Classes;
using EllApp_server.definitions;
using EllApp_server.Network.Packets;
using Newtonsoft.Json;

namespace EllApp_server.Network.Handlers
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
			string chats = JsonConvert.SerializeObject(User.GetChats(accountID, ChatRequestID));
			Sessions.First(s => s.GetUser().GetID() == accountID).SendMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT_REQUEST_RESPONSE, 0, accountID, chats));
		}

		public void ChatRequestList(List<Session> Sessions, dynamic obj)
		{
			Console.WriteLine("Received CHAT LIST REQUEST from AccountID = " + obj.accid + ".");
			int accountid = obj.accid;
			var chatlist = User.GetChats(accountid, "");
			Sessions.First(s => s.GetUser().GetID() == accountid).SendMessage(new MessagePacket(MessageType.MSG_TYPE_CHAT_REQUEST_LIST_RESPONSE, 0, accountid, new ChatRequestListResponse{ ChatList = chatlist }));
		}
	}
}
