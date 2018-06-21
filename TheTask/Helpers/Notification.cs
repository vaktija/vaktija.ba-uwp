using System;
using BackgroundTask.Views;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BackgroundTask.Helpers
{
    class Notification
    {
        public static void Create_New_Alarm(Vakat vakat, bool firstDel = false)
        {
            if (vakat.time <= DateTime.Now.AddMinutes(Memory.predAlarm[vakat.rbr])) return;
            string key = Create_Key(vakat, "alarm");

            if (firstDel)
            {
                Delete(Create_Key(vakat, "alarm"));
            }
            else
            {
                if (If_Different_Delete(key, vakat, Memory.predAlarm[vakat.rbr])) return;
            }

            try
            {
                string toastXmlString =
                          "<toast duration=\"long\">\n" +
                               "<visual>\n" +
                                   "<binding template=\"ToastText02\">\n" +
                                   "<text id=\"1\">" + vakat.name + " je za " + Memory.predAlarm[vakat.rbr] + " minuta" + "</text>\n" +
                                   "<text id=\"2\">" + Fixed.App_Name + "</text>\n" +
                                   "</binding>\n" +
                               "</visual>\n" +
                               "<commands scenario=\"alarm\">\n" +
                                   "<command id=\"snooze\"/>\n" +
                                   "<command id=\"dismiss\"/>\n" +
                               "</commands>\n" +
                               "<audio src=\"" + Memory.Alarm_Sound + "\" loop=\"true\" />\n" +
                           "</toast>\n";
                if (!Fixed.IsPhone)
                    toastXmlString =
                          "<toast duration=\"long\">\n" +
                               "<visual>\n" +
                                   "<binding template=\"ToastText02\">\n" +
                                   "<text id=\"1\">" + vakat.name + " je za " + Memory.predAlarm[vakat.rbr] + " minuta" + "</text>\n" +
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

                ScheduledToastNotification scheduledToast = new ScheduledToastNotification(doc, vakat.time.AddMinutes(-Memory.predAlarm[vakat.rbr]));
                scheduledToast.Id = key;
                ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
                System.Diagnostics.Debug.WriteLine("Alarm created at " + vakat.time + " (" + vakat.name + ")");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Greška pri registraciji alarma za " + vakat.name + " u vrijeme " + vakat.time + " (" + ex.Message + ")");
            }
        }
        public static void Create_New_Toast(Vakat vakat, bool firstDel = false)
        {
            if (vakat.time <= DateTime.Now.AddMinutes(Memory.predToast[vakat.rbr])) return;
            string key = Create_Key(vakat, "toast");

            if (firstDel)
            {
                Delete(Create_Key(vakat, "toast"));
            }
            else
            {
                if (If_Different_Delete(key, vakat, Memory.predToast[vakat.rbr])) return;
            }

            try
            {
                ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

                XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                toastTextElements[0].AppendChild(toastXml.CreateTextNode(vakat.name + " je za " + Memory.predToast[vakat.rbr] + " minuta"));
                toastTextElements[1].AppendChild(toastXml.CreateTextNode("Utišajte zvukove uređaja!"));

                IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
                ((XmlElement)toastNode).SetAttribute("launch", "MainPage.xaml");

                XmlElement audio = toastXml.CreateElement("audio");
                if (Fixed.IsPhone)
                    audio.SetAttribute("src", Memory.Toast_Sound);
                toastNode.AppendChild(audio);

                ScheduledToastNotification scheduledToast = new ScheduledToastNotification(toastXml, vakat.time.AddMinutes(-Memory.predToast[vakat.rbr]));
                scheduledToast.Id = key;
                ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
                System.Diagnostics.Debug.WriteLine("Notification created at " + vakat.time + " (" + vakat.name + ")");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Greška pri registraciji notifikacije za " + vakat.name + " u vrijeme " + vakat.time + " (" + ex.Message + ")");
            }
        }

        public static void Delete(string key)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();
            var scheduled = notifier.GetScheduledToastNotifications();
            for (int i = 0; i < scheduled.Count; i++)
            {
                if (scheduled[i].Id == key)
                {
                    ToastNotificationManager.CreateToastNotifier().RemoveFromSchedule(scheduled[i]);
                }
            }
        }

        public static bool If_Different_Delete(string key, Vakat vakat, int predNotif)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();
            var scheduled = notifier.GetScheduledToastNotifications();
            for (int i = 0; i < scheduled.Count; i++)
            {
                if (scheduled[i].Id == key)
                {
                    if (scheduled[i].DeliveryTime.DateTime == vakat.time.AddMinutes(-predNotif)) return true;
                    else
                    {
                        ToastNotificationManager.CreateToastNotifier().RemoveFromSchedule(scheduled[i]);
                        return false;
                    }
                }
            }
            return false;
        }

        public static string Create_Key(Vakat vakat, string type)
        {
            return (vakat.time.Year + "|" + vakat.time.Month + "|" + vakat.time.Day + "|" + vakat.name.Substring(0, 2).ToLower() + "|" + type.Substring(0, 1)).ToLower();
        }
    }
}
