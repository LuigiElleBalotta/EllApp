﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemy;
using Alchemy.Classes;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json;
using EllApp_server.Classes;
using EllApp_server.definitions;
using System.Diagnostics;
using System.Configuration;
using System.Reflection;
using EllApp_server.Commands;
using EllApp_server.Network;
using EllApp_server.Network.Handlers;

namespace EllApp_server
{
    class Program
    {
        //Thread-safe collection of Online Connections.
        protected static ConcurrentDictionary<string, Connection> OnlineConnections = new ConcurrentDictionary<string, Connection>();
        public static List<Session> Sessions = new List<Session>();

        static void Main(string[] args)
        {
            // instantiate a new server - acceptable port and IP range,
            // and set up your methods.

            var aServer = new WebSocketServer(Convert.ToInt16(ConfigurationSettings.AppSettings["serverport"]), System.Net.IPAddress.Any)
            {
                OnReceive = OnReceive,
                OnSend = OnSend,
                OnConnected = OnConnect,
                OnDisconnect = OnDisconnect,
                TimeOut = new TimeSpan(0, 5, 0)
            };
            aServer.Start();


            Console.ForegroundColor = ConsoleColor.Red;
            Console.Title = "EllApp WebSocket Server";
            Console.WriteLine("Running EllApp WebSocket Server ...");
            Console.WriteLine("[Type \"exit\" and hit enter to stop the server]");

            //Open DB Instance
            DB.Instantiate();

            // Accept commands on the console and keep it alive
            var command = string.Empty;
            while (command != "exit")
            {
                if (command != "")
                {
					CommandHandlers cmd = new CommandHandlers();
	                Type type = cmd.GetType();
	                MethodInfo metodo = type.GetMethod(Utility.ToTitleCase(command));
                    switch (command)
                    {
	                    case "fakemessage":
						case "gsm":
                        case "online":
	                        metodo.Invoke(cmd, new object[]{ Sessions });
                            break;
                        case "serverinfo":
	                        metodo.Invoke(cmd, new object[]{ aServer });
                            break;
                        case "createaccount":
                        case "clearconsole":
                            break;
                        default:
                            Console.WriteLine("Unknown command");
                            break;
                    }
                }
                command = Console.ReadLine();
            }

            aServer.Stop();
            Environment.Exit(0);
        }

        public static void OnConnect(UserContext aContext)
        {

            Console.WriteLine("Client Connected From : " + aContext.ClientAddress.ToString());

            // Create a new Connection Object to save client context information
            var conn = new Connection { Context = aContext };

            // Add a connection Object to thread-safe collection
            OnlineConnections.TryAdd(aContext.ClientAddress.ToString(), conn);
        }



        public static void OnReceive(UserContext aContext)
        {
            try
            {
				ClassChooser.Handle(aContext, Sessions, OnlineConnections);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

        }
        public static void OnSend(UserContext aContext)
        {
            Console.WriteLine("Data Sent To : " + aContext.ClientAddress.ToString());
        }

        public static void OnDisconnect(UserContext aContext)
        {
            Console.WriteLine("Client Disconnected : " + aContext.ClientAddress.ToString());

            // Remove the connection Object from the thread-safe collection
            Connection conn;
            OnlineConnections.TryRemove(aContext.ClientAddress.ToString(), out conn);
            var sessionlist = Sessions.Where(s => s.GetContext().ClientAddress.ToString() == aContext.ClientAddress.ToString());
            foreach (var s in sessionlist)
                s.GetUser().SetOffline();
            Sessions.Remove(Sessions.First(s => s.GetContext().ClientAddress.ToString() == aContext.ClientAddress.ToString()));
            // Dispose timer to stop sending messages to the client.
            conn.timer.Dispose();
        }

        

    }

    public class Connection
    {
        public System.Threading.Timer timer;
        public UserContext Context { get; set; }
        public Connection()
        {
            this.timer = new System.Threading.Timer(this.TimerCallback, null, 0, 1000);
        }

        private void TimerCallback(object state)
        {
            try
            {
                // Sending Data to the Client
                //Context.Send("[" + Context.ClientAddress.ToString() + "] " + System.DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

    }

}