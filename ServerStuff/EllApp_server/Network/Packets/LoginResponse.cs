using EllApp_server.definitions;

namespace EllApp_server.Network.Packets
{
	public class LoginResponse
	{
		public LoginResult LoginResult { get; set; }
		public int		   AccountID { get; set; } = 0;
	}
}
