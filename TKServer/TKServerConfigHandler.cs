using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKServer.Config
{
    public class TKServerConfigHandler : ConfigurationSection
    {
        /// <summary>
        /// The name of this section in the app.config.
        /// </summary>
        public const string SectionName = "tkserver";

        private const string EndpointCollectionName = "master";

        [ConfigurationProperty(EndpointCollectionName)]
        [ConfigurationCollection(typeof(MasterCollection), AddItemName = "server")]
        public MasterCollection ServerEndpoints { get { return (MasterCollection)base[EndpointCollectionName]; } }
    }

    public class MasterCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("address", IsRequired = true)]
        public string Address
        {
            get { return (String)this["address"]; }
            set { this["address"] = value; }
        }

        [ConfigurationProperty("wsPort", IsRequired = true)]
        public short WsPort
        {
            get { return (short)this["wsPort"]; }
            set { this["wsPort"] = value; }
        }

        [ConfigurationProperty("remotingPort", IsRequired = true)]
        public short RemotingPort
        {
            get { return (short)this["remotingPort"]; }
            set { this["remotingPort"] = value; }
        }

        [ConfigurationProperty("local", DefaultValue = false, IsRequired = false)]
        public bool LocalMaster
        {
            get { return (bool)this["local"]; }
            set { this["local"] = value; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServerElement)element).Id;
        }
    }

    public class ServerElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public uint Id
        {
            get { return (uint)this["id"]; }
            set { this["id"] = value; }
        }

        [ConfigurationProperty("local", DefaultValue = false, IsRequired = false)]
        public bool LocalServer
        {
            get { return (bool)this["local"]; }
            set { this["local"] = value; }
        }

        [ConfigurationProperty("address", IsRequired = false, DefaultValue = "")]
        public string Address
        {
            get { return (string)this["address"]; }
            set { this["address"] = value; }
        }

        [ConfigurationProperty("wsPort", IsRequired = false, DefaultValue = 0)]
        public short WsPort
        {
            get { return (short)this["wsPort"]; }
            set { this["wsPort"] = value; }
        }

        [ConfigurationProperty("remotingPort", IsRequired = false, DefaultValue = 0)]
        public short RemotingPort
        {
            get { return (short)this["remotingPort"]; }
            set { this["remotingPort"] = value; }
        }

        [ConfigurationProperty("samPort", IsRequired = true)]
        public string SamPort
        {
            get { return (string)this["samPort"]; }
            set { this["samPort"] = value; }
        }
    }
}