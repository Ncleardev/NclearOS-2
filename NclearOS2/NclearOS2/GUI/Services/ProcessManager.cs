using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace NclearOS2.GUI
{
    public static class ProcessManager
    {
        internal static List<Process> running = new();
        static short second = -1;
        static char second10;
        public static void Refresh()
        {
            bool lowpriority = false;
            bool highpriority = false;
            if(RTC.Second != second)
            {
                highpriority = true;
                second = RTC.Second;
            }
            if (NclearOS2.Date.CurrentSecond()[0] != second10)
            {
                lowpriority = true;
                second10 = NclearOS2.Date.CurrentSecond()[0];
            }
            for (int i = running.Count; i > 0; i--)
            {
                uint j = GCImplementation.GetUsedRAM() / 1024;

                running[i - 1].id = i - 1;

                if (running[i - 1] is Window w)
                {
                    if (!w.minimized) { WindowManager.Draw(w); }
                }
                if(running[i - 1].priority == Process.Priority.Realtime) { Update(i - 1); }
                if (highpriority && running[i - 1].priority == Process.Priority.High) { Update(i - 1); }
                if (lowpriority && running[i - 1].priority == Process.Priority.Low) { Update(i - 1); }

                running[i - 1].usageRAM = GCImplementation.GetUsedRAM() / 1024 - j;
            }
        }
        public static void Update(int id)
        {
            try { running[id].Update(); }
            catch (Exception e) { Msg.Main("Error", "Process '" + running[id].name + "' crashed: " + e, Icons.error); RemoveAt(id, true); }
        }
        internal static void Run(Process process)
        {
            GUI.Loading = true;
            GUI.Refresh();
            process.id = 0;
            int code = process.Start();
            if(code == -1) { GUI.Loading = false; return; }
            else if (code != 0) { Msg.Main("Process Manager", "Process '" + process.name + " launched and then stopped with an unexpected error code: " + code, Icons.warn); GUI.Loading = false; return; }
            GUI.Refresh();
            running.Insert(0, process);
            running[0].id = 0;
            GUI.Loading = false;
        }
        public static int RemoveAt(int i, bool force = false)
        {
            int exitCode = running[i].Stop();
            //GCImplementation.Free(running[i]);
            if (force || exitCode == 0) { running.RemoveAt(i); }
            else if (exitCode != -1) {
                Msg.Main("Process Manager", "Process '" + running[i].name + " stopped with an unexpected error code: " + exitCode, Icons.warn); running.RemoveAt(i);
            }
            return exitCode;
        }
        public static string StopAll()
        {
            GUI.Loading = true;
            GUI.Refresh();
            string result = null;
            for (int i = 0; i < running.Count; i++)
            {
                if (RemoveAt(i) == -1)
                {
                    if (result == null)
                    {
                        result = running[i].name;
                    }
                    else { result += "; " + running[i].name; }
                }
                GUI.Refresh();
            }
            GUI.Loading = false;
            return result;
        }
    }
    public static class WindowManager
    {
        internal static void Draw(Window w)
        {
            if (w.windowlock)
            {
                if (GUI.LongPress)
                {
                    w.StartX = w.StartXOld + (int)MouseManager.X;
                    w.StartY = w.StartYOld + (int)MouseManager.Y;
                    if (w.StartY < 0) { w.StartY = 0; }
                    else if (w.StartY > GUI.displayMode.Rows - 60) { w.StartY = (int)GUI.displayMode.Rows - 60; }
                    if (w.StartX < 0) { w.StartX = 0; }
                    else if (w.StartX + w.x > GUI.displayMode.Columns) { w.StartX = (int)GUI.displayMode.Columns - w.x; }
                }
                else
                {
                    w.windowlock = false;
                    w.borderCanvas = PostProcess.CropBitmap(Images.wallpaperBlur, w.StartX, w.StartY, w.x, 30);
                    Font.DrawString(w.name, Color.White.ToArgb(), 36, 10, w.borderCanvas.rawData, w.x);
                    Font.DrawImageAlpha(w.icon, 5, 3, w.borderCanvas.rawData, w.x);
                    Font.DrawImageAlpha(Icons.minimize, w.x - 50, 7, w.borderCanvas.rawData, w.x);
                    Font.DrawImageAlpha(Icons.close, w.x - 20, 7, w.borderCanvas.rawData, w.x);
                    w.OnMoved?.Invoke();
                }
                GUI.canvas.DrawImage(w.borderCanvas, w.StartX, w.StartY);
            }
            else
            {
                GUI.canvas.DrawImage(w.borderCanvas, w.StartX, w.StartY);
                if (MouseManager.Y > (w.StartY) && MouseManager.Y < w.StartY + 30)
                {
                    if (MouseManager.X > w.StartX - 23 + w.x && MouseManager.X < w.StartX + 1 + w.x)
                    {
                        GUI.canvas.DrawImageAlpha(Icons.close2, w.StartX - 20 + w.x, w.StartY + 7);
                        if (GUI.Pressed) { ProcessManager.RemoveAt(w.id); return; }
                    }
                    else if (MouseManager.X > w.StartX - 53 + w.x && MouseManager.X < w.StartX - 23 + w.x)
                    {
                        GUI.canvas.DrawImageAlpha(Icons.minimize2, w.StartX - 50 + w.x, w.StartY + 7);
                        if (GUI.Pressed) { w.minimized = true; }
                    }
                    else if (GUI.StartClick && MouseManager.X > w.StartX && MouseManager.X < w.StartX + w.x - 53)
                    {
                        w.StartXOld = w.StartX - (int)MouseManager.X;
                        w.StartYOld = w.StartY - (int)MouseManager.Y;
                        w.windowlock = true;
                        MemoryOperations.Fill(w.borderCanvas.rawData, 0);
                        Font.DrawString(w.name, Color.White.ToArgb(), 36, 10, w.borderCanvas.rawData, w.x);
                        Font.DrawImageAlpha(w.icon, 5, 3, w.borderCanvas.rawData, w.x);
                        Font.DrawImageAlpha(Icons.minimize, w.x - 50, 7, w.borderCanvas.rawData, w.x);
                        Font.DrawImageAlpha(Icons.close, w.x - 20, 7, w.borderCanvas.rawData, w.x);
                        FocusAtWindow(w.id);
                        w.OnStartMoving?.Invoke();
                    }
                }
            }
            GUI.canvas.DrawImage(w.appCanvas, w.StartX, w.StartY + 30);
        }
        public static void FocusAtWindow(int id)
        {
            if (ProcessManager.running[id] is Window w)
            {
                w.minimized = false;
                MoveToTop(ProcessManager.running, id);
            }
        }
        public static void MoveToTop<T>(this List<T> list, int index)
        {
            T item = list[index];
            for (int i = index; i > 0; i--)
                list[i] = list[i - 1];
            list[0] = item;
        }
    }
    internal class TaskManager : Window
    {
        internal TaskManager() : base("Process Manager", 300, 500, new Bitmap(Resources.TaskmngIcon), Priority.High) { }
        internal override int Start()
        {
            Background(GUI.DarkGrayPen.ValueARGB);
            return 0;
        }
        internal override void Update()
        {
            Background(GUI.DarkGrayPen.ValueARGB);
            int i = 0;
            foreach(var task in ProcessManager.running)
            {
                i++;
                DrawString(task.name + "   " + task.usageRAM + " KB", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, i * 20);
            }
            int ram = (int)(NclearOS2.Sysinfo.usedRAM / NclearOS2.Sysinfo.installedRAM * 100);
            DrawFilledRectangle(Color.DarkGray.ToArgb(), 0, y - 20, x, 20);
            if (ram > 100) { ram = 100; }
            DrawFilledRectangle(GUI.SystemPen.ValueARGB, 0, y - 20, ram*(x/100), 20);
            DrawStringAlpha("RAM: " + ram + "%", Color.White.ToArgb(), 10, y - 15);
            
        }
        internal override int Stop() { return 0; }
    }
}