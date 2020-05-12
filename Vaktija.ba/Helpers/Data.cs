using System;
using System.Globalization;
using System.IO;

namespace Vaktija.ba.Helpers
{
    class Data
    {
        public static Data data { get; set; }

        public Vaktija vaktija { get; set; }
        public System.Collections.Generic.List<Differences> differences { get; set; }
        public System.Collections.Generic.List<string> locations { get; set; }
        public System.Collections.Generic.List<string> locationsDative { get; set; }
        public System.Collections.Generic.List<string> locationsShort { get; set; }
        public System.Collections.Generic.List<string> vakatNames { get; set; }
        public System.Collections.Generic.List<int> weights { get; set; }
        public System.Collections.Generic.List<string> weekdays { get; set; }
        public System.Collections.Generic.List<string> monthnames { get; set; }
        public System.Collections.Generic.List<string> hijrimonthnames { get; set; }
        public System.Collections.Generic.List<Location> coordinates { get; set; }


        public static DateTime SecondsToTime(DateTime dt, int sec)
        {
            System.DateTime rd = new System.DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            rd = rd.AddSeconds(sec);

            if (rd.IsDaylightSavingTime())
                rd = rd.AddSeconds(3600);

            return rd;
        }

        public static DateTime VakatTime(DateTime date, int rbr)
        {
            int v_def = data.vaktija.months[date.Month - 1].days[date.Day - 1].vakat[rbr];
            int v_dif = data.differences[Helpers.Memory.location].months[date.Month - 1].vakat[rbr];
            int v_rez = v_def + v_dif;

            return SecondsToTime(date, v_rez);
        }

        public static Vakat GetCurrentPrayer(DateTime date)
        {
            Vakat tmp = new Vakat { time = VakatTime(date, 0), name = data.vakatNames[0], rbr = 0 };

            for (int i = 0; i < data.vakatNames.Count; i++)
            {
                if (VakatTime(date, i) > date)
                    return tmp;
                else
                    tmp = new Vakat { time = VakatTime(date, i), name = data.vakatNames[i], rbr = i };
            }

            return new Vakat { time = VakatTime(date, 0), name = data.vakatNames[0], rbr = 0 };
        }

        public static Vakat GetNextPrayer(DateTime date)
        {
            for (int i = 0; i < data.vakatNames.Count; i++)
                if (VakatTime(date, i) > date)
                    return new Vakat { time = VakatTime(date, i), name = data.vakatNames[i], rbr = i };

            return new Vakat { time = VakatTime(date, data.vakatNames.Count - 1), name = data.vakatNames[data.vakatNames.Count - 1], rbr = data.vakatNames.Count - 1 };
        }

        public enum Obavijest { All, Alarm, Toast }

        public static System.Globalization.CultureInfo HijriFormat = new System.Globalization.CultureInfo("ar-SA");

        public static async System.Threading.Tasks.Task<string> Read_Data_To_String(string uri)
        {
            try
            {
                Windows.Storage.StorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));
                using (StreamReader sRead = new StreamReader(await file.OpenStreamForReadAsync()))
                {
                    return await sRead.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static System.Collections.Generic.List<T> JsonToArray<T>(string jsonString)
        {
            if (jsonString[0] != '[')
                jsonString = "[" + jsonString + "]";

            try
            {
                System.Collections.Generic.List<T> trs = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<T>>(jsonString);
                return trs;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("greska: " + ex.Message);
                return new System.Collections.Generic.List<T>();
            }
        }

        public static string Hijri_Date_To_String(DateTime datum)
        {
            return datum.ToString("d.", HijriFormat) + " " + data.hijrimonthnames[new UmAlQuraCalendar().GetMonth(datum) % 12] + " " + datum.ToString("yyyy.", HijriFormat);
        }
    }

    class Vaktija
    {
        public System.Collections.Generic.List<Months> months { get; set; }
    }
    class Months
    {
        public System.Collections.Generic.List<Days> days { get; set; }
    }
    class Days
    {
        public System.Collections.Generic.List<int> vakat { get; set; }
    }

    class Differences
    {
        public System.Collections.Generic.List<DMonths> months { get; set; }
    }
    class DMonths
    {
        public System.Collections.Generic.List<int> vakat { get; set; }
    }

    class Vakat
    {
        private System.DateTime timeParam;
        public System.DateTime time
        {
            get
            {
                if(Memory.Std_Podne && rbr == 2)
                {
                    if (timeParam.IsDaylightSavingTime()) return new System.DateTime(timeParam.Year, timeParam.Month, timeParam.Day, 13, 0, 0);
                    else return new System.DateTime(timeParam.Year, timeParam.Month, timeParam.Day, 12, 0, 0);
                }
                else
                {
                    return timeParam;
                }
            }
            set
            {
                timeParam = value;
            }
        }

        public int rbr { get; set; }

        public string name { get; set; }

