using System.Collections.Generic;
using Server.Classes;

namespace Server.Network.Packets
{
	public class ChatRequestListResponse
	{
		public List<Chat> ChatList { get; set; }
	}
}
