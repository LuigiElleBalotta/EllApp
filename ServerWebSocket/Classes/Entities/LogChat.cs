using Lappa.ORM;
using ServerWebSocket.definitions;

namespace ServerWebSocket.Classes.Entities
{
	public class LogChat : Entity
	{
		public int		idLog { get; set; }
		public string	content { get; set; }
		public int		from { get; set; }
		public ChatType toType { get; set; }
		public int		to { get; set; }
	}
}
