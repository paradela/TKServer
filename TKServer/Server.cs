﻿using System;
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
    public class RemoteServer : MarshalByRefObject, IServer
    {
        private static RemoteServer server = null;

        private string JobId { get; set; }

        private RemoteServer() {}

        public static RemoteServer Singleton
        {
            get
            {
                if (server == null)
                    server = new RemoteServer();
                return server;
            }
        }

        public void SetNextJob(String id)
        {
            JobId = id;
        }

        public void RunCommand(String Id, IList<String> TKCmds, ExAPDU RdrCallback)
        {
            if (JobId != Id) return;
            //Call TK
            Console.WriteLine("Cenas");
        }

    }

    public class ServerConfig
    {
        public uint Id { get; set; }
        public String MasterAddress { get; set; }
        public short MasterWsPort { get; set; }
        public short MasterRemotingPort { get; set; }
        public bool LocalMaster { get; set; }

        public bool LocalServer { get; set; }
        public String ServerAddress { get; set; }
        public short ServerRemotingPort { get; set; }
        public short ServerWsPort { get; set; }
        public String ServerSAMPort { get; set; }
        public String ServerUri
        {
            get
            {
                return String.Format("tcp://{0}:{1}/Server", 
                    (ServerAddress == "")? MasterAddress : ServerAddress, 
                    (ServerRemotingPort == 0)? MasterRemotingPort : ServerRemotingPort);
            }
        }
        public String MasterUri
        {
            get
            {
                return String.Format("tcp://{0}:{1}/Master", MasterAddress, MasterRemotingPort);
            }
        }
    }

    public class Server
    {
        public bool LoadConfig { get; set; }

        public bool LocalMaster { get; set; }
        public string MasterAddress { get; set; }
        public short MasterRemotingPort { get; set; }
        public short MasterWsPort { get; set; }

        public uint ServerId { get; set; }
        public string ServerAddress { get; set; }
        public short ServerWsPort { get; set; }
        public short ServerRemotingPort { get; set; }
        private IMaster RemoteMaster { get; set; }
        private string Uri { get; set; }


        public void InitServer(String MasterUri, String ServerUri)
        {
            this.RemoteMaster = (IMaster)Activator.GetObject(typeof(IMaster), MasterUri);
            try
            {
                RemoteMaster.RegisterServer(ServerUri);

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

        private TcpChannel StartServers(IList<ServerConfig> serverList)
        {
            TcpChannel channel;
            ServerConfig localMaster = (from srv in serverList
                                  where srv.LocalMaster == true
                                  select srv).FirstOrDefault();
            ServerConfig localServer = (from srv in serverList
                                  where srv.LocalServer == true
                                  select srv).FirstOrDefault();

            channel = new TcpChannel((localMaster != null)? localMaster.MasterRemotingPort : 
                (localServer != null)? localServer.ServerRemotingPort : 0);
            ChannelServices.RegisterChannel(channel, false);

            if (localMaster != null)
            {
                Master masterServer = TKServer.Master.Singleton;
                RemotingServices.Marshal(masterServer, "Master", typeof(Master));
            }
            if(localServer != null) 
            {
                RemoteServer remoteServer = RemoteServer.Singleton;
                RemotingServices.Marshal(remoteServer, "Server", typeof(RemoteServer));
                InitServer(localServer.MasterUri, localServer.ServerUri);
            }

            foreach(ServerConfig server in (from srv in serverList where srv.LocalServer != true && srv.LocalMaster != true select srv)) 
            {
                //Lauch processess
            }

            return channel;
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
                tmp.LocalMaster = serverConfig.ServerEndpoints.LocalMaster;

                foreach (ServerElement endpointElement in serverConfig.ServerEndpoints)
                {
                    ServerConfig server = new ServerConfig();
                    server.MasterAddress = tmp.MasterAddress;
                    server.MasterRemotingPort = tmp.MasterRemotingPort;
                    server.MasterWsPort = tmp.MasterWsPort;
                    server.LocalMaster = tmp.LocalMaster;
                    server.Id = endpointElement.Id;
                    server.ServerAddress = endpointElement.Address;
                    server.ServerRemotingPort = endpointElement.RemotingPort;
                    server.ServerWsPort = endpointElement.WsPort;
                    server.ServerSAMPort = endpointElement.SamPort;
                    server.LocalServer = endpointElement.LocalServer;
                    serverList.Add(server);
                }
            }
            return serverList;
        }

        public static void Main(string[] args)
        {
            Server server = new Server();
            server.Uri = "tcp://localhost:8080/Server";
            TcpChannel channel;
            IList<ServerConfig> serverList;

            //TODO: Remove
            server.LoadConfig = true;
            server.ServerRemotingPort = 8080;

            if (server.LoadConfig)
            {
                serverList = server.LoadConfigFile();
                channel = server.StartServers(serverList);
            }



            if (server.LocalMaster)
            {
                channel = new TcpChannel(server.MasterRemotingPort);
                ChannelServices.RegisterChannel(channel, false);
            }
                
            server.InitServer("", server.Uri);
            

            //TODO: remove
            server.ServerAddress = "localhost";
            server.ServerWsPort = 80;
            string baseAddress = String.Format("http://{0}:{1}", server.ServerAddress, server.ServerWsPort);
            IDisposable webServer = WebApp.Start<Startup>(url: baseAddress);            
            Console.WriteLine("Press any key to exit..");
            Console.ReadLine();
            webServer.Dispose();
        }
    }
}