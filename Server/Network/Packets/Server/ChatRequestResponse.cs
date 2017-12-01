using Server.Classes;

namespace Server.Network.Packets.Server
{
	public class ChatRequestResponse : Response
	{
		public string ChatRoomID { get; set; }
		public ChatMessage[] Messages { get; set; }
	}
}
