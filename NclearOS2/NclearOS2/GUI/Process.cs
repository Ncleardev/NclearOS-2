using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;

namespace NclearOS2.GUI
{
    internal abstract class Process
    {
        internal Process(string name, ProcessManager.Priority priority)
        {
            this.Name = name;
            this.priority = priority;
        }
        internal int ID { get { return ProcessManager.running.IndexOf(this); } }
        internal uint usageRAM;
        internal string Name;
        internal string name
        {
            get { return Name; }
            set
            {
                if(Name != value)
                {
                    Name = value;
                    if(this is Window w) { w.RefreshBorder(); }
                }
                
            }
        }
        internal ProcessManager.Priority priority;

        public override string ToString() { return name; }

        internal virtual int Start() { return 0; } // returns 0: OK; returns -1: cancel launch
        internal virtual void Update() { }
        internal virtual int Stop(bool force = false) { return 0; } // returns 0: OK; returns 1: deny stopping;
        internal void Notify(string text, Bitmap icon = null)
        {
            NotificationSystem.Notify(name, text, icon);
        }
        internal int Exit(bool force = false)
        {
            return ProcessManager.RemoveAt(ID, force);
        }
    }
}