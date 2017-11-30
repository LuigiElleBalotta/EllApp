using System.Collections.Generic;
using ServerWebSocket.Classes;

namespace ServerWebSocket.Network.Packets
{
	public class ChatRequestListResponse
	{
		public List<Chat> ChatList { get; set; }
	}
}
