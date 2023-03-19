using Cosmos.Core;
using Cosmos.System;
using System;
using System.Collections.Generic;

namespace NclearOS2
{
    public static class ProcessManager
    {
        public static List<Window> running = new();
        public static List<Window> windows = new();
        public static List<Service> services = new();
        public static void Refresh()
        {
            for (int i = running.Count; i > 0; i--)
            {
                uint j = GCImplementation.GetUsedRAM() / 1024;
                if (!running[i - 1].minimized) { if (!Update(i - 1)) { continue; } }
                running[i - 1].usageRAM = GCImplementation.GetUsedRAM() / 1024 - j;
            }
        }
        public static void RefreshService()
        {
            if (Kernel.Lock)
            {
                for (int i = 0; i < services.Count; i++)
                {
                    if (services[i].allowOnLockScreen) { UpdateService(i); }
                }
            }
            else
            {
                for (int i = 0; i < services.Count; i++)
                {
                    UpdateService(i);
                }
            }

        }
        public static bool Update(int i)
        {
            try
            {
                if (!running[i].Update(running[i].StartWindowX, running[i].StartWindowY + 30, running[i].x, running[i].y)) { RemoveAt(i); return false; }
                running[i].id = i;
                Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, running[i].StartWindowX, running[i].StartWindowY, running[i].x, 30);
                Kernel.canvas.DrawString(running[i].name, Kernel.font, Kernel.WhitePen, running[i].StartWindowX + 36, running[i].StartWindowY + 10);
                Kernel.canvas.DrawImageAlpha(running[i].icon, running[i].StartWindowX + 5, running[i].StartWindowY + 3);
                Kernel.canvas.DrawImageAlpha(Resources.minimize, running[i].StartWindowX - 50 + running[i].x, running[i].StartWindowY + 7);
                Kernel.canvas.DrawImageAlpha(Resources.close, running[i].StartWindowX - 20 + running[i].x, running[i].StartWindowY + 7);
                if (running[i].windowlock)
                {
                    if (Kernel.LongPress)
                    {
                        running[i].StartWindowX = running[i].StartWindowXOld + (int)MouseManager.X;
                        running[i].StartWindowY = running[i].StartWindowYOld + (int)MouseManager.Y;
                        if (running[i].StartWindowY < 0) { running[i].StartWindowY = 0; }
                        else if (running[i].StartWindowY > Kernel.screenY - 60) { running[i].StartWindowY = (int)Kernel.screenY - 60; }
                        if (running[i].StartWindowX < 0) { running[i].StartWindowX = 0; }
                        else if (running[i].StartWindowX + running[i].x > Kernel.screenX) { running[i].StartWindowX = (int)Kernel.screenX - running[i].x; }
                    }
                    else
                    {
                        running[i].windowlock = false;
                    }
                }
                else if (MouseManager.Y > running[i].StartWindowY && MouseManager.Y < running[i].StartWindowY + 30)
                {
                    if (MouseManager.X > running[i].StartWindowX - 23 + running[i].x && MouseManager.X < running[i].StartWindowX + 1 + running[i].x)
                    {
                        Kernel.canvas.DrawImageAlpha(Resources.closered, running[i].StartWindowX - 20 + running[i].x, running[i].StartWindowY + 7);
                        if (Kernel.Pressed) { RemoveAt(i); return false; }
                    }
                    else if (MouseManager.X > running[i].StartWindowX - 53 + running[i].x && MouseManager.X < running[i].StartWindowX - 23 + running[i].x)
                    {
                        Kernel.canvas.DrawImageAlpha(Resources.minimize2, running[i].StartWindowX - 50 + running[i].x, running[i].StartWindowY + 7);
                        if (Kernel.Pressed) { Animation.hideWindow = (running[i].name, (float)running[i].StartWindowX, (float)running[i].StartWindowY, (float)running[i].x, running[i].icon); running[i].minimized = true; return false; }
                    }
                    else if (Kernel.StartClick && MouseManager.X > running[i].StartWindowX && MouseManager.X < running[i].StartWindowX + running[i].x - 53)
                    {
                        running[i].StartWindowXOld = running[i].StartWindowX - (int)MouseManager.X;
                        running[i].StartWindowYOld = running[i].StartWindowY - (int)MouseManager.Y;
                        running[i].windowlock = true;
                        FocusAtWindow(i);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Msg.Main("Error", "App '" + running[i].name + "' crashed: " + e, Resources.error); RemoveAt(i, true);
                return false;
            }
        }
        public static void UpdateService(int i)
        {
            try
            {
                if (!services[i].Update()) { RemoveAtService(i); }
            }
            catch (Exception e)
            {
                Msg.Main("Error", "Service '" + services[i].name + "' crashed: " + e, Resources.error); services.RemoveAt(i);
            }
        }
        public static void Run(Service service)
        {
            if (service.Start())
            {
                services.Add(service);
                services[services.Count - 1].id = services.Count - 1;
            }
            else { Msg.Main("Process Manager", "Service " + service.name + " started but then stopped", Resources.warn); }
        }
        public static void Add(Window window)
        {
            Kernel.Loading = true;
            Kernel.Refresh();
            if (window.Start())
            {
                Animation.newWindow = window;
            }
            Kernel.Loading = false;
        }
        public static void RemoveAt(int i, bool force = false)
        {
            int exitCode = running[i].Stop();
            if (force || exitCode == 0) { if (!running[i].minimized) { Animation.hideWindow = (running[i].name, (float)running[i].StartWindowX, (float)running[i].StartWindowY, (float)running[i].x, running[i].icon); } running.RemoveAt(i); }
            else if (exitCode != 1) { Msg.Main("Process Manager", "App '" + running[i].name + " exited with unexcepted code " + exitCode, Resources.warn); running.RemoveAt(i); }
        }
        public static void RemoveAtService(int i, bool force = false)
        {
            int exitCode = services[i].Stop();
            if (force || exitCode == 0) { services.RemoveAt(i); }
            else if (exitCode != 1) { Msg.Main("Process Manager", "Service '" + services[i].name + " exited with unexcepted code " + exitCode, Resources.warn); services.RemoveAt(i); }
        }
        public static void FocusAtWindow(int i)
        {
            if (running[i].minimized) { Animation.unMinimize = i; }
            MoveToTop(running, i);
        }
        static void MoveToTop<T>(this List<T> list, int index)
        {
            T item = list[index];
            for (int i = index; i > 0; i--)
                list[i] = list[i - 1];
            list[0] = item;
        }
    }
    public class TaskManager : Window
    {
        public TaskManager() : base("Task Manager", 300, 300, Resources.program) { }
        public static List<string> taskList = new();
        private int click = -1;
        private int selY;
        private int oclick = -1;
        internal override bool Start()
        {
            return true;
        }
        internal override bool Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkGrayPen, StartX, StartY, x, y);
            taskList.Clear();
            taskList.Add("");
            for (int i = 0; i < ProcessManager.running.Count; i++)
            {
                taskList.Add(ProcessManager.running[i].id + " | " + ProcessManager.running[i].name + " | " + ProcessManager.running[i].usageRAM + " KB");
            }
            taskList.Add("");
            for (int i = 0; i < ProcessManager.services.Count; i++)
            {
                taskList.Add(ProcessManager.services[i].id + " | " + ProcessManager.services[i].name);
            }
            (click, oclick, selY) = Graphic.Listview(taskList, StartX + 4, StartY + 4, 300, 300, click, oclick, selY);
            if (Graphic.Button("End task", StartX + x - 80, StartY + y - 30)) { if (oclick < ProcessManager.running.Count) { ProcessManager.RemoveAt(oclick - 1, true); } else { ProcessManager.RemoveAtService(oclick - 3, true); } selY = 0; }
            return true;
        }
        internal override int Stop() { return 0; }
        internal override void Key(ConsoleKeyEx keyEx) { }
    }
}