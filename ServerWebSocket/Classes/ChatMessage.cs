using System;

namespace ServerWebSocket.Classes
{
	public class ChatMessage : Chat
	{
		public int idMessage { get; set; }
		public string Content { get; set; }
		public DateTime Time { get; set; }
		public long TimeStamp { get; set; }
		public int OwnerID { get; set; }
		public string OwnerUsername { get; set; }
		public int ReceiverID { get; set; }
		public string ReceiverUsername { get; set; }
	}
}
