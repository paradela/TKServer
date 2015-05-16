using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKServer
{
    public class Master : MarshalByRefObject, IMaster
    {
        private static Master master = null;

        //List of available servers. URL -> IServer
        private Dictionary<string, WorkerServer> availableServers = new Dictionary<string, WorkerServer>();

        private Master() { }

        public static Master Singleton
        {
            get
            {
                if (master == null)
                {
                    master = new Master();
                }
                return master;
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        static Int32 unixTimestamp()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public void RegisterServer(string url)
        {
            System.Console.WriteLine("Add server {0}", url);
            IServer remoteServer = (IServer)Activator.GetObject(typeof(IServer), url);
            if (remoteServer == null)
            {
                System.Console.WriteLine("Could not connect to the remote server!");
            }
            else
            {
                lock (this.availableServers)
                {
                    var server = new WorkerServer();
                    server.Uri = url;
                    server.Working = false;
                    server.HeartBeat = unixTimestamp();
                    server.RemoteRef = remoteServer;
                    this.availableServers.Add(url, server);
                }
            }

        }

        public bool RemoveServer(string url)
        {
            throw new NotImplementedException();
        }

        public void JobFinished(string id)
        {
            throw new NotImplementedException();
        }

        public void Ping(string sender, string msg)
        {
            lock (this.availableServers)
            {
                Console.WriteLine(String.Format("Message from {0} : {1}", sender, msg));
                this.availableServers[sender].HeartBeat = unixTimestamp();
            }
        }

        public string GetServer()
        {
            var worker = "";
            var minTimestamp = unixTimestamp() - 6;
            lock (this.availableServers)
            {
                worker = (from server in availableServers.Values
                               where server.Working == false && server.HeartBeat > minTimestamp
                               orderby server.HeartBeat descending
                               select server.Uri).FirstOrDefault();

            }
            return worker;
        }

    }

    public class WorkerServer
    {
        public String Uri { get; set; }
        public bool Working { get; set; }
        public Int32 HeartBeat { get; set; }
        public IServer RemoteRef { get; set; }
    }
}
