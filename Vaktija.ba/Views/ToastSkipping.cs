using Vaktija.ba.Helpers;

namespace Vaktija.ba.Views
{
    class ToastSkipping
    {
        public static bool IsSkipped(Vakat vakat)
        {
            return Memory.skippedToasts.Contains("[" + Vakat.Key(vakat) + "]");
        }
        public static void SkipToast(Vakat vakat)
        {
            string tmp = Memory.skippedToasts;
            tmp = tmp.Replace("[" + Vakat.Key(vakat) + "]", "");
            tmp += "[" + Vakat.Key(vakat) + "]";
            Memory.skippedToasts = tmp;
            Year.year.months[vakat.time.Month - 1].days[vakat.time.Day - 1].vakti[vakat.rbr].toastSkipped = true;
        }
        public static void StayToast(Vakat vakat)
        {
            Memory.skippedToasts = Memory.skippedToasts.Replace("[" + Vakat.Key(vakat) + "]", "");
            Year.year.months[vakat.time.Month - 1].days[vakat.time.Day - 1].vakti[vakat.rbr].toastSkipped = false;
        }
    }
}
