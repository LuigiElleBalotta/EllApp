using ServerWebSocket.definitions;

namespace ServerWebSocket.Network.Packets
{
	public class LoginResponse
	{
		public LoginResult LoginResult { get; set; }
		public int		   AccountID { get; set; } = 0;
	}
}
