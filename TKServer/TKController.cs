using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Card4B;

namespace TKServer
{
    public class ProductLoad
    {
        public String tkmsg { get; set; }
        public String card { get; set; }
    }

    public class TKController : ApiController
    {
        public object Get()
        {
            Master master = Master.Singleton;
            string url = master.GetServer();

            
            return new { URL = url };
        }

        public object Post([FromUri] String id, [FromBody] ProductLoad body)
        {
            IList<CTSWriteOperation> operations;
            String tkmsgout;
            RemoteServer server = RemoteServer.Singleton;
            server.RunCommand("", "", out tkmsgout, out operations, "");

            return new { tkmsg_out = tkmsgout, card_messages = operations };
        }
    }
}
