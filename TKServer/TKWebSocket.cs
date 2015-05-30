using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Owin.WebSocket;

namespace TKServer
{
    public delegate byte[] ExAPDU(byte[] apdu);

    public class TKWebSocket : WebSocketConnection
    {
        private Mutex mutex;
        private byte[] BytesRead = null;

        //var a = new ExAPDU(SendRcvAPDU);
        //a(message.Array);
        private byte[] SendRcvMessage(byte[] apdu)
        {
            SendText(apdu, true);
            mutex.WaitOne();
            return BytesRead;
        }

        public override Task OnMessageReceived(ArraySegment<byte> message, System.Net.WebSockets.WebSocketMessageType type)
        {
            //if (response to Read command)
            //{ 
            //  BytesRead = message;
            //  mutex.ReleaseMutex;
            //}
            //else //new Command
            //{
            Card c = new Card();
            RemoteServer server = RemoteServer.Singleton;
            IList<CTSWriteOperation> operations;
            string tkmsg;
            uint status, result;
            server.RunCommand("id", "", out status, out result, out tkmsg, out operations, c, new ExAPDU(SendRcvMessage));
            //}
            Console.WriteLine(String.Format("Message Received"));
            
            return SendText(message, true);
        }

        public override void OnOpen()
        {
            mutex = new Mutex();
        }

        public override void OnClose(System.Net.WebSockets.WebSocketCloseStatus? closeStatus, string closeStatusDescription)
        {
            mutex.Close();
        }
    }
}
