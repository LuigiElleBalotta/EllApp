using Server.Classes;
using Server.definitions;

namespace Server.Network.Packets.Server
{
	public class LoginResponse : Response
	{
		public LoginResult LoginResult { get; set; }
		public int		   AccountID { get; set; } = 0;
	}
}
