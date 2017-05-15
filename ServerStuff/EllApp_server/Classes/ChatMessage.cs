using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EllApp_server.Classes
{
	public class ChatMessage
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
