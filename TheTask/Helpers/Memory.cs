using System;
using System.Collections.Generic;

namespace BackgroundTask.Helpers
{
    class Memory
    {
        public static Windows.Storage.ApplicationDataContainer local = Windows.Storage.ApplicationData.Current.LocalSettings;

        public static bool Reset()
        {
            local.Values.Clear();
            return First_Time();
        }
        public static bool First_Time()
        {
            if (local.Values.ContainsKey("memory")) return false;

            local.Values["memory"] = "true";

            alarmBlocked = new List<bool> { true, false, true, true, true, true, };
            predAlarm = new List<int> { 30, 30, 30, 30, 30, 30 };
            toastBlocked = new List<bool> { true, false, false, false, false, false };
            predToast = new List<int> { 15, 15, 15, 15, 15, 15 };

            location = 77;

            Date_Show_In_App = true;
            Hijri_Date_In_App = false;
            Std_Podne = false;
            Theme = 1; // 0-Default; 1-Light; 2-Dark;

            Alarm_Sound = "ms-appx:///Assets/Sounds/Beep-Beep-Beep.mp3";
            Toast_Sound = "ms-appx:///Assets/Sounds/BeepBeep.mp3";

            Live_Tile = true;

            return true;
        }

        public static List<int> predAlarm
        {
            get
            {
                if (local.Values.ContainsKey("predAlarm"))
                {
                    string s = local.Values["predAlarm"].ToString();
                    string[] sp = new string[] { "|" };
                    string[] ss = s.Split(sp, System.StringSplitOptions.RemoveEmptyEntries);
                    List<int> a = new List<int>();
                    foreach (var it in ss)
                    {
                        a.Add(int.Parse(it));
                    }
                    return a;
                }
                else
                {
                    predAlarm = new List<int> { 30, 30, 30, 30, 30, 30 };
                    return new List<int> { 30, 30, 30, 30, 30, 30 };
                }
            }
            set
            {
                string s = "";
                foreach (var it in value)
                {
                    s += it + "|";
                }
                local.Values["predAlarm"] = s;
            }
        }
        public static List<int> predToast
        {
            get
            {
                if (local.Values.ContainsKey("predToast"))
                {
                    string s = local.Values["predToast"].ToString();
                    string[] sp = new string[] { "|" };
                    string[] ss = s.Split(sp, System.StringSplitOptions.RemoveEmptyEntries);
                    List<int> a = new List<int>();
                    foreach (var it in ss)
                    {
                        a.Add(int.Parse(it));
                    }
                    return a;
                }
                else
                {
                    predToast = new List<int> { 15, 15, 15, 15, 15, 15 };
                    return new List<int> { 15, 15, 15, 15, 15, 15 };
                }
            }
            set
            {
                string s = "";
                foreach (var it in value)
                {
                    s += it + "|";
                }
                local.Values["predToast"] = s;
            }
        }

        public static List<bool> alarmBlocked
        {
            get
            {
                if (local.Values.ContainsKey("alarmBlocked"))
                {
                    string s = local.Values["alarmBlocked"].ToString();
                    string[] sp = new string[] { "|" };
                    string[] ss = s.Split(sp, System.StringSplitOptions.RemoveEmptyEntries);
                    List<bool> a = new List<bool>();
                    foreach (var it in ss)
                    {
                        a.Add(Convert.ToBoolean(it));
                    }
                    return a;
                }
                else
                {
                    alarmBlocked = new List<bool> { true, false, true, true, true, true, };
                    return new List<bool> { true, false, true, true, true, true, };
                }
            }
            set
            {
                string s = "";
                foreach (var it in value)
                {
                    s += it.ToString() + "|";
                }
                local.Values["alarmBlocked"] = s;
            }
        }
        public static List<bool> toastBlocked
        {
            get
            {
                if (local.Values.ContainsKey("toastBlocked"))
                {
                    string s = local.Values["toastBlocked"].ToString();
                    string[] sp = new string[] { "|" };
                    string[] ss = s.Split(sp, System.StringSplitOptions.RemoveEmptyEntries);
                    List<bool> a = new List<bool>();
                    foreach (var it in ss)
                    {
                        a.Add(Convert.ToBoolean(it));
                    }
                    return a;
                }
                else
                {
                    toastBlocked = new List<bool> { true, false, false, false, false, false };
                    return new List<bool> { true, false, false, false, false, false };
                }
            }
            set
            {
                string s = "";
                foreach (var it in value)
                {
                    s += it.ToString() + "|";
                }
                local.Values["toastBlocked"] = s;
            }
        }

