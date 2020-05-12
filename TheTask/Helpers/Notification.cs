using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BackgroundTask.Helpers
{
    class Notification
    {
        public static void Set(Data.Obavijest obavijest)
        {
            Obavijesti_Za_Dan(DateTime.Now, obavijest);               // POSTAVI OBAVIJESTI ZA DANAS
            Obavijesti_Za_Dan(DateTime.Now.AddDays(1), obavijest);    // POSTAVI OBAVIJESTI ZA SUTRA

            Last_Notification();
        }

        public static void Obavijesti_Za_Dan(DateTime dt, Data.Obavijest obavijest)
        {
            for (int i = 0; i < Data.data.vakatNames.Count; i++)
            {
                Vakat vakat = new Vakat { time = Data.VakatTime(dt, i), name = Data.data.vakatNames[i], rbr = i };

                if (obavijest == Data.Obavijest.All)
                {
                    Kreiraj_Novu_Obavijest(vakat, Data.Obavijest.Alarm);
                    Kreiraj_Novu_Obavijest(vakat, Data.Obavijest.Toast);
                }
                else
                {
                    Kreiraj_Novu_Obavijest(vakat, obavijest);
                }
            }
        }

        public static void Kreiraj_Novu_Obavijest(Vakat vakat, Data.Obavijest obavijest)
        {
            if (vakat.time.AddMinutes(-vakat.notificationsOffset(obavijest)) <= DateTime.Now) return; // Ako je prošlo vrijeme za obavijest - prekid

            if (Obavijest_Postavljena(vakat, obavijest)) return; // Ako je obavijest već postavljena - prekid // Ako je isključena - obrisati - prekid

            string key = vakat.NotificationKey(obavijest);

            if (obavijest == Data.Obavijest.Alarm)
            {
                if (vakat.time <= DateTime.Now.AddMinutes(vakat.predAlarm)) return; // Ako je prošlo vrijeme za alarm - prekid

                try
                {
                    string toastXmlString = "";
                    if (Fixed.IsPhone)
                        toastXmlString = "<toast duration=\"long\">\n" +
                             "<visual>\n" +
                                 "<binding template=\"ToastText02\">\n" +
                                 "<text id=\"1\">" + vakat.name + " je za " + vakat.predAlarm + " minuta" + "</text>\n" +
                                 "<text id=\"2\">" + Fixed.App_Name + "</text>\n" +
                                 "</binding>\n" +
                             "</visual>\n" +
                             "<commands scenario=\"alarm\">\n" +
                                 "<command id=\"snooze\"/>\n" +
                                 "<command id=\"dismiss\"/>\n" +
                             "</commands>\n" +
                             "<audio src=\"" + Memory.Alarm_Sound + "\" loop=\"true\" />\n" +
                         "</toast>\n";
                    else
                        toastXmlString =
                              "<toast duration=\"long\">\n" +
                                   "<visual>\n" +
                                       "<binding template=\"ToastText02\">\n" +
                                       "<text id=\"1\">" + vakat.name + " je za " + vakat.predAlarm + " minuta" + "</text>\n" +
                                       "<text id=\"2\">" + Fixed.App_Name + "</text>\n" +
                                       "</binding>\n" +
                                   "</visual>\n" +
                                   "<commands scenario=\"alarm\">\n" +
                                       "<command id=\"snooze\"/>\n" +
                                       "<command id=\"dismiss\"/>\n" +
                                   "</commands>\n" +
                                   "<audio src=\"ms-winsoundevent:Notification.Looping.Alarm2\" loop=\"true\" />\n" +
                               "</toast>\n";

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(toastXmlString);

                    ScheduledToastNotification scheduledToast = new ScheduledToastNotification(doc, vakat.time.AddMinutes(-vakat.predAlarm));
                    scheduledToast.Id = key;
                    ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
                    System.Diagnostics.Debug.WriteLine("Alarm created at " + vakat.time.AddMinutes(-vakat.predToast) + " (" + vakat.name + ")");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Greška pri registraciji alarma za " + vakat.name + " u vrijeme " + vakat.time.AddMinutes(-vakat.predToast) + " (" + ex.Message + ")");
                }
            }
            else if (obavijest == Data.Obavijest.Toast)
            {
                if (vakat.time <= DateTime.Now.AddMinutes(vakat.predToast)) return; // Ako je prošlo vrijeme za alarm - prekid

                try
                {
                    ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

                    XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                    toastTextElements[0].AppendChild(toastXml.CreateTextNode(vakat.name + " je za " + vakat.predToast + " minuta"));
                    toastTextElements[1].AppendChild(toastXml.CreateTextNode("Utišajte zvukove uređaja!"));

                    IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
                    ((XmlElement)toastNode).SetAttribute("launch", "MainPage.xaml");

                    XmlElement audio = toastXml.CreateElement("audio");
                    if (Fixed.IsPhone)
                        audio.SetAttribute("src", Memory.Toast_Sound);
                    toastNode.AppendChild(audio);

                    ScheduledToastNotification scheduledToast = new ScheduledToastNotification(toastXml, vakat.time.AddMinutes(-vakat.predToast));
                    scheduledToast.Id = key;
                    ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
                    System.Diagnostics.Debug.WriteLine("Notification created at " + vakat.time.AddMinutes(-vakat.predToast) + " (" + vakat.name + ")");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Greška pri registraciji notifikacije za " + vakat.name + " u vrijeme " + vakat.time.AddMinutes(-vakat.predToast) + " (" + ex.Message + ")");
                }
            }
        }

        public static void Obrisi_Obavijest(string key)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();
            var scheduled = notifier.GetScheduledToastNotifications();
            for (int i = 0; i < scheduled.Count; i++)
            {
                if (scheduled[i].Id == key)
                {
                    ToastNotificationManager.CreateToastNotifier().RemoveFromSchedule(scheduled[i]);
                    System.Diagnostics.Debug.WriteLine("Obrisana obavijest [key]: " + key);
                }
            }
        }

        public static bool Obavijest_Postavljena(Vakat vakat, Data.Obavijest obavijest)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();
            var scheduled = notifier.GetScheduledToastNotifications();

            string key = vakat.NotificationKey(obavijest);

            if (vakat.isSkipped(obavijest))
            {
                Obrisi_Obavijest(vakat.NotificationKey(obavijest));
                return true;
            }

            if (vakat.blockedNotifications(obavijest))
            {
                Obrisi_Obavijest(key);
                return true;
            }

            int predNotif = vakat.notificationsOffset(obavijest);

            for (int i = 0; i < scheduled.Count; i++)
            {
                if (scheduled[i].Id == key)
                {
                    if (scheduled[i].DeliveryTime.DateTime == vakat.time.AddMinutes(-predNotif))
                    {
                        System.Diagnostics.Debug.WriteLine("Obavijest [key]: " + key + " - već postoji...");
                        return true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Obavijest [key]: " + key + " - na pogrešnom mjestu - BRIŠEM...");
                        ToastNotificationManager.CreateToastNotifier().RemoveFromSchedule(scheduled[i]);
                        return false;
                    }
                }
            }
            return false;
        }

        public static void Last_Notification()
        {
            for (int i = 0; i < 7; i++)
            {
                if (DateTime.Now.AddDays(i).DayOfWeek == DayOfWeek.Thursday)
                {
                    try
                    {
                        DateTime dt = Data.VakatTime(DateTime.Now.AddDays(i), 5);
                        Obrisi_Obavijest("lastnotif");
                        ScheduledToastNotification scheduledToast = new ScheduledToastNotification(Last_Notification_XML(), dt.AddMinutes(-25));
                        scheduledToast.Id = "lastnotif";
                        scheduledToast.SuppressPopup = true;
                        ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
                        System.Diagnostics.Debug.WriteLine("Alert obavijest postavljena...");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Greška pri registraciji Alert obavijesti " + "(" + ex.Message + ")");
                    }
                }
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
    }
}
