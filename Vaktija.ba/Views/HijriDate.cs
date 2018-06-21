using System;
using System.Globalization;

namespace Vaktija.ba.Views
{
    class HijriDate
    {
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }

        public static HijriDate Get(DateTime dt)
        {
            HijriDate hd = new HijriDate();

            CultureInfo arSA = new CultureInfo("ar-SA");

            DateTime gr = new DateTime(2016, 7, 4);

            if (dt > gr) dt = dt.AddDays(1);

            hd.day = int.Parse(dt.ToString("dd", arSA));
            hd.month = int.Parse(dt.ToString("MM", arSA));
            hd.year = int.Parse(dt.ToString("yyyy", arSA));

            return hd;
        }
    }
}
