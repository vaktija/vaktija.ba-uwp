using System;
using BackgroundTask.Views;
using Windows.Data.Xml.Dom;
using System.Collections.Generic;

namespace BackgroundTask.Helpers
{
    class LiveTile
    {
        public static void Update()
        {
            Day juce = Year.year.months[DateTime.Now.AddDays(-1).Month - 1].days[DateTime.Now.AddDays(-1).Day - 1];
            Day danas = Year.year.months[DateTime.Now.Month - 1].days[DateTime.Now.Day - 1];
            Day sutra = Year.year.months[DateTime.Now.AddDays(1).Month - 1].days[DateTime.Now.AddDays(1).Day - 1];

            #region Lock screen details
            string rowOnLockScreen1 = danas.vakti[0].time.ToString("HH:mm") + " zora       " + " izl sunca " + danas.vakti[1].time.ToString("HH:mm");
            string rowOnLockScreen2 = danas.vakti[2].time.ToString("HH:mm") + " podne   " + "    ikindija " + danas.vakti[3].time.ToString("HH:mm");
            string rowOnLockScreen3 = danas.vakti[4].time.ToString("HH:mm") + " akšam   " + "        jacija " + danas.vakti[5].time.ToString("HH:mm") + "\n" + Memory.location.ime.ToLower();
            #endregion

            var nextPrayer = Get.Next_Prayer_Time();

            string xml = "";
            xml += "<tile>\n";
            xml += "    <visual>\n";
            xml += "        <binding template=\"TileWide\" hint-lockDetailedStatus1=\"" + rowOnLockScreen1.ToLower() + "\" hint-lockDetailedStatus2=\"" + rowOnLockScreen2.ToLower() + "\" hint-lockDetailedStatus3=\"" + rowOnLockScreen3.ToLower() + "\">";
            //xml += "            <image placement=\"peek\" src=\"Assets/Wide310x150Logo.png\"/>";
            xml += "            <group>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">zora</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + danas.vakti[0].time.ToString("HH:mm") + "h</text>";
            xml += "                </subgroup>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">izl sunca</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + danas.vakti[1].time.ToString("HH:mm") + "h</text>";
            xml += "                </subgroup>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">podne</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + danas.vakti[2].time.ToString("HH:mm") + "h</text>";
            xml += "                </subgroup>";
            xml += "            </group>";
            xml += "            <group>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">ikindija</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + danas.vakti[3].time.ToString("HH:mm") + "h</text>";
            xml += "                </subgroup>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">akšam</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + danas.vakti[4].time.ToString("HH:mm") + "h</text>";
            xml += "                </subgroup>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">jacija</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + danas.vakti[5].time.ToString("HH:mm") + "h</text>";
            xml += "                </subgroup>";
            xml += "            </group>";
            xml += "        </binding>";
            xml += "        <binding template=\"TileMedium\" hint-textStacking=\"center\">";
            //xml += "            <image placement=\"peek\" src=\"Assets/Square150x150Logo.png\"/>";
            xml += "            <text hint-style=\"body\" hint-align=\"center\">" + nextPrayer.name.ToLower() + "</text>";
            xml += "            <text hint-style=\"subtitle\" hint-align=\"center\">" + nextPrayer.time.ToString("HH:mm") + "h</text>";
            xml += "        </binding>";
            xml += "        <binding template=\"TileSmall\" hint-textStacking=\"center\">";
            //xml += "            <image placement=\"peek\" src=\"Assets/Square71x71Logo.png\"/>";
            xml += "            <text hint-align=\"center\">" + nextPrayer.name.ToLower() + "</text>";
            xml += "            <text hint-style=\"body\" hint-align=\"center\">" + nextPrayer.time.ToString("HH:mm") + "h</text>";
            xml += "        </binding>";
            xml += "    </visual>";
            xml += "</tile>";

            Windows.Data.Xml.Dom.XmlDocument txml = new Windows.Data.Xml.Dom.XmlDocument();
            txml.LoadXml(xml);
            Windows.UI.Notifications.TileNotification tNotification = new Windows.UI.Notifications.TileNotification(txml);
            Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().Update(tNotification);

            RegisterLiveTiles();
        }
        public static void RegisterLiveTiles()
        {
            UnregisterAllScheduledLiveTiles();

            Day juce = Year.year.months[DateTime.Now.AddDays(-1).Month - 1].days[DateTime.Now.AddDays(-1).Day - 1];
            Day danas = Year.year.months[DateTime.Now.Month - 1].days[DateTime.Now.Day - 1];
            Day sutra = Year.year.months[DateTime.Now.AddDays(1).Month - 1].days[DateTime.Now.AddDays(1).Day - 1];
            List<Day> jds = new List<Day> { juce, danas, sutra };

            #region Lock screen details
            string rowOnLockScreen1 = danas.vakti[0].time.ToString("HH:mm") + " zora       " + " izl sunca " + danas.vakti[1].time.ToString("HH:mm");
            string rowOnLockScreen2 = danas.vakti[2].time.ToString("HH:mm") + " podne   " + "    ikindija " + danas.vakti[3].time.ToString("HH:mm");
            string rowOnLockScreen3 = danas.vakti[4].time.ToString("HH:mm") + " akšam   " + "        jacija " + danas.vakti[5].time.ToString("HH:mm") + "\n" + Memory.location.ime.ToLower();
            #endregion

            var nextPrayer = Get.Next_Prayer_Time();
            System.Diagnostics.Debug.WriteLine("Next prayer: " + nextPrayer.name);

            string xml = "";
            xml += "<tile>\n";
            xml += "    <visual>\n";
            xml += "        <binding template=\"TileWide\" hint-lockDetailedStatus1=\"" + rowOnLockScreen1.ToLower() + "\" hint-lockDetailedStatus2=\"" + rowOnLockScreen2.ToLower() + "\" hint-lockDetailedStatus3=\"" + rowOnLockScreen3.ToLower() + "\">";
            //xml += "            <image placement=\"peek\" src=\"Assets/Wide310x150Logo.png\"/>";
            xml += "            <group>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">zora</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + danas.vakti[0].time.ToString("HH:mm") + "h</text>";
            xml += "                </subgroup>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">izl sunca</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + danas.vakti[1].time.ToString("HH:mm") + "h</text>";
            xml += "                </subgroup>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">podne</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + danas.vakti[2].time.ToString("HH:mm") + "h</text>";
            xml += "                </subgroup>";
            xml += "            </group>";
            xml += "            <group>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">ikindija</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + danas.vakti[3].time.ToString("HH:mm") + "h</text>";
            xml += "                </subgroup>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">akšam</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + danas.vakti[4].time.ToString("HH:mm") + "h</text>";
            xml += "                </subgroup>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">jacija</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + danas.vakti[5].time.ToString("HH:mm") + "h</text>";
            xml += "                </subgroup>";
            xml += "            </group>";
            xml += "        </binding>";
            xml += "        <binding template=\"TileMedium\" hint-textStacking=\"center\" branding=\"logo\">";
            //xml += "            <image placement=\"peek\" src=\"Assets/Square150x150Logo.png\"/>";
            xml += "            <text hint-style=\"body\" hint-align=\"center\">[text1]</text>";
            xml += "            <text hint-style=\"subtitle\" hint-align=\"center\">[text2]h</text>";
            xml += "        </binding>";
            xml += "        <binding template=\"TileSmall\" hint-textStacking=\"center\">";
            //xml += "            <image placement=\"peek\" src=\"Assets/Square71x71Logo.png\"/>";
            xml += "            <text hint-align=\"center\">[text1]</text>";
            xml += "            <text hint-style=\"body\" hint-align=\"center\">[text2]h</text>";
            xml += "        </binding>";
            xml += "    </visual>";
            xml += "</tile>";

            Vakat trenV = Get.Current_Prayer_Time();
            var currentPrayer = Get.Current_Prayer_Time();

            int j = 0;

            bool el = false;

            foreach (var day in jds)
            {
                foreach (var it in day.vakti)
                {
                    if (el)
                    {
                        j++;
                        if (j > 6)
                        {
                            var notifier = Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication();
                            var scheduled = notifier.GetScheduledTileNotifications();
                            for (int i = 0; i < scheduled.Count; i++)
                            {
                                System.Diagnostics.Debug.WriteLine("Livetile at: " + scheduled[i].DeliveryTime);
                            }
                            return;
                        }
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(xml.Replace("[text2]", it.time.ToString("H:mm")).Replace("[text1]", it.name.ToLower()));
                        try
                        {
                            Windows.UI.Notifications.ScheduledTileNotification scheduledTile = new Windows.UI.Notifications.ScheduledTileNotification(doc, trenV.time);
                            Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(scheduledTile);
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine("Greška pri registraciji livetile za " + it.name + " (" + it.time.ToString() + ")");
                        }
                    }

                    trenV = it;

                    if (it.time == nextPrayer.time) el = true;
                }
            }
        }

