using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Server.Classes;
using Server.definitions;
using Server.Network.Handlers;
using Newtonsoft.Json;
using NLog;
using Server.Network;
using Server.Network.Packets.Client;
using Server.Network.Packets.Server;

namespace EllApp_server.Network
{
	public class ClassChooser
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public static List<GenericResponsePacket> Handle(ClientContext aContext, string json, List<Session> sessions, ConcurrentDictionary<string, Connection> OnlineConnections)
		{
			List<GenericResponsePacket> ret = new List<GenericResponsePacket>();

			logger.Info($"======================================={Environment.NewLine}Received packet: {Environment.NewLine} {json} { Environment.NewLine }=======================================");

			GenericRequestPacket obj = JsonConvert.DeserializeObject<GenericRequestPacket>(json);
			Type type;
			MethodInfo metodo;

			/*Handlers declaration*/
			ChatHandler chatHandler = new ChatHandler();
			LoginHandler lh = new LoginHandler();
			MessageHandler messageHandler = new MessageHandler();
			RegistrationHandler rh = new RegistrationHandler();

			switch (obj.Type)
			{
				case CommandType.Login:
					type = lh.GetType();
					metodo = type.GetMethod("DoLogin");
					ret.AddRange((List<GenericResponsePacket>)metodo.Invoke(lh, new object[]{ aContext, sessions, OnlineConnections, obj.LoginPacket }));
					break;
				case CommandType.Message:
					type = messageHandler.GetType();
					metodo = type.GetMethod("HandleMessage");
					metodo.Invoke(messageHandler, new object[]{ aContext, obj, sessions });
					break;
				case CommandType.ChatsRequest:
					type = chatHandler.GetType();
					metodo = type.GetMethod("ChatRequest");
					metodo.Invoke(chatHandler, new object[]{ sessions, obj });
					break;
				case CommandType.ChatListRequest:
					type = chatHandler.GetType();
					metodo = type.GetMethod("ChatRequestList");
                    ret.AddRange((List<GenericResponsePacket>)metodo.Invoke(chatHandler, new object[]{ sessions, obj }));
					break;
				case CommandType.Registration:
					type = rh.GetType();
					metodo = type.GetMethod("RegisterAccount");
					ret.AddRange((List<GenericResponsePacket>)metodo.Invoke(rh, new object[]{ obj.RegistrationPacket, aContext }));
					break;
			}

            if( ret.Count == 0 )
                ret.Add( new GenericResponsePacket{ Client = aContext, Response = new Response{ ErrorMessage = "Nessun metodo da chiamare in base alla richiesta.", ResponseType = ResponseType.Error } });

            return ret;
		}
	}
}
