using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using ServerWebSocket.Classes;
using ServerWebSocket.Network;

namespace ServerWebSocket
{
    public static class Utils
    {
        public static DB mysqlDB;
    }

    public class Startup
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected static ConcurrentDictionary<string, Connection> OnlineConnections = new ConcurrentDictionary<string, Connection>();
        public static List<Session> Sessions = new List<Session>();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            var webSocketOptions = new WebSocketOptions()
                                   {
                                       KeepAliveInterval = TimeSpan.FromSeconds(120),
                                       ReceiveBufferSize = 4 * 1024
                                   };
            app.UseWebSockets(webSocketOptions);

            app.Use(async (http, next) =>
                    {
                        if (http.WebSockets.IsWebSocketRequest)
                        {
                            var webSocket = await http.WebSockets.AcceptWebSocketAsync();
                            
                            Connection connection;
                            ClientContext cc = new ClientContext
                                               {
                                                   Port = http.Connection.LocalPort,
                                                   IPAddress = http.Connection.LocalIpAddress.ToString(),
                                                   WebSocket = webSocket
                                               };
                            
                            while (webSocket.State == WebSocketState.Open)
                            {
                                var token = CancellationToken.None;
                                var buffer = new ArraySegment<Byte>(new Byte[4096]);
                                var received = await webSocket.ReceiveAsync(buffer, token);

                                switch (received.MessageType)
                                {
                                    case WebSocketMessageType.Text:
                                        var request = Encoding.UTF8.GetString(buffer.Array,
                                                                              buffer.Offset,
                                                                              buffer.Count);
                                        var type = WebSocketMessageType.Text;
                                        
                                        cc.Token = token;
                                        cc.Type = type;
                                        connection = new Connection { Context = cc, IP = cc.IPAddress };

                                        // Add a connection Object to thread-safe collection
                                        OnlineConnections.TryAdd(cc.IPAddress, connection);
                                        Sessions.Add(new Session(0, null, cc));

                                        var response = Encoding.UTF8.GetBytes("Echo from server :" + request);
                                        buffer = new ArraySegment<Byte>(response);
                                        await webSocket.SendAsync(buffer, type, true, token);
                                        break;
                                }
                            }

                            
                            OnlineConnections.TryRemove(cc.IPAddress, out connection);
                            if (connection != null) //E' riuscito a rimuovere la connessione.
                            {
                                Sessions.Remove(Sessions.First(s =>
                                                               {
                                                                   bool res = (s.context.IPAddress == connection.IP);
                                                                   if (res)
                                                                   {
                                                                       if(s.GetID() > 0)
                                                                           AccountMgr.SetOffline(s.user);   
                                                                   }
                                                                   return res;
                                                               }));
                                // Dispose timer to stop sending messages to the client.
                                connection.timer.Dispose();
                            }
                        } else {
                            await next();
                        }

                    });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
