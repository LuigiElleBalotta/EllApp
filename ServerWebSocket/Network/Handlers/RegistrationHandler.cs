using System.Net.WebSockets;
using ServerWebSocket.Classes;
using ServerWebSocket.definitions;
using ServerWebSocket.Network.Packets;

namespace ServerWebSocket.Network.Handlers
{
	public class RegistrationHandler
	{
		public static void RegisterAccount(dynamic obj, ClientContext uContext)
		{
			string username = obj.Username.ToString();
			string password = obj.Psw.ToString();
			string email = obj.Email.ToString();

			bool result = AccountMgr.CreateAccount(username, password, email);

			Session tmpSession = new Session(0, null, uContext);

			MessagePacket registrationInfo = new MessagePacket(MessageType.MSG_TYPE_REGISTRATION_RESPONSE, 0, -1, new RegistrationResponse{ Result = result });
                            
			tmpSession.SendMessage(registrationInfo);
		}
	}
}
