using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EllApp_server.definitions;

namespace EllApp_server.Classes.Entities
{
	public class LogChat
	{
		public int		idLog { get; set; }
		public string	content { get; set; }
		public int		from { get; set; }
		public ChatType toType { get; set; }
		public int		to { get; set; }
	}
}
