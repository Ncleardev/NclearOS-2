using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using System.Threading;

namespace NclearOS2
{
    public abstract class Service
    {
        public string name;
        public int id;
        public bool allowOnLockScreen;
        public enum Priority //to be implemented
        {
            Low, //every 10 Kernel refreshes
            Medium, //every 5 Kernel refreshes
            High, //every 2 Kernel refreshes
            Realtime //every Kernel refresh
        }
        public Service(string name, Priority priority, bool allowOnLockScreen = false)
        {
            this.allowOnLockScreen = allowOnLockScreen;
            this.name = name;
        }
        internal abstract bool Start();
        internal abstract bool Update();
        internal abstract int Stop();
    }
}