using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Card4B;

namespace TKServer
{
    [RoutePrefix("api/tk")]
    public class TKController : ApiController
    {
        [Route("server")]
        public object Get()
        {
            Master master = Master.Singleton;
            string url = master.GetServer();
            
            return new { URL = url };
        }

        [Route("server/execute")]
        public object Post([FromBody] String id, [FromBody] String tkmsg, [FromBody] String card)
        {
            IList<CTSWriteOperation> operations;
            String tkmsgout;
            RemoteServer server = RemoteServer.Singleton;
            //server.RunCommand(id, tkmsg, CardOperations: operations);
            server.RunCommand(id, tkmsg, out tkmsgout, out operations, card);

            return new { tkmsg_out = tkmsgout, card_messages = operations };
        }
    }
}
