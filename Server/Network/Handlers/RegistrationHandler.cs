using System.Collections.Generic;
using Server.Classes;
using Server.definitions;
using Server.Network.Packets;
using Server.Network.Packets.Client;
using Server.Network.Packets.Server;

namespace Server.Network.Handlers
{
	public class RegistrationHandler
	{
		public static List<GenericResponsePacket> RegisterAccount(RegistrationPacket packet, ClientContext uContext)
		{
            List<GenericResponsePacket> responsePackets = new List<GenericResponsePacket>();

			bool result = AccountMgr.CreateAccount(packet.Username, packet.Psw, packet.Email);

            GenericResponsePacket grp = new GenericResponsePacket
                                        {
                                            Client = uContext,
                                            Response = new RegistrationResponse
                                                       {
                                                           MessageType = MessageType.MSG_TYPE_REGISTRATION_RESPONSE,
                                                           Result = result
                                                       },
                                            SenderType = SenderType.Server
                                        };

            responsePackets.Add( grp );

            return responsePackets;
		}
	}
}
