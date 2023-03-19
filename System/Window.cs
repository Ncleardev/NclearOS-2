using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using System.Threading;

namespace NclearOS2
{
    public abstract class Window
    {
        public string name;
        public int id;
        public uint usageRAM;
        public int x;
        public int y;
        public int StartWindowX = 70;
        public int StartWindowY = 70;
        public int StartWindowXOld;
        public int StartWindowYOld;
        public bool windowlock;
        public Bitmap icon = Resources.program;
        public bool minimized;
        public Window(string name, int x, int y, Bitmap icon)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.icon = icon;
            this.StartWindowX = ProcessManager.running.Count * 20 + 50;
            this.StartWindowY = ProcessManager.running.Count * 20 + 50;
            this.StartWindowXOld = ProcessManager.running.Count * 20 + 50;
            this.StartWindowYOld = ProcessManager.running.Count * 20 + 50;
            this.windowlock = false;
        }
        internal abstract bool Start();
        internal abstract bool Update(int StartX, int StartY, int x, int y);
        internal abstract int Stop();
        internal abstract void Key(ConsoleKeyEx key);
    }
}