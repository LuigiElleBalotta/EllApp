using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using EllApp_server.Network;
using Lappa.ORM.Constants;
using Newtonsoft.Json;
using Server.Classes;
using Server.Network;
using Server.Network.Packets.Server;
using Session = Server.Classes.Session;

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
   
        public static void StartListening( Config.Config Configuration ) { 
            
            Utils.mysqlDB = new DB( DatabaseType.MySql, Program.Configuration );

            Console.WriteLine( $"MySQL DB status: {Utils.mysqlDB.Connected}");

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

                Program.Server = new ServerContext
                                  {
                                      Socket = listener,
                                      IPAddress = listener.LocalEndPoint.ToString()
                                  };

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
            StateObject state = new StateObject {workSocket = handler};


            SocketAddress socketAddress = state.workSocket.RemoteEndPoint.Serialize();

            ClientContext cc = new ClientContext
                               {
                                   Socket = state.workSocket,
                                   IPAddress = state.workSocket.RemoteEndPoint.ToString()
                               };

            // Create a new Connection Object to save client context information
            var conn = new Connection { Context = cc, IP = state.workSocket.RemoteEndPoint };

            Console.WriteLine( $"Connected client from {conn.IP}" );

            // Add a connection Object to thread-safe collection
            Program.Server.OnlineConnections.TryAdd(conn.IP.ToString(), conn);
            Program.Server.Sessions.Add(new Session(conn.IP.ToString(), null, cc));
            
            handler.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,  
                                    new AsyncCallback(ReadCallback), state);  
        }  

        public static void ReadCallback(IAsyncResult ar) {  
            StateObject state = (StateObject) ar.AsyncState;  
            try {
                String content = String.Empty;  

                // Retrieve the state object and the handler socket  
                // from the asynchronous state object.  
                
                Socket handler = state.workSocket;  

                // Read data from the client socket.   
                int bytesRead = handler.EndReceive(ar);  

                if (bytesRead > 0) {  
                    // There  might be more data, so store the data received so far. 

                    content = Encoding.ASCII.GetString( state.buffer, 0, bytesRead ); 
 
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content );  
                    
                    if( Program.Server.OnlineConnections.TryGetValue( state.workSocket.RemoteEndPoint.ToString(), out Connection val ) ) {
                        var responses = ClassChooser.Handle( val.Context, content, Program.Server.Sessions, Program.Server.OnlineConnections );

                        foreach( GenericResponsePacket response in responses ) {
                            Send( response.Client.Socket, JsonConvert.SerializeObject( response.Response )  );
                        }
                    } else {
                        Console.WriteLine( $"Nessun client connesso dall'ip {state.workSocket.RemoteEndPoint}" );
                    }
                } 
            }
            catch( Exception ex ) {
                Console.WriteLine( ex.Message );
                Console.WriteLine( ex.InnerException?.Message );

                Connection conn;
                Program.Server.OnlineConnections.TryRemove(state.workSocket.RemoteEndPoint.ToString(), out conn);
                if (conn != null) //E' riuscito a rimuovere la connessione.
                {
                    Session sess = Program.Server.Sessions.First(s =>
                                                              {
                                                                  bool res = (s.ID == conn.IP.ToString());
                                                                  if (res)
                                                                  {
                                                                      if(s.ID != "" )
                                                                          if( s.user != null )
                                                                              AccountMgr.SetOffline( s.user );   
                                                                  }
                                                                  return res;
                                                              });
                    if( sess != null )
                        Program.Server.Sessions.Remove( sess );
                    // Dispose timer to stop sending messages to the client.
                    conn.timer.Dispose();
                }
            }
             
        }  

        private static void Send(Socket handler, String data) {  
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);  

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,  
                new AsyncCallback(SendCallback), handler); 
            
            try {
                StateObject state = new StateObject {workSocket = handler};
                handler.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback( ReadCallback ), state );
            }
            catch( Exception ex ) {
                Console.WriteLine( ex.Message );
            }
        }  

        private static void SendCallback(IAsyncResult ar) {  
            try {  
                // Retrieve the socket from the state object.  
                Socket handler = (Socket) ar.AsyncState;  

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend( ar );  

                Console.WriteLine("Sent {0} bytes to client.", bytesSent);  

            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }  
	}  
}
