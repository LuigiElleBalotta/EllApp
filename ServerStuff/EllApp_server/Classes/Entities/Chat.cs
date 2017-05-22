using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EllApp_server.definitions;

namespace EllApp_server.Classes.Entities
{
    public class Chat
    {
        public ChatType Chattype { get; set; }
        public string ChatRoom { get; set; }

		public string LastMessage { get; set; }
		public DateTime LastMessageDate { get; set; }
		public string LastMessageUsername { get; set; }
		public int LastMessageUserID { get; set; }
    }
}
