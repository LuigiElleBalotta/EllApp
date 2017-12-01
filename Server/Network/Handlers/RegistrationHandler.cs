using Server.Classes;
using Server.definitions;
using Server.Network.Alchemy.Classes;
using Server.Network.Packets;

namespace Server.Network.Handlers
{
	public class RegistrationHandler
	{
		public static void RegisterAccount(dynamic obj, ClientContext uContext)
		{
			string username = obj.Username.ToString();
			string password = obj.Psw.ToString();
			string email = obj.Email.ToString();

			bool result = AccountMgr.CreateAccount(username, password, email);

			Session tmpSession = new Session("tmp_" + Program.Server.Sessions.Count + 1, null, uContext);

			MessagePacket registrationInfo = new MessagePacket(MessageType.MSG_TYPE_REGISTRATION_RESPONSE, 0, -1, new RegistrationResponse{ Result = result });
                            
			tmpSession.SendMessage(registrationInfo);
		}
	}
}
