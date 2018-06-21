using BackgroundTask.Helpers;

namespace BackgroundTask.Views
{
    class AlarmSkipping
    {
        public static bool IsSkipped(Vakat vakat)
        {
            return Memory.skippedAlarms.Contains("[" + Vakat.Key(vakat) + "]");
        }
        public static void SkipAlarm(Vakat vakat)
        {
            string tmp = Memory.skippedAlarms;
            tmp = tmp.Replace("[" + Vakat.Key(vakat) + "]", "");
            tmp += "[" + Vakat.Key(vakat) + "]";
            Memory.skippedAlarms = tmp;
            Year.year.months[vakat.time.Month - 1].days[vakat.time.Day - 1].vakti[vakat.rbr].alarmSkipped = true;
        }
        public static void StayAlarm(Vakat vakat)
        {
            Memory.skippedAlarms = Memory.skippedAlarms.Replace("[" + Vakat.Key(vakat) + "]", "");
            Year.year.months[vakat.time.Month - 1].days[vakat.time.Day - 1].vakti[vakat.rbr].alarmSkipped = false;
        }
    }
}
