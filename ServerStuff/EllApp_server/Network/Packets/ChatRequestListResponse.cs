using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EllApp_server.Classes;
using EllApp_server.Classes.Entities;

namespace EllApp_server.Network.Packets
{
	public class ChatRequestListResponse
	{
		public List<Chat> ChatList { get; set; }
	}
}
