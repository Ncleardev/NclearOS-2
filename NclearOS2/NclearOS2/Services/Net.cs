using Cosmos.HAL;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;

namespace NclearOS2
{
    public class Net
    {
        public static string GetInfo()
        {
            return "IP Address: " + NetworkConfiguration.CurrentAddress.ToString();
        }
        public static string Ping(string target)
        {
            return "Not implemented yet";
        }
    }
}
namespace NclearOS2.GUI
{
    internal class Net : Process
    {
        public Net() : base("Network Service", Priority.Low, true) { }
        internal override int Start()
        {
            NetworkDevice nic = NetworkDevice.GetDeviceByName("eth0");
            IPConfig.Enable(nic, new Address(192, 168, 0, 1), new Address(255, 255, 255, 0), new Address(192, 168, 1, 254));
            if (NetworkConfiguration.CurrentAddress != null)
            {
                Kernel.useNetwork = true;
            }
            return 0;
        }
        internal override void Update()
        {
            Kernel.useNetwork = NetworkConfiguration.CurrentAddress != null;
        }
        internal override int Stop() { return 0; }
    }
}
namespace NclearOS2.Commands
{
    internal class Network : CommandsTree
    {
        internal Network() : base
            ("Network", "Manages network.",
            new Command[] {
            new Command(new string[] {"net"}, "Displays current Network configuration."),
            new Command(new string[] {"ping"}, "Pings a specified target.", new string[] { "[target] - target to ping"})
            })
        {
        }
        internal override int Execute(string[] args, CommandShell shell)
        {
            switch (args[0])
            {
                case "net":
                    shell.print = Net.GetInfo();
                    return 0;
                case "ping":
                    shell.print = Net.Ping(args[1]);
                    return 0;
            }
            return 1;
        }
    }
}