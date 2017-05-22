using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EllApp_server.definitions;

namespace EllApp_server.Classes.Entities
{
	public class ChatMessage
	{
		public int ID { get; set; }
		public string ChatRoom { get; set; }
		public int MessageFrom { get; set; }
		public ChatType MessageToType { get; set; }
		public int MessageTo { get; set; }
		public string Text { get; set; }
		public DateTime Date { get; set; }

		public string FromUsername { get; set; }
		public string ToUsername { get; set; }
	}
}
