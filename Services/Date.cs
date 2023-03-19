using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;

namespace NclearOS2
{
    public class Date : Window
    {
        public Date() : base("Date", 220, 50, Resources.program) { }
        internal override bool Start() { return true; }
        internal override bool Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkPen, StartX, StartY, x, y);
            Kernel.canvas.DrawString(CurrentTime(true), Kernel.font, Kernel.GreenPen, StartX + x - (x/2)-50, StartY+10);
            Kernel.canvas.DrawString(CurrentDate(true, true), Kernel.font, Kernel.WhitePen, StartX + x - (x / 2) - 100, StartY+25);
            return true;
        }
        public static string CurrentDate(bool DisplayWeekday, bool DisplayYear)
        {
            string Weekday = "";
            string Month = "";
            if (DisplayWeekday)
            {
                switch (RTC.DayOfTheWeek)
                {
                    case 1:
                        Weekday = "Monday";
                        break;
                    case 2:
                        Weekday = "Tuesday";
                        break;
                    case 3:
                        Weekday = "Wednesday";
                        break;
                    case 4:
                        Weekday = "Thursday";
                        break;
                    case 5:
                        Weekday = "Friday";
                        break;
                    case 6:
                        Weekday = "Saturday";
                        break;
                    case 7:
                        Weekday = "Sunday";
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
                    return Weekday + ", " + RTC.DayOfTheMonth + "." + RTC.Month + ".20" + RTC.Year;
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
                    return Weekday + ", " + RTC.DayOfTheMonth + " " + Month;
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
            if (DisplaySeconds)
            {
                string sec = RTC.Second.ToString();
                if (sec.Length == 1) { sec = "0" + sec; }
                return hour + ":" + min + ":" + sec;
            }
            else
            {
                return hour + ":" + min;
            }
        }
        internal override int Stop() { return 0; }
        internal override void Key(ConsoleKeyEx key) { }
    }
}