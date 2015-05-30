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
        public Card card { get; set; }
    }

    public class Card
    {
        public String type { get; set; }
        public String data { get; set; }
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
            uint tkresult, tkstatus;

            RemoteServer server = RemoteServer.Singleton;
            server.RunCommand(id, body.tkmsg, out tkstatus, out tkresult, out tkmsgout, out operations, body.card);

            return new { status = tkstatus, result = tkresult ,msg = tkmsgout, card_messages = operations };
        }
    }
}
