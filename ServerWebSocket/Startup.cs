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
                            while (webSocket.State == WebSocketState.Open)
                            {
                                ClientContext cc = new ClientContext
                                                   {
                                                       Port = http.Connection.LocalPort,
                                                       IPAddress = http.Connection.LocalIpAddress.ToString(),
                                                       WebSocket = webSocket
                                                   };
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
                                        var data = Encoding.UTF8.GetBytes("Echo from server :" + request);
                                        buffer = new ArraySegment<Byte>(data);
                                        await webSocket.SendAsync(buffer, type, true, token);
                                        break;
                                }
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