        public static void UnregisterAllScheduledLiveTiles()
        {
            var notifier = Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication();
            var scheduled = notifier.GetScheduledTileNotifications();
            for (int i = 0; i < scheduled.Count; i++)
            {
                Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().RemoveFromSchedule(scheduled[i]);
            }
        }
        public static void Reset()
        {
            var updater = Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication();
            var scheduled = updater.GetScheduledTileNotifications();
            for (int i = 0; i < scheduled.Count; i++)
            {
                Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().RemoveFromSchedule(scheduled[i]);
            }

            string xml = "";
            xml += "<tile>\n";
            xml += "    <visual>\n";
            xml += "        <binding template=\"TileWide\" displayName=\"Vaktija.ba\" branding=\"name\">\n";
            xml += "            <image src=\"Assets/Wide310x150Logo.png\" placement=\"background\"/>";
            xml += "        </binding>\n";
            xml += "        <binding template=\"TileMedium\" displayName=\"Vaktija.ba\" branding=\"name\">\n";
            xml += "            <image src=\"Assets/Square150x150Logo.png\" placement=\"background\"/>";
            xml += "        </binding>\n";
            xml += "        <binding template=\"TileSmall\">";
            xml += "            <image src=\"Assets/Square71x71Logo.png\" placement=\"background\"/>";
            xml += "        </binding>";
            xml += "    </visual>\n";
            xml += "</tile>";

            Windows.Data.Xml.Dom.XmlDocument txml = new Windows.Data.Xml.Dom.XmlDocument();
            txml.LoadXml(xml);
            Windows.UI.Notifications.TileNotification tNotification = new Windows.UI.Notifications.TileNotification(txml);
            Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().Update(tNotification);
        }
    }
}
