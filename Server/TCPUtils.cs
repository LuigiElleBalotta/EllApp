using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using EllApp_server.Network;
using Lappa.ORM.Constants;
using NLog;
using Server.Classes;
using Server.Commands;
using Server.Network;
using Server.Network.Alchemy;
using Server.Network.Alchemy.Classes;

namespace Server
{
    public static class Utils
    {
        public static DB mysqlDB;
    }

    // State object for reading client data asynchronously  
    public class StateObject {  
        // Client  socket.  
        public Socket workSocket = null;  
        // Size of receive buffer.  
        public const int BufferSize = 1024;  
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];  
        // Received data string.  
        public StringBuilder sb = new StringBuilder();    
    } 

	class TcpHelper
	{  
        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false); 
		private static TcpListener listener { get; set; }  
		private static bool accept { get; set; } = false;  
		private static Logger logger = LogManager.GetCurrentClassLogger();
        protected static ConcurrentDictionary<string, Connection> OnlineConnections = new ConcurrentDictionary<string, Connection>();
        public static List<Session> Sessions = new List<Session>();
        private static SocketServer aServer;
   
		public static void StartServer(string host, int port) {  
			IPAddress address = IPAddress.Parse(host);  
			listener = new TcpListener(address, port);  
   
			listener.Start();  
			accept = true;  
   
			Console.WriteLine($"Server started. Listening to TCP clients at 127.0.0.1:{port}");  
		}  
   
		public static void Listen()
		{  
			if(listener != null && accept) 
			{  
				// Continue listening.  
				while (true)
				{  
					Console.WriteLine("Waiting for client...");  
					var clientTask = listener.AcceptTcpClientAsync(); // Get the client  
   
					if(clientTask.Result != null)
					{  
						Console.WriteLine("Client connected. Waiting for data.");  
						var client = clientTask.Result;  
						string message = "";  
   
						while (message != null && message != "exit")
						{  

							if (message != "")
							{
								CommandHandlers cmd = new CommandHandlers();
								Type type = cmd.GetType();
								MethodInfo metodo = type.GetMethod(Utility.ToTitleCase(message));
								switch (message)
								{
									case "fakemessage":
									case "gsm":
									case "online":
										metodo.Invoke(cmd, new object[]{ Sessions });
										break;
									case "serverinfo":
										metodo.Invoke(cmd, new object[]{ Program.Configuration });
										break;
									case "commands":
										metodo.Invoke(cmd, null);
										break;
									case "createaccount":
									case "clearconsole":
										break;
									default:
										logger.Warn($"Unknown command \"{message}\".");
										break;
								}
							}
							message = Console.ReadLine();

							//------------------------------------------

							/*byte[] data = Encoding.ASCII.GetBytes("Send next data: [enter 'quit' to terminate] ");  
							client.GetStream().Write(data, 0, data.Length);  
   
							byte[] buffer = new byte[1024];  
							client.GetStream().Read(buffer, 0, buffer.Length);  
   
							message = Encoding.ASCII.GetString(buffer);  
							Console.WriteLine(message);  */
						}  
						Console.WriteLine("Closing connection.");  
						client.GetStream().Dispose();  
					}  
				}  
			}  
		}  

        public static void StartListening( Config.Config Configuration ) {  
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];  

            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry( Configuration.Host );  
            IPAddress ipAddress = ipHostInfo.AddressList[0];  
            IPEndPoint localEndPoint = new IPEndPoint( ipAddress, Configuration.Port );  

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,  
                SocketType.Stream, ProtocolType.Tcp );  

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try {  
                listener.Bind(localEndPoint);  
                listener.Listen(100);  

                while (true) {  
                    // Set the event to nonsignaled state.  
                    allDone.Reset();  

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");  
                    listener.BeginAccept(   
                        new AsyncCallback(AcceptCallback),  
                        listener );  

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();  
                }  

            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  

            Console.WriteLine("\nPress ENTER to continue...");  
            Console.Read();  

        }  

        public static void AcceptCallback(IAsyncResult ar) {  
            // Signal the main thread to continue.  
            allDone.Set();  

            // Get the socket that handles the client request.  
            Socket listener = (Socket) ar.AsyncState;  
            Socket handler = listener.EndAccept(ar);  

            // Create the state object.  
            StateObject state = new StateObject();  
            state.workSocket = handler;  
            handler.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,  
                new AsyncCallback(ReadCallback), state);  
        }  

        public static void ReadCallback(IAsyncResult ar) {  
            String content = String.Empty;  

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject) ar.AsyncState;  
            Socket handler = state.workSocket;  

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);  

            if (bytesRead > 0) {  
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(  
                    state.buffer,0,bytesRead));  

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString(); 
   
                while (content != null && content != "exit")
                {  

                    if (content != "")
                    {
                        CommandHandlers cmd = new CommandHandlers();
                        Type type = cmd.GetType();
                        MethodInfo metodo = type.GetMethod(Utility.ToTitleCase(content));
                        switch (content)
                        {
                            case "fakemessage":
                            case "gsm":
                            case "online":
                                metodo.Invoke(cmd, new object[]{ Sessions });
                                break;
                            case "serverinfo":
                                metodo.Invoke(cmd, new object[]{ Program.Configuration });
                                break;
                            case "commands":
                                metodo.Invoke(cmd, null);
                                break;
                            case "createaccount":
                            case "clearconsole":
                                break;
                            default:
                                logger.Warn($"Unknown command \"{content}\".");
                                break;
                        }
                    }
                    content = Console.ReadLine();
                }


                if (content.IndexOf("<EOF>") > -1) {  
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",  
                        content.Length, content );  
                    // Echo the data back to the client.  
                    Send(handler, content);  
                } else {  
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,  
                    new AsyncCallback(ReadCallback), state);  
                }  
            }  
        }  

        private static void Send(Socket handler, String data) {  
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);  

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,  
                new AsyncCallback(SendCallback), handler);  
        }  

        private static void SendCallback(IAsyncResult ar) {  
            try {  
                // Retrieve the socket from the state object.  
                Socket handler = (Socket) ar.AsyncState;  

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);  
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);  

                handler.Shutdown(SocketShutdown.Both);  
                handler.Close();  

            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }  


        public static void StartListening()
        {
            aServer = new SocketServer(Convert.ToInt16(Program.Configuration.Port), IPAddress.Parse( Program.Configuration.Host ))
                      {
                          OnReceive = OnReceive,
                          OnSend = OnSend,
                          OnConnected = OnConnect,
                          OnDisconnect = OnDisconnect,
                          TimeOut = new TimeSpan(0, 5, 0)
                      };
            aServer.Start();

            Utils.mysqlDB = new DB( DatabaseType.MySql, Program.Configuration );

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
                        case "commands":
                        case "createaccount":
                        case "clearconsole":
                            metodo.Invoke( cmd, null );
                            break;
                        default:
                            logger.Warn($"Unknown command \"{command}\".");
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

            logger.Info("Client Connected From : " + aContext.ClientAddress);

            // Create a new Connection Object to save client context information
            var conn = new Connection { Context = aContext, IP = aContext.ClientAddress };

            // Add a connection Object to thread-safe collection
            OnlineConnections.TryAdd(aContext.ClientAddress.ToString(), conn);
            Sessions.Add(new Session(0, null, aContext));
        }
        public static void OnReceive(UserContext aContext)
        {
            try
            {
                ClassChooser.Handle(aContext, Sessions, OnlineConnections);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static void OnSend(UserContext aContext)
        {
            logger.Info("Data Sent To : " + aContext.ClientAddress);
        }
		
        public static void OnDisconnect(UserContext aContext)
        {
            logger.Info("Client Disconnected : " + aContext.ClientAddress);
            Sessions = Sessions;
            // Remove the connection Object from the thread-safe collection
            Connection conn;
            OnlineConnections.TryRemove(aContext.ClientAddress.ToString(), out conn);
            if (conn != null) //E' riuscito a rimuovere la connessione.
            {
                Sessions.Remove(Sessions.First(s =>
                                               {
                                                   bool res = (s.context.ClientAddress == conn.IP);
                                                   if (res)
                                                   {
                                                       if(s.GetID() > 0)
                                                           AccountMgr.SetOffline(s.user);   
                                                   }
                                                   return res;
                                               }));
                // Dispose timer to stop sending messages to the client.
                conn.timer.Dispose();
            }
            
        }
	}  
}
