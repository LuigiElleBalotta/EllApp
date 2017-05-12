using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alchemy.Classes;
using EllApp_server.Classes;
using EllApp_server.definitions;
using EllApp_server.Network.Packets;

namespace EllApp_server.Network.Handlers
{
	public class RegistrationHandler
	{
		public static void RegisterAccount(dynamic obj, UserContext uContext)
		{
			string username = obj.Username.ToString();
			string password = obj.Psw.ToString();
			string email = obj.Email.ToString();

			bool result = Account.CreateAccount(username, password, email);

			Session tmpSession = new Session(0, null, uContext);

			MessagePacket registrationInfo = new MessagePacket(MessageType.MSG_TYPE_REGISTRATION_RESPONSE, 0, -1, new RegistrationResponse{ Result = result });
                            
			tmpSession.SendMessage(registrationInfo);
		}
	}
}
