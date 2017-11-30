using System;
using Lappa.ORM;

namespace Server.Classes.Entities
{
	public class Account : Entity
	{
		public int		idAccount { get; set; }
		public string	username { get; set; }
		public string	password { get; set; }
		public string	email { get; set; }
		public DateTime data_creazione { get; set; }
		public string	last_connection { get; set; }
		public string	last_ip { get; set; }
		public bool		isOnline { get; set; }
	}
}
