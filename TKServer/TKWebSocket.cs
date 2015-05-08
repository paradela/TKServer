using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin.WebSocket;

namespace TKServer
{
    public class TKWebSocket : WebSocketConnection
    {
        public override Task OnMessageReceived(ArraySegment<byte> message, System.Net.WebSockets.WebSocketMessageType type)
        {
            Console.WriteLine(String.Format("Message Received"));
            return SendText(message, true);
        }

        public override void OnOpen()
        {
            Console.WriteLine("Open");
        }

        public override void OnClose(System.Net.WebSockets.WebSocketCloseStatus? closeStatus, string closeStatusDescription)
        {
            Console.WriteLine("Close");
        }
    }
}
