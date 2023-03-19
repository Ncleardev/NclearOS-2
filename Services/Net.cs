using Cosmos.HAL;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;

namespace NclearOS2
{
    public class Net : Service
    {
        public Net() : base("Network Service", Priority.Realtime) { }
        internal override bool Start()
        {
            NetworkDevice nic = NetworkDevice.GetDeviceByName("eth0");
            IPConfig.Enable(nic, new Address(192, 168, 1, 69), new Address(255, 255, 255, 0), new Address(192, 168, 1, 254));
            if(!NetworkConfiguration.CurrentAddress.ToString().Contains("implemented"))
            {
                Kernel.network = true;
            }
            return true;
        }
        internal override bool Update()
        {
            return true;
        }
        internal override int Stop() { return 0; }
        public static string GetInfo()
        {
            return "IP Address: " + NetworkConfiguration.CurrentAddress.ToString();
        }
        public static string Ping(string i)
        {
            return "Not implemented yet";
        }
    }
}