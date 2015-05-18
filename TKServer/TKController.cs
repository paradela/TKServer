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
        public String Get()
        {
            Master master = Master.Singleton;
            string url = master.GetServer();
            return url;
        }
    }
}
