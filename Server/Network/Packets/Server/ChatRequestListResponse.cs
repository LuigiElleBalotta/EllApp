using System.Collections.Generic;
using Server.Classes;

namespace Server.Network.Packets.Server
{
	public class ChatRequestListResponse : Response
	{
		public List<Chat> ChatList { get; set; }
	}
}
