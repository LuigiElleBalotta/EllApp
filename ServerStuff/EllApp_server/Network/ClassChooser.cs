using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alchemy.Classes;
using EllApp_server.Classes;
using EllApp_server.definitions;
using EllApp_server.Network.Handlers;
using Newtonsoft.Json;

namespace EllApp_server.Network
{
	public class ClassChooser
	{
		public static void Handle(UserContext aContext, List<Session> Sessions, ConcurrentDictionary<string, Connection> OnlineConnections)
		{
			var json = aContext.DataFrame.ToString();
			dynamic obj = JsonConvert.DeserializeObject(json);
			Type type;
			MethodInfo metodo;

			/*Handlers declaration*/
			ChatHandler chatHandler = new ChatHandler();
			LoginHandler lh = new LoginHandler();
			MessageHandler messageHandler = new MessageHandler();

			switch ((int)obj.Type)
			{
				case (int)CommandType.Login:
					type = lh.GetType();
					metodo = type.GetMethod("DoLogin");
					metodo.Invoke(lh, new object[]{ aContext, Sessions, OnlineConnections, obj, json });
					break;
				case (int)CommandType.Message:
					type = messageHandler.GetType();
					metodo = type.GetMethod("HandleMessage");
					metodo.Invoke(messageHandler, new object[]{ aContext, obj, Sessions });
					break;
				case (int)CommandType.ChatsRequest:
					type = chatHandler.GetType();
					metodo = type.GetMethod("ChatRequestList");
					metodo.Invoke(chatHandler, new object[]{ Sessions, obj });
					break;
				case (int)CommandType.ChatListRequest:
					type = chatHandler.GetType();
					metodo = type.GetMethod("ChatRequestList");
					metodo.Invoke(chatHandler, new object[]{ Sessions, obj });
					break;
			}
		}
	}
}
