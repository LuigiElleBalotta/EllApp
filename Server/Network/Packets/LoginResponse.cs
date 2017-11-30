using Server.definitions;

namespace Server.Network.Packets
{
	public class LoginResponse
	{
		public LoginResult LoginResult { get; set; }
		public int		   AccountID { get; set; } = 0;
	}
}
