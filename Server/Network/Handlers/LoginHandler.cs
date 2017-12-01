using Server.Classes;
using Server.definitions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Server.Classes.Entities;
using Server.Network.Packets;
using Server.Network.Packets.Client;
using Server.Network.Packets.Server;

namespace Server.Network.Handlers
{
    public class LoginHandler
    {

        public static List<GenericResponsePacket> DoLogin(ClientContext aContext, List<Session> sessions, ConcurrentDictionary<string, Connection> onlineConnections, LoginPacket packet)
        {
            List<GenericResponsePacket> responsePackets = new List<GenericResponsePacket>();

	        Console.WriteLine("LOGIN REQUEST FROM " + aContext.Socket.RemoteEndPoint);

	        Account u = AccountMgr.GetAccount(packet.Username.ToUpper(), packet.Psw.ToUpper());
	        if (!AccountMgr.Validate( u ))
	        {
                GenericResponsePacket grp = new GenericResponsePacket
                                            {
                                                Client = aContext,
                                                Response = new LoginResponse
                                                           {
                                                              MessageType = MessageType.MSG_TYPE_LOGIN_INFO,
                                                              LoginResult = LoginResult.WrongCredentials
                                                           },
                                                SenderType = SenderType.Server
                                            };

                responsePackets.Add( grp );
	        }
	        else
	        {
		        Session s = sessions.First( session => session.ID == aContext.IPAddress );
		        s.user = u;
                s.context = aContext;


                GenericResponsePacket grpLogin = new GenericResponsePacket
                                                 {
                                                     Client = aContext,
                                                     Response = new LoginResponse
                                                                {
                                                                    MessageType = MessageType.MSG_TYPE_LOGIN_INFO,
                                                                    LoginResult = LoginResult.Success,
                                                                    AccountID = s.user.idAccount
                                                                },
                                                     SenderType = SenderType.Server,
                                                     IDAccountReceiver = s.user.idAccount
                                                 };

                responsePackets.Add( grpLogin );
				
		        if(packet.WantWelcomeMessage)
		        {
			        //Create the welcome message object
			        //Chat chat = new Chat{ chattype = ChatType.CHAT_TYPE_GLOBAL_CHAT, text = "Benvenuto " + s.user.username };
			        AccountMgr.SetOnline( s.user );
			        //var welcomeMessage = new MessagePacket(MessageType.MSG_TYPE_CHAT, 0, s.user.idAccount, chat);
			        //s.CreateResponse(welcomeMessage);
		        }
                /*
                GenericResponsePacket grpWwm = new GenericResponsePacket
                                               {
                                                   Client = aContext,
                                                   Response = new ChatMessage
                                                              {
                                                                  Message = 
                                                              }
                                               };
                */
	        }
            return responsePackets;
        }
    }
}
