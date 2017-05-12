using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EllApp_server.Classes.Entities
{
	public class Friendship
	{
		public string person1 { get; set; }
		public string person2 { get; set; }
		public bool isPending { get; set; }
	}
}
