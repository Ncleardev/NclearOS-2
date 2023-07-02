using Cosmos.System;
using System;

namespace NclearOS2.GUI
{
    internal abstract class Process
    {
        internal Process(string name, Priority priority, bool system = false)
        {
            this.name = name;
            this.priority = priority;
        }
        internal int id;
        internal uint usageRAM;
        internal string name;
        internal enum Priority //how often Update() method is called
        {
            None,
            Low, //every 10s
            High, //every second
            Realtime //every Kernel refresh
        }
        internal Priority priority;
        internal abstract int Start(); //returns: 0 - ok, -1 - silent exit/proceess handles displaying message by itself, any other code - unexpected exit
        internal abstract void Update();
        internal abstract int Stop(); //returns: 0 - ok, -1 - silent exit/proceess handles displaying message by itself, any other code - unexpected exit
    }
}