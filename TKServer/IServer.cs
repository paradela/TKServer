using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKServer
{
    public interface IServer
    {
        void SetNextJob(String id);
    }
}
