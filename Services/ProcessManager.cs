using Cosmos.System;
using System;

namespace NclearOS2
{
    public static class Process
    {
        public enum Apps
        {
            none,
            console,
            settings,
            notepad,
            files,
            sysinfo,
            date
        }
        public static Apps currentApp = Apps.none;
        
        public static void UpdateCanvas(int x, int y)
        {
            try
            {
                switch (currentApp)
                {
                    case Apps.console:
                        ConsoleApp.Update(Window.StartWindowX, Window.StartWindowY + 30, x, y);
                        break;
                    case Apps.settings:
                        Settings.Update(Window.StartWindowX, Window.StartWindowY + 30, x, y);
                        break;
                    case Apps.notepad:
                        Notepad.Update(Window.StartWindowX, Window.StartWindowY + 30, x, y);
                        break;
                    case Apps.files:
                        Files.Update(Window.StartWindowX, Window.StartWindowY + 30, x, y);
                        break;
                    case Apps.sysinfo:
                        Sysinfo.Update(Window.StartWindowX, Window.StartWindowY + 30, x, y);
                        break;
                    case Apps.date:
                        Date.Update(Window.StartWindowX, Window.StartWindowY + 30, x, y);
                        break;
                }
            }
            catch (Exception e)
            {
                Msg.Main("App " + currentApp + " crashed: " + e, true);
                Window.display = false;
            }
            
        }
        public static void Run(Apps app)
        {
            Kernel.Loading = true;
            Input.Inputing = false;
            currentApp = app;
            switch (app)
            {
                case Apps.console:
                    Window.Init("Console", (int)(Kernel.screenX - 250), (int)(Kernel.screenY - 170), Kernel.console);
                    Input.Register(true);
                    break;
                case Apps.settings:
                    Window.Init("Settings", 300, 300, Kernel.settings);
                    break;
                case Apps.notepad:
                    Window.Init("Notepad", (int)(Kernel.screenX - 150), (int)(Kernel.screenY - 170), Kernel.notepad);
                    Input.Register(false);
                    break;
                case Apps.files:
                    Window.Init("Files", (int)(Kernel.screenX - 150), (int)(Kernel.screenY - 170), Kernel.filesicon);
                    Files.ListDisks();
                    break;
                case Apps.sysinfo:
                    Window.Init("System Info", 500, 200, Kernel.sysinfo);
                    break;
                case Apps.date:
                    Window.Init("Date", 220, 50, Kernel.program);
                    break;
            }
            Kernel.Loading = false;
        }
    }
}