using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Vaktija.ba.Helpers
{
    class Notification
    {
        public static async void Set(Data.Obavijest obavijest)
        {
            System.Diagnostics.Debug.WriteLine("Učitavanje podataka za " + Data.data.locationsDative[Memory.location]);
            Set_System_Tray("Učitavanje podataka za " + Data.data.locationsDative[Memory.location]);
            await System.Threading.Tasks.Task.Delay(20);

            Obavijesti_Za_Dan(DateTime.Now, obavijest);               // POSTAVI OBAVIJESTI ZA DANAS
            Obavijesti_Za_Dan(DateTime.Now.AddDays(1), obavijest);    // POSTAVI OBAVIJESTI ZA SUTRA

            Last_Notification();
            await System.Threading.Tasks.Task.Delay(20);
            Set_System_Tray();
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
            else if(obavijest == Data.Obavijest.Toast)
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

            if(vakat.isSkipped(obavijest))
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
                        if(DateTime.Now >= dt.AddMinutes(-25)) dt = Data.VakatTime(DateTime.Now.AddDays(i+7), 5);
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

        public static async void Set_System_Tray(string message = "")
        {
            if (Fixed.IsPhone)
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                if (Fixed.IsDarkTheme)
                {
                    statusBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 31, 31, 31);
                    statusBar.ForegroundColor = Windows.UI.Colors.White;
                }
                else
                {
                    statusBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 244, 244, 244);
                    statusBar.ForegroundColor = Windows.UI.Colors.Black;
                }
                statusBar.BackgroundOpacity = 1;
                statusBar.ProgressIndicator.ProgressValue = 0;
                statusBar.ProgressIndicator.Text = message;
                if (message != "")
                {
                    statusBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 165, 149, 115);
                    statusBar.ForegroundColor = Windows.UI.Colors.Black;
                }
                await statusBar.ProgressIndicator.ShowAsync();
            }
            else
            {
                var v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
                if (Fixed.IsDarkTheme)
                {
                    v.TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 31, 31, 31);
                    v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 31, 31, 31);
                    v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.White;
                    v.TitleBar.ForegroundColor = Windows.UI.Colors.White;
                }
                else
                {
                    v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 244, 244, 244);
                    v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.Black;
                    v.TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 244, 244, 244);
                    v.TitleBar.ForegroundColor = Windows.UI.Colors.Black;
                }
                if (message != "")
                {
                    v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 165, 149, 115);
                    v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.Black;
                    v.TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 165, 149, 115);
                    v.TitleBar.ForegroundColor = Windows.UI.Colors.Black;
                }
                v.Title = message;
            }
        }

    }
}