        public static string skippedAlarms
        {
            get
            {
                if (local.Values.ContainsKey("skippedAlarms"))
                {
                    return local.Values["skippedAlarms"].ToString();
                }
                else
                {
                    skippedAlarms = "[]";
                    return "[]";
                }
            }
            set
            {
                local.Values["skippedAlarms"] = value;
            }
        }
        public static string skippedToasts
        {
            get
            {
                if (local.Values.ContainsKey("skippedToasts"))
                {
                    return local.Values["skippedToasts"].ToString();
                }
                else
                {
                    skippedToasts = "[]";
                    return "[]";
                }
            }
            set
            {
                local.Values["skippedToasts"] = value;
            }
        }

        public static int location
        {
            get
            {
                if (local.Values.ContainsKey("Location_Id"))
                {
                    return (int)local.Values["Location_Id"];
                }
                else
                {
                    return 77;
                }
            }
            set
            {
                local.Values["Location_Id"] = value;
            }
        }
        public static bool Std_Podne
        {
            get
            {
                if (local.Values.ContainsKey("Std_Podne"))
                    return (bool)local.Values["Std_Podne"];
                else
                {
                    Std_Podne = false;
                    return true;
                }
            }
            set
            {
                local.Values["Std_Podne"] = value;
            }
        }
        public static int Theme
        {
            get
            {
                if (local.Values.ContainsKey("Theme"))
                    return (int)local.Values["Theme"];
                else
                {
                    Theme = 1;
                    return 1;
                }
            }
            set
            {
                local.Values["Theme"] = value;
            }
        }
        public static string Alarm_Sound
        {
            get
            {
                if (local.Values.ContainsKey("Alarm_Sound"))
                    return local.Values["Alarm_Sound"].ToString();
                else
                {
                    Alarm_Sound = "ms-appx:///Assets/Sounds/Beep-Beep-Beep.mp3";
                    return "ms-appx:///Assets/Sounds/Beep-Beep-Beep.mp3";
                }
            }
            set
            {
                local.Values["Alarm_Sound"] = value;
            }
        }
        public static string Toast_Sound
        {
            get
            {
                if (local.Values.ContainsKey("Toast_Sound"))
                    return local.Values["Toast_Sound"].ToString();
                else
                {
                    Toast_Sound = "ms-appx:///Assets/Sounds/BeepBeep.mp3";
                    return "ms-appx:///Assets/Sounds/BeepBeep.mp3";
                }
            }
            set
            {
                local.Values["Toast_Sound"] = value;
            }
        }
        public static bool Date_Show_In_App
        {
            get
            {
                if (local.Values.ContainsKey("Date_Show_In_App"))
                    return (bool)local.Values["Date_Show_In_App"];
                else
                {
                    Date_Show_In_App = true;
                    return true;
                }
            }
            set
            {
                local.Values["Date_Show_In_App"] = value;
            }
        }
        public static bool Hijri_Date_In_App
        {
            get
            {
                if (local.Values.ContainsKey("Hijri_Date_In_App"))
                    return (bool)local.Values["Hijri_Date_In_App"];
                else
                {
                    Hijri_Date_In_App = false;
                    return false;
                }
            }
            set
            {
                local.Values["Hijri_Date_In_App"] = value;
            }
        }
        public static bool Live_Tile
        {
            get
            {
                if (local.Values.ContainsKey("Live_Tile"))
                    return (bool)local.Values["Live_Tile"];
                else
                {
                    Live_Tile = true;
                    return true;
                }
            }
            set
            {
                local.Values["Live_Tile"] = value;
            }
        }
    }
}