        public bool blockedNotifications(Data.Obavijest obavijest)
        {
            if (obavijest == Data.Obavijest.Alarm) return blockedAlarm;
            else if (obavijest == Data.Obavijest.Toast) return blockedToast;
            else return true;
        }
        public bool blockedAlarm
        {
            get
            {
                return Memory.alarmBlocked[rbr];
            }
            set
            {
                var tmp = Memory.alarmBlocked;
                tmp[rbr] = value;
                Memory.alarmBlocked = tmp;
            }
        }
        public bool blockedToast
        {
            get
            {
                return Memory.toastBlocked[rbr];
            }
            set
            {
                var tmp = Memory.toastBlocked;
                tmp[rbr] = value;
                Memory.toastBlocked = tmp;
            }
        }

        public int notificationsOffset(Data.Obavijest obavijest)
        {
            if (obavijest == Data.Obavijest.Alarm) return predAlarm;
            else if (obavijest == Data.Obavijest.Toast) return predToast;
            else return 0;
        }
        public int predAlarm
        {
            get
            {
                return Memory.predAlarm[rbr];
            }
            set
            {
                var tmp = Memory.predAlarm;
                tmp[rbr] = value;
                Memory.predAlarm = tmp;
            }
        }
        public int predToast
        {
            get
            {
                return Memory.predToast[rbr];
            }
            set
            {
                var tmp = Memory.predToast;
                tmp[rbr] = value;
                Memory.predToast = tmp;
            }
        }

        public string predAlarm_String
        {
            get
            {
                return (predAlarm / 60).ToString("00") + ":" + (predAlarm % 60).ToString("00");
            }
        }
        public string predToast_String
        {
            get
            {
                return (predToast / 60).ToString("00") + ":" + (predToast % 60).ToString("00");
            }
        }

        public bool isCurrent
        {
            get
            {
                int a = rbr + 1;
                bool nd = false;
                if (a >= Data.data.vakatNames.Count)
                {
                    nd = true;
                    a = 0;
                }
                Vakat nv = new Vakat { time = Data.VakatTime(time, a), rbr = a, name = Data.data.vakatNames[a] };
                if (nd) nv.time = Data.VakatTime(nv.time.AddDays(1), a);

                if (System.DateTime.Now > time && System.DateTime.Now < nv.time) return true;
                else return false;
            }
        }
        public bool isNext
        {
            get
            {
                int a = rbr - 1;
                bool pd = false;
                if (a < 0)
                {
                    pd = true;
                    a = Data.data.vakatNames.Count - 1;
                }
                Vakat pv = new Vakat { time = Data.VakatTime(time, a), rbr = a, name = Data.data.vakatNames[a] };
                if (pd) pv.time = Data.VakatTime(pv.time.AddDays(-1), a);

                if (System.DateTime.Now < time && System.DateTime.Now > pv.time) return true;
                else return false;
            }
        }

        public bool isSkipped(Data.Obavijest obavijest)
        {
            if (obavijest == Data.Obavijest.Alarm)
                return Memory.skippedAlarms.Contains("[" + MemoryKey + "]");
            else if (obavijest == Data.Obavijest.Toast)
                return Memory.skippedToasts.Contains("[" + MemoryKey + "]");
            else
                return true;
        }

        public void skipNotification(Data.Obavijest obavijest)
        {
            if (obavijest == Data.Obavijest.Alarm)
            {
                string tmp = Memory.skippedAlarms;
                tmp = tmp.Replace("[" + MemoryKey + "]", "");
                tmp += "[" + MemoryKey + "]";
                Memory.skippedAlarms = tmp;
            }
            else if (obavijest == Data.Obavijest.Toast)
            {
                string tmp = Memory.skippedToasts;
                tmp = tmp.Replace("[" + MemoryKey + "]", "");
                tmp += "[" + MemoryKey + "]";
                Memory.skippedToasts = tmp;
            }
            Notification.Obrisi_Obavijest(this.NotificationKey(obavijest));
        }

        public void stayNotification(Data.Obavijest obavijest)
        {
            if (obavijest == Data.Obavijest.Alarm)
                Memory.skippedAlarms = Memory.skippedAlarms.Replace("[" + MemoryKey + "]", "");
            else if (obavijest == Data.Obavijest.Toast)
                Memory.skippedToasts = Memory.skippedToasts.Replace("[" + MemoryKey + "]", "");

            Notification.Kreiraj_Novu_Obavijest(this, obavijest);
        }

        public string NotificationKey(Data.Obavijest obavijest)
        {
            return (time.ToString("yy|MM|dd") + "|" + name.Substring(0, 2).ToLower() + "|" + (int)obavijest).ToLower();
        }

        public string MemoryKey
        {
            get
            {
                return (time.ToString("yy|MM|dd") + "|" + name.Substring(0, 2)).ToLower();
            }
        }
    }

    class Location
    {
        public string ime { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}