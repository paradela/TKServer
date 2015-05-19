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
            RemoteServer server = RemoteServer.Singleton;
            server.RunCommand(id, tkmsg, card);
            return new { tkmsg_out = "<tkmsg><complete></complete></tkmsg>", card_messages = "{ \"messages\"= []" };
        }
    }
}
