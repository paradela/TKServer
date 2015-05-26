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

        public void RegisterServer(String Address, short RemotingPort, short WsPort)
        {
            string tcpUrl = String.Format("tcp://{0}:{1}/Server", Address, RemotingPort);
            string wsUrl = String.Format("http://{0}:{1}", Address, WsPort);
            System.Console.WriteLine("Add server {0}", tcpUrl);
            IServer remoteServer = (IServer)Activator.GetObject(typeof(IServer), tcpUrl);
            if (remoteServer == null)
            {
                System.Console.WriteLine("Could not connect to the remote server!");
            }
            else
            {
                lock (this.availableServers)
                {
                    var server = new WorkerServer();
                    server.TcpUri = tcpUrl;
                    server.WsUri = wsUrl;
                    server.Working = false;
                    server.HeartBeat = unixTimestamp();
                    server.RemoteRef = remoteServer;
                    this.availableServers.Add(tcpUrl, server);
                }
            }

        }

        public bool RemoveServer(string url)
        {
            throw new NotImplementedException();
        }

        public void JobFinished(string id)
        {
            lock (this.availableServers)
            {
                var worker = (from server in availableServers.Values
                              where server.Working = true && server.CurrentId == id
                              select server).FirstOrDefault();
                if (worker != null)
                    worker.Working = false;
            }
        }

        public void Ping(string sender, string msg)
        {
            lock (this.availableServers)
            {
                this.availableServers[sender].HeartBeat = unixTimestamp();
            }
        }

        public string GetServer()
        {
            var minTimestamp = unixTimestamp() - 6;
            lock (this.availableServers)
            {
                var worker = (from server in availableServers.Values
                              where server.Working == false && server.HeartBeat > minTimestamp
                              orderby server.HeartBeat descending
                              select server).FirstOrDefault();
                if (worker != null)
                {
                    Random rnd = new Random();
                    var job = String.Format("job-{0}", rnd.Next());
                    worker.RemoteRef.SetNextJob(job);
                    worker.Working = true;
                    worker.CurrentId = job;
                    return String.Format("{0}/api/tk/{1}",worker.WsUri, job);
                }
                else return null;
            }
            
        }

    }

    public class WorkerServer
    {
        public String TcpUri { get; set; }
        public String WsUri { get; set; }
        public bool Working { get; set; }
        public String CurrentId { get; set; }
        public Int32 HeartBeat { get; set; }
        public IServer RemoteRef { get; set; }
    }
}
