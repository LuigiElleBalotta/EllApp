using Lappa.ORM;

namespace Server.Classes.Entities
{
	public class Friendship : Entity
	{
		public string person1 { get; set; }
		public string person2 { get; set; }
		public bool isPending { get; set; }
	}
}
