using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKServer
{
    public interface IMaster
    {
        void RegisterServer(String Address, short RemotingPort, short WsPort);
        bool RemoveServer(String url);

        void JobFinished(String id);
        void Ping(string sender, string msg);
    }
}
