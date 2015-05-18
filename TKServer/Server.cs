using System;
using Microsoft.Owin.Hosting;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using TKServer.Config;
using System.Configuration;
using System.Linq;

namespace TKServer
{
    public class ServerConfig
    {
        public uint Id { get; set; }
        public String MasterAddress { get; set; }
        public short MasterWsPort { get; set; }
        public short MasterRemotingPort { get; set; }

        public bool MasterServer { get; set; }
        public String ServerAddress { get; set; }
        public short ServerRemotingPort { get; set; }
        public short ServerWsPort { get; set; }

        public String ServerSAMPort { get; set; }
        public String ServerTcpUri
        {
            get
            {
                return String.Format("tcp://{0}:{1}/Server", 
                    ServerAddress, 
                    ServerRemotingPort);
            }
        }
        public String MasterTcpUri
        {
            get
            {
                return String.Format("tcp://{0}:{1}/Master", MasterAddress, MasterRemotingPort);
            }
        }
        public String ServerWSUri
        {
            get
            {
                return String.Format("http://{0}:{1}/ws/",
                    ServerAddress,
                    ServerWsPort);
            }
        }
    }

    public class Server
    {
        private IMaster RemoteMaster { get; set; }
        private string Uri { get; set; }

        TcpChannel channel;


        public void InitServer(String MasterUri, String Address, short RemotingPort, short WsPort)
        {
            this.RemoteMaster = (IMaster)Activator.GetObject(typeof(IMaster), MasterUri);
            try
            {
                RemoteMaster.RegisterServer(Address, RemotingPort, WsPort);

                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = 3000;
                timer.Elapsed += delegate { Heartbeat(); };
                timer.Start();
            }
            catch (RemotingException re)
            {
                System.Console.WriteLine("Unable to connect to master: {0}",
                        re.Message);
            }
        }

        public void Heartbeat()
        {
            try
            {
                this.RemoteMaster.Ping(this.Uri, "Ping?");
            }
            catch (RemotingException re)
            {
                System.Console.WriteLine("Cannot connect to master: {0}\nExiting.", re.Message);
                //Try to become master
            }
        }

        private IDisposable StartServers(IList<ServerConfig> serverList, int id = -1)
        {
            IDisposable webServer = null;
            if (id < 0)
            {
                var master = (from srv in serverList
                              where srv.MasterServer
                              select srv).FirstOrDefault();

                if (master != null)
                {
                    channel = new TcpChannel(master.MasterRemotingPort);
                    ChannelServices.RegisterChannel(channel, false);
                    Master masterServer = TKServer.Master.Singleton;
                    RemotingServices.Marshal(masterServer, "Master", typeof(Master));
                }
            }

            IEnumerable<ServerConfig> list;
            if (id > 0)
                list = from srv in serverList
                       where srv.Id == id
                       select srv;
            else list = serverList;

            foreach(ServerConfig server in list) 
            {
                //Lauch processess
                if (id < 0 && server.Id != 0)
                {
                    //start process with id
                    System.Diagnostics.Process.Start("TKServer.exe", "" + server.Id);
                }
                else //launch local
                {
                    if (id > 0)
                    {
                        channel = new TcpChannel(server.ServerRemotingPort);
                        ChannelServices.RegisterChannel(channel, false);
                    }
                    RemoteServer remoteServer = RemoteServer.Singleton;
                    RemotingServices.Marshal(remoteServer, "Server", typeof(RemoteServer));
                    InitServer(server.MasterTcpUri, server.ServerAddress, server.ServerRemotingPort, server.ServerWsPort);
                    webServer = WebApp.Start<Startup>(url: server.ServerWSUri);
                    this.Uri = server.ServerTcpUri;
                }
            }

            return webServer;
        }

        private IList<ServerConfig> LoadConfigFile()
        {
            IList<ServerConfig> serverList = new List<ServerConfig>();
            var serverConfig = ConfigurationManager.GetSection(TKServerConfigHandler.SectionName) as TKServerConfigHandler;

            if (serverConfig != null)
            {
                ServerConfig tmp = new ServerConfig();
                tmp.MasterAddress = serverConfig.ServerEndpoints.Address;
                tmp.MasterRemotingPort = serverConfig.ServerEndpoints.RemotingPort;
                tmp.MasterWsPort = serverConfig.ServerEndpoints.WsPort;

                foreach (ServerElement endpointElement in serverConfig.ServerEndpoints)
                {
                    ServerConfig server = new ServerConfig();
                    server.MasterAddress = tmp.MasterAddress;
                    server.MasterRemotingPort = tmp.MasterRemotingPort;
                    server.MasterWsPort = tmp.MasterWsPort;
                    server.Id = endpointElement.Id;
                    server.ServerAddress = (endpointElement.Address == "") ? server.MasterAddress = tmp.MasterAddress : endpointElement.Address;//optional
                    server.ServerRemotingPort = (endpointElement.RemotingPort == 0)? server.MasterRemotingPort : endpointElement.RemotingPort;//optional
                    server.ServerWsPort = (endpointElement.WsPort == 0)? server.MasterWsPort : endpointElement.WsPort;//optional
                    server.MasterServer = endpointElement.MasterServer;//optional
                    server.ServerSAMPort = endpointElement.SamPort;
                    serverList.Add(server);
                }
            }
            return serverList;
        }

        public static void Main(string[] args)
        {
            Server server = new Server();
            int Id = -1;
            IList<ServerConfig> serverList;
            IDisposable webServer = null;

            if (args.Length == 1)
            {
                Id = Int32.Parse(args[0]);
            }

            serverList = server.LoadConfigFile();
            webServer = server.StartServers(serverList, Id);
                        
            Console.WriteLine("Press any key to exit..");
            Console.ReadLine();
            webServer.Dispose();
        }
    }
}
