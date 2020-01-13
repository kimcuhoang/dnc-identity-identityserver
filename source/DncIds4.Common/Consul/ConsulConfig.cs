using System;

namespace DncIds4.Common.Consul
{
    public class ConsulConfig
    {
        public bool Enabled { get; set; }
        public string Url { get; set; }
        public string ServiceName { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public bool PingEnabled { get; set; }
        public string PingEndpoint { get; set; }
        public int PingInterval { get; set; }

        public Uri ConsulServiceDiscoveryUrl => new Uri(this.Url);
    }
}
