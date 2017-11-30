using ServerWebSocket.Classes;

namespace ServerWebSocket.Network.Packets
{
	public class ChatRequestResponse
	{
		public string ChatRoomID { get; set; }
		public ChatMessage[] Messages { get; set; }
	}
}
