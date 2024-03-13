using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using NclearOS2.Commands;
using NclearOS2.GUI;
using System.Drawing;
using System.Linq;

namespace NclearOS2
{
    public class Date
    {
        public static string CurrentDate(bool DisplayWeekday, bool DisplayYear)
        {
            string Weekday = "";
            string Month = "";
            if (DisplayWeekday)
            {
                switch (RTC.DayOfTheWeek)
                {
                    case 1:
                        Weekday = "Monday, ";
                        break;
                    case 2:
                        Weekday = "Tuesday, ";
                        break;
                    case 3:
                        Weekday = "Wednesday, ";
                        break;
                    case 4:
                        Weekday = "Thursday, ";
                        break;
                    case 5:
                        Weekday = "Friday, ";
                        break;
                    case 6:
                        Weekday = "Saturday, ";
                        break;
                    case 7:
                        Weekday = "Sunday, ";
                        break;
                }
            }
            switch (RTC.Month)
            {
                case 1:
                    Month = "January";
                    break;
                case 2:
                    Month = "February";
                    break;
                case 3:
                    Month = "March";
                    break;
                case 4:
                    Month = "April";
                    break;
                case 5:
                    Month = "May";
                    break;
                case 6:
                    Month = "June";
                    break;
                case 7:
                    Month = "July";
                    break;
                case 8:
                    Month = "August";
                    break;
                case 9:
                    Month = "September";
                    break;
                case 10:
                    Month = "October";
                    break;
                case 11:
                    Month = "November";
                    break;
                case 12:
                    Month = "December";
                    break;

            }
            if (DisplayYear)
            {
                if (DisplayWeekday)
                {
                    return Weekday + RTC.DayOfTheMonth + "." + RTC.Month + ".20" + RTC.Year;
                }
                else
                {
                    return RTC.DayOfTheMonth + "." + RTC.Month + ".20" + RTC.Year;
                }
            }
            else
            {
                if (DisplayWeekday)
                {
                    return Weekday + RTC.DayOfTheMonth + " " + Month;
                }
                else
                {
                    return RTC.DayOfTheMonth + " " + Month;
                }
            }
        }
        public static string CurrentTime(bool DisplaySeconds)
        {
            string hour = RTC.Hour.ToString();
            if (hour.Length == 1) { hour = "0" + hour; }
            string min = RTC.Minute.ToString();
            if (min.Length == 1) { min = "0" + min; }
            return DisplaySeconds ? hour + ":" + min + ":" + CurrentSecond() : hour + ":" + min;
        }
        public static string CurrentSecond()
        {
            return RTC.Second.ToString().Length == 1 ? "0" + RTC.Second.ToString() : RTC.Second.ToString();
        }
    }
}
namespace NclearOS2.GUI
{
    internal class Date : Window
    {
        internal Date() : base("Date", 220, 50, Icons.program, ProcessManager.Priority.High, WindowManager.Resizable.None) { }
        internal override int Start()
        {
            MemoryOperations.Fill(appCanvas.rawData, Color.Black.ToArgb());
            DrawString(NclearOS2.Date.CurrentTime(true), Color.LimeGreen.ToArgb(), Color.Black.ToArgb(), x - (x / 2) - 35, 10);
            DrawString(NclearOS2.Date.CurrentDate(true, true), Color.White.ToArgb(), Color.Black.ToArgb(), x - (x / 2) - 100, 25);
            return 0;
        }
        internal override void Update()
        {
            DrawString(NclearOS2.Date.CurrentTime(true), Color.LimeGreen.ToArgb(), Color.Black.ToArgb(), x - (x / 2) - 35, 10);
            DrawString(NclearOS2.Date.CurrentDate(true, true), Color.White.ToArgb(), Color.Black.ToArgb(), x - (x / 2) - 100, 25);
        }


    }
}
namespace NclearOS2.Commands
{
    internal class Date : CommandsTree
    {
        internal Date() : base
            ("Date", "Displays the system time and date.",
            new Command[] {
            new Command(new string[] { "time", "date" }, "Displays the current system time and date.", new string[] {"/t - display only time","/d - display only date"})
            })
        {
        }
        internal override int Execute(string[] args, CommandShell shell, string rawInput)
        {
            if (args[0] == "time" || args[0] == "date")
            {
                bool noDate = false;
                bool noTime = false;
                foreach (string arg in args.Skip(1))
                {
                    if (arg == "/t")
                    {
                        noDate = true;
                    }
                    else if (arg == "/d")
                    {
                        noTime = true;
                    }
                }
                string str = "";
                if (!noDate) { str += NclearOS2.Date.CurrentDate(true, true) + " "; }
                if (!noTime) { str += NclearOS2.Date.CurrentTime(true); }
                shell.Print = str;
                return 0;
            }
            return 1;
        }
    }
}