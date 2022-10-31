using System;

namespace NclearOS2
{
    public static class Process
    {
        public static bool console;
        public static bool settings;
        public static bool notepad;
        public static bool files;
        public static bool sysinfo;
        public static void Reset()
        {
            console = false;
            settings = false;
            notepad = false;
            files = false;
            sysinfo = false;
        }
        public static void UpdateCanvas(int x, int y)
        {
            if (console) { try { ConsoleApp.Update(Window.StartWindowX, Window.StartWindowY + 30, x, y); } catch (Exception e) { console = false; Window.display = false; Msg.Main("App Console crashed: " + e, true); } }
            if (settings) { try { Settings.Update(Window.StartWindowX, Window.StartWindowY + 30, x, y); } catch (Exception e) { settings = false; Window.display = false; Msg.Main("App Settings crashed: " + e, true); } }
            if (notepad) { try { Notepad.Update(Window.StartWindowX, Window.StartWindowY + 30, x, y); } catch (Exception e) { notepad = false; Window.display = false; Msg.Main("App Notepad crashed: " + e, true); } }
            if (files) { try { Files.Update(Window.StartWindowX, Window.StartWindowY + 30, x, y); } catch (Exception e) { files = false; Window.display = false; Msg.Main("App Files crashed: " + e, true); } }
            if (sysinfo) { try { Sysinfo.Update(Window.StartWindowX, Window.StartWindowY + 30, x, y); } catch (Exception e) { sysinfo = false; Window.display = false; Msg.Main("App System Info crashed: " + e, true); } }
        }
    }
}