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

        //Keeps the last local timestamp of each server heartbeat. URL -> Timestamp
        private Dictionary<string, Int32> heartBeats = new Dictionary<string, Int32>();

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

        public uint RegisterServer(string url)
        {
            throw new NotImplementedException();
        }

        public bool RemoveServer(string url)
        {
            throw new NotImplementedException();
        }

        public void JobFinished(string id)
        {
            throw new NotImplementedException();
        }

        public void Ping(string sender, string msg)
        {
            lock (this.heartBeats)
            {
                this.heartBeats[sender] = unixTimestamp();
            }
        }

        public string GetServer()
        {
            return "ws://localhost/ws/abs12345";
        }

    }
}
