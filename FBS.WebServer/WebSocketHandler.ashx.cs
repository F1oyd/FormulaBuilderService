using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;
using FBS.Builder;

namespace FBS.WebServer
{
    public class WebSocketHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
            {
                context.AcceptWebSocketRequest(WebSocketRequest);
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write($"Connect to the following web socket server adress ws://{context.Request.Url.Authority}/ in the web client app.");
            }
        }

        private async Task WebSocketRequest(AspNetWebSocketContext context)
        {
            var socket = context.WebSocket;

            while (true)
            {
                var buffer = new ArraySegment<byte>(new byte[1024]);

                // Listen to client
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                //If input frame is cancelation frame, send close command. 
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    var receivedString = Encoding.UTF8.GetString(buffer.ToArray());

                    var builder = new FormulaBuilder(receivedString);
                    var sendingString = builder.Build();

                    var bytes = Encoding.UTF8.GetBytes(sendingString);

                    await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public bool IsReusable => false;
    }
}