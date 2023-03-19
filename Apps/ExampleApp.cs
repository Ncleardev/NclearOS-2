using Cosmos.System;
using System;
using System.Collections.Generic;

namespace NclearOS2
{
    public class AppExample : Window
    {
        public static List<string> RAWlistresult = new();
        private int click;
        private int selY;
        private int dclick = -1;
        public AppExample() : base("App Example", 300, 100, Resources.program) { }
        internal override bool Start()
        {
            RAWlistresult.Add("wow");
            RAWlistresult.Add("wow2");
            RAWlistresult.Add("wow3");
            return true;
        }
        internal override bool Update(int StartX, int StartY, int x, int y)
        {
            (click, dclick, selY) = Graphic.Listview(RAWlistresult, StartX + 10, StartY + 10, 280, 80, click, dclick, selY);
            return true;
        }
        internal override int Stop() { return 0; }
        internal override void Key(ConsoleKeyEx key) { }
    }
}