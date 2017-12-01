using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using ServerWebSocket.Classes;
using ServerWebSocket.definitions;
using ServerWebSocket.Network.Handlers;
using Newtonsoft.Json;
using NLog;
using Server.Network;
using Server.Network.Alchemy.Classes;
using ServerWebSocket.Network.Packets;
using ServerWebSocket.Network.Packets.Client;

namespace ServerWebSocket.Network
{
	public class ClassChooser
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public static string Handle(ClientContext aContext, string json, List<Session> sessions, ConcurrentDictionary<string, Connection> OnlineConnections)
		{

			logger.Info($"======================================={Environment.NewLine}Received packet: {Environment.NewLine} {json} { Environment.NewLine }=======================================");

            string ret = "";

			GenericRequestPacket obj = JsonConvert.DeserializeObject<GenericRequestPacket>( json );

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
					ret = (string)metodo.Invoke(lh, new object[]{ aContext, sessions, OnlineConnections, obj, json });
					break;
				case CommandType.Message:
					type = messageHandler.GetType();
					metodo = type.GetMethod("HandleMessage");
					metodo.Invoke(messageHandler, new object[]{ aContext, obj, sessions });
					break;
				case CommandType.ChatsRequest:
					type = chatHandler.GetType();
					metodo = type.GetMethod("ChatRequest");
                    ret = (string)metodo.Invoke(chatHandler, new object[]{ sessions, obj.ChatsRequestPacket });
					break;
				case CommandType.ChatListRequest:
					type = chatHandler.GetType();
					metodo = type.GetMethod("ChatRequestList");
                    ret = (string)metodo.Invoke(chatHandler, new object[]{ sessions, obj });
					break;
				case CommandType.Registration:
					type = rh.GetType();
					metodo = type.GetMethod("RegisterAccount");
                    ret = (string)metodo.Invoke(rh, new object[]{ obj, aContext });
					break;
			}

            return ret;
		}
	}
}
