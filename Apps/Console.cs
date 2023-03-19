using Cosmos.System;

namespace NclearOS2
{
    public class ConsoleApp : Window
    {
        private Input inputInstance;
        private CommandExecute commandHandler = new CommandExecute();

        public ConsoleApp(int x, int y) : base("Console", x, y, Resources.console) { }
        internal override bool Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkPen, StartX, StartY, x, y);
            Kernel.canvas.DrawChar('>', Kernel.font, Kernel.WhitePen, StartX+10, StartY + 10);
            Graphic.EditText(inputInstance, StartX + 25, StartY + 10, Kernel.WhitePen, true);
            if (inputInstance.ready)
            {
                commandHandler.Main(inputInstance.input);
                inputInstance.ready = false;
                inputInstance.input = null;
            }
            Graphic.TextView(commandHandler.result, StartX + 10, StartY + 34, Kernel.WhitePen);
            return true;
        }
        internal override bool Start()
        {
            inputInstance = new Input("Console", true);
            ProcessManager.Run(inputInstance);
            inputInstance.input = null;
            return true;
        }
        internal override int Stop() { ProcessManager.RemoveAtService(inputInstance.id); return 0; }
        internal override void Key(ConsoleKeyEx keyEx) { }
    }
}