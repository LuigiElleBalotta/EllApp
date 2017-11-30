using System.Collections.Generic;
using EllApp_server.Classes;

namespace EllApp_server.Network.Packets
{
	public class ChatRequestListResponse
	{
		public List<Chat> ChatList { get; set; }
	}
}
