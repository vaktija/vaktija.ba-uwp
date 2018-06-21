using System;
using BackgroundTask.Views;
using System.IO;

namespace BackgroundTask.Helpers
{
    class Get
    {
        public static Vakat Next_Prayer_Time()
        {
            Day today = Year.year.months[DateTime.Now.Month - 1].days[DateTime.Now.Day - 1];

            foreach (var it in today.vakti)
            {
                if (it.time > DateTime.Now) return it;
            }

            return Year.year.months[DateTime.Now.AddDays(1).Month - 1].days[DateTime.Now.AddDays(1).Day - 1].vakti[0];
        }
        public static Vakat Current_Prayer_Time()
        {
            Day today = Year.year.months[DateTime.Now.Month - 1].days[DateTime.Now.Day - 1];

            for (int i = today.vakti.Count - 1; i >= 0; i--)
            {
                if (today.vakti[i].time < DateTime.Now) return today.vakti[i];
            }

            Day yesterday = Year.year.months[DateTime.Now.AddDays(-1).Month - 1].days[DateTime.Now.AddDays(-1).Day - 1];
            return yesterday.vakti[yesterday.vakti.Count - 1];
        }

        public static async System.Threading.Tasks.Task<string> Read_Data_To_String()
        {
            string text = "";
            Windows.Storage.StorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Files/data.txt"));
            using (StreamReader sRead = new StreamReader(await file.OpenStreamForReadAsync()))
                text = await sRead.ReadToEndAsync();
            return text;
        }

        public static string Name_Of_Day_In_Week(int a)
        {
            string[] sedmica = new string[] { "Nedjelja", "Ponedjeljak", "Utorak", "Srijeda", "Četvrtak", "Petak", "Subota" };
            return sedmica[a & 7];
        }
        public static string Name_Of_Month(int a)
        {
            string[] mjeseci = new string[] { "Decembar", "Januar", "Februar", "Mart", "April", "Maj", "Juni", "Juli", "August", "Septembar", "Oktobar", "Novembar" };
            return mjeseci[a % 12];
        }
        public static int Number_Of_Days_In_Month(int a, int y)
        {
            if (a == 1)
                return 31;
            if (a == 2)
            {
                if (y % 4 == 0) return 29;
                else return 28;
            }
            if (a == 3)
                return 31;
            if (a == 4)
                return 30;
            if (a == 5)
                return 31;
            if (a == 6)
                return 30;
            if (a == 7)
                return 31;
            if (a == 8)
                return 31;
            if (a == 9)
                return 30;
            if (a == 10)
                return 31;
            if (a == 11)
                return 30;

            return 31;
        }
        public static string Name_Of_Month_Hijri(int a)
        {
            string[] mjeseci = new string[] { "Zu-l-ka'de", "Muharrem", "Safer", "Rebiul-evvel", "Rebiul-ahir", "Džumade-l-ula", "Džumade-l-ahira", "Redžeb", "Ša'ban", "Ramazan", "Ševval", "Zu-l-ka'de" };
            return mjeseci[a % 12];
        }
        public static string Minutes_Double_Format(double a)
        {
            int h = (int)(a / 60);
            double m = a % 60;
            return h.ToString("00") + ":" + m.ToString("00");
        }
        public static string Minutes_Double_Format_In_String(double a)
        {
            int h = (int)(a / 60);
            double m = a % 60;
            return h.ToString("00") + "h " + m.ToString("00") + "m";
        }
        public static double Difference_Between_Times(System.DateTime dt1, System.DateTime dt2)
        {
            return (dt2 - dt1).TotalMinutes;
        }
    }
}
