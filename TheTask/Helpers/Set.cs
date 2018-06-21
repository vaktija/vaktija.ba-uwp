using System;
using BackgroundTask.Views;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BackgroundTask.Helpers
{
    class Set
    {
        public static void Group_Notifications(int broj_dana, int od = 0, bool obnovi = true, int rbrv = 6, int AorT = 0, bool firstDel = false)
        {
            System.Diagnostics.Debug.WriteLine("Učitavanje podataka. Lokacija " + Memory.location.ime);

            if (obnovi)
                Year.Set();

            for (int i = od; i <= broj_dana; i++)
            {
                Day dan = Year.year.months[DateTime.Now.AddDays(i).Month - 1].days[DateTime.Now.AddDays(i).Day - 1];
                Notifications_On_Day(dan, rbrv, AorT, firstDel);
            }

            Last_Notification(Year.year.months[DateTime.Now.AddDays(broj_dana).Month - 1].days[DateTime.Now.AddDays(broj_dana).Day - 1]);
        }
        public static void Notifications_On_Day(Day day, int rbrv, int AorT, bool firstDel)
        {
            if (rbrv < 6)
            {
                try
                {
                    if (AorT == 0)
                    {
                        if (!Memory.alarmBlocked[rbrv])
                        {
                            if (!day.vakti[rbrv].alarmSkipped)
                                Notification.Create_New_Alarm(day.vakti[rbrv], firstDel);
                            else
                                Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "alarm"));
                        }
                        else
                            Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "alarm"));

                        if (!Memory.toastBlocked[rbrv])
                        {
                            if (!day.vakti[rbrv].toastSkipped)
                                Notification.Create_New_Toast(day.vakti[rbrv], firstDel);
                            else
                                Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "toast"));
                        }
                        else
                            Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "toast"));
                    }
                    else if (AorT == 1)
                    {
                        if (!Memory.alarmBlocked[rbrv])
                        {
                            if (!day.vakti[rbrv].alarmSkipped)
                                Notification.Create_New_Alarm(day.vakti[rbrv], firstDel);
                            else
                                Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "alarm"));
                        }
                        else
                            Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "alarm"));

                    }
                    else if (AorT == 2)
                    {
                        if (!Memory.toastBlocked[rbrv])
                        {
                            if (!day.vakti[rbrv].toastSkipped)
                                Notification.Create_New_Toast(day.vakti[rbrv], firstDel);
                            else
                                Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "toast"));
                        }
                        else
                            Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "toast"));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Greška pri registraciji grupe notifikacija " + "(" + ex.Message + ")");
                }
            }
            else //All
            {
                foreach (var it in day.vakti)
                {
                    try
                    {
                        if (AorT == 0)
                        {
                            if (!Memory.alarmBlocked[it.rbr])
                            {
                                if (!it.alarmSkipped)
                                    Notification.Create_New_Alarm(it, firstDel);
                                else
                                    Notification.Delete(Notification.Create_Key(it, "alarm"));
                            }
                            else
                            {
                                Notification.Delete(Notification.Create_Key(it, "alarm"));
                            }

                            if (!Memory.toastBlocked[it.rbr])
                            {
                                if (!it.toastSkipped)
                                    Notification.Create_New_Toast(it, firstDel);
                                else
                                    Notification.Delete(Notification.Create_Key(it, "toast"));
                            }
                            else
                            {
                                Notification.Delete(Notification.Create_Key(it, "toast"));
                            }
                        }
                        else if (AorT == 1)
                        {
                            if (!Memory.alarmBlocked[it.rbr])
                            {
                                if (!it.alarmSkipped)
                                    Notification.Create_New_Alarm(it, firstDel);
                                else
                                    Notification.Delete(Notification.Create_Key(it, "alarm"));
                            }
                            else
                            {
                                Notification.Delete(Notification.Create_Key(it, "alarm"));
                            }
                        }
                        else if (AorT == 2)
                        {
                            if (!Memory.toastBlocked[it.rbr])
                            {
                                if (!it.toastSkipped)
                                    Notification.Create_New_Toast(it, firstDel);
                                else
                                    Notification.Delete(Notification.Create_Key(it, "toast"));
                            }
                            else
                            {
                                Notification.Delete(Notification.Create_Key(it, "toast"));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Greška pri registraciji grupe notifikacija " + "(" + ex.Message + ")");
                    }
                }
            }
        }

        public static void Last_Notification(Day day)
        {
            try
            {
                Notification.Delete("lastnotif");
                ScheduledToastNotification scheduledToast = new ScheduledToastNotification(Last_Notification_XML(), day.vakti[day.vakti.Count - 1].time.AddMinutes(-5));
                scheduledToast.Id = "lastnotif";
                scheduledToast.SuppressPopup = true;
                ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Greška pri registraciji upozoravajuće notifikacije " + "(" + ex.Message + ")");
            }
        }
        private static XmlDocument Last_Notification_XML()
        {
            string toastXmlString =
                        "<toast launch=\"lastnotif\">\n" +
                            "<visual>\n" +
                                "<binding template=\"ToastGeneric\">\n" +
                                    "<text>O vjernici, molitvu obavljajte i Gospodaru svome se klanjajte, i dobra djela činite da biste postigli ono što želite...</text>\n" +
                                    "<text>Sura 22:77 (El-Hadždž – Hadž, Medina)</text>\n" +
                                "</binding>\n" +
                            "</visual>\n" +
                            "<audio silent=\"true\" />\n" +
                       "</toast>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(toastXmlString);
            return doc;
        }

        public static System.Collections.Generic.List<T> JsonToArray<T>(string jsonString)
        {
            jsonString = jsonString.Replace("č", "\u010d").Replace("ć", "\u0107").Replace("đ", "\u0111").Replace("š", "\u0161").Replace("ž", "\u017e").Replace("Č", "\u010c").Replace("Ć", "\u0106").Replace("Đ", "\u0110").Replace("Š", "\u0160").Replace("Ž", "\u017d");
            if (jsonString.Length < 1) return new System.Collections.Generic.List<T>();
            if (jsonString[0] != '[') jsonString = "[" + jsonString + "]";
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
    }
}