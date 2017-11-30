using EllApp_server.Classes;

namespace EllApp_server.Network.Packets
{
	public class ChatRequestResponse
	{
		public string ChatRoomID { get; set; }
		public ChatMessage[] Messages { get; set; }
	}
}
