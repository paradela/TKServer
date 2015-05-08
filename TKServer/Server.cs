using System;
using Microsoft.Owin.Hosting;


namespace TKServer
{
    public class RemoteServer : MarshalByRefObject
    {

        public void SetNextJob(String id)
        {
        }
    }

    public class Server
    {
        public bool Master = true;
        public short RemotingPort = 8080;
        public short Port = 80;
        public string Address = "localhost";

        public static void Main(string[] args)
        {
            Server server = new Server();
            string baseAddress = String.Format("http://{0}:{1}", server.Address, server.Port);
            IDisposable webServer = WebApp.Start<Startup>(url: baseAddress);
            
            Console.WriteLine("Press any key to exit..");
            Console.ReadLine();
            webServer.Dispose();
        }
    }
}
