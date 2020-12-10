using System;
using Windows.Data.Xml.Dom;

namespace Vaktija.ba.Helpers
{
    class LiveTile
    {
        public static void Update()
        {
            if (!Memory.Live_Tile)
            {
                Reset();
                return;
            }

            XmlDocument txml = new XmlDocument();
            txml.LoadXml(XMLforLiveTile(DateTime.Now));

            Windows.UI.Notifications.TileNotification tNotification = new Windows.UI.Notifications.TileNotification(txml);
            Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().Update(tNotification);

            RegisterLiveTiles();
        }

        public static void RegisterLiveTiles()
        {
            UnregisterAllScheduledLiveTiles();

            for (int i = 0; i < Data.data.vakatNames.Count; i++)
            {
                if (Data.VakatTime(DateTime.Now, i) > DateTime.Now)
                {
                    Vakat vakat = new Vakat { time = Data.VakatTime(DateTime.Now, i), name = Data.data.vakatNames[i].Replace("Izlazak", "Izl."), rbr = i };

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(XMLforLiveTile(vakat.time));
                    try
                    {
                        Windows.UI.Notifications.ScheduledTileNotification scheduledTile = new Windows.UI.Notifications.ScheduledTileNotification(doc, vakat.time);
                        Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(scheduledTile);
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine("Greška pri registraciji livetile za " + vakat.name + " (" + vakat.time.ToString() + ")");
                    }
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
            UnregisterAllScheduledLiveTiles();

            string xml = "";
            xml += "<tile>\n";
            xml += "    <visual>\n";
            xml += "        <binding template=\"TileLarge\" displayName=\"Vaktija.ba\" branding=\"name\">\n";
            xml += "            <image src=\"Assets/Square310x310Logo.png\" placement=\"background\"/>";
            xml += "        </binding>\n";
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

        public static string XMLforLiveTile(DateTime date)
        {

            #region Lock screen details
            string rowOnLockScreen1 = Data.VakatTime(date, 0).ToString("HH:mm") + " " + Data.data.vakatNames[0];
            rowOnLockScreen1 += Environment.NewLine + Data.VakatTime(date, 1).ToString("HH:mm") + " " + Data.data.vakatNames[1].Replace("Izlazak", "Izl.");

            string rowOnLockScreen2 = Data.VakatTime(date, 2).ToString("HH:mm") + " " + Data.data.vakatNames[2];
            rowOnLockScreen2 += Environment.NewLine + Data.VakatTime(date, 3).ToString("HH:mm") + " " + Data.data.vakatNames[3];

            string rowOnLockScreen3 = Data.VakatTime(date, 4).ToString("HH:mm") + " " + Data.data.vakatNames[4];
            rowOnLockScreen3 += Environment.NewLine + Data.VakatTime(date, 5).ToString("HH:mm") + " " + Data.data.vakatNames[5];

            rowOnLockScreen3 += Environment.NewLine + "Vaktija za " + Data.data.locationsDative[Memory.location];
            #endregion

            Vakat nextPrayer = Data.GetNextPrayer(date);

            string xml = "";
            xml += "<tile>\n";
            xml += "    <visual version=\"2\">\n";
            xml += "        <binding template=\"TileSquare310x310BlockAndText01\">";
            xml += "            <text id=\"1\">Vaktija za " + Data.data.locationsDative[Memory.location] + "</text>";
            xml += "            <text id=\"2\">" + Data.VakatTime(date, 0).ToString("HH:mm") + " " + Data.data.vakatNames[0] + "</text>";
            xml += "            <text id=\"3\">" + Data.VakatTime(date, 1).ToString("HH:mm") + " " + Data.data.vakatNames[1].Replace("Izlazak", "Izl.") + "</text>";
            xml += "            <text id=\"4\">" + Data.VakatTime(date, 2).ToString("HH:mm") + " " + Data.data.vakatNames[2] + "</text>";
            xml += "            <text id=\"5\">" + Data.VakatTime(date, 3).ToString("HH:mm") + " " + Data.data.vakatNames[3] + "</text>";
            xml += "            <text id=\"6\">" + Data.VakatTime(date, 4).ToString("HH:mm") + " " + Data.data.vakatNames[4] + "</text>";
            xml += "            <text id=\"7\">" + Data.VakatTime(date, 5).ToString("HH:mm") + " " + Data.data.vakatNames[5] + "</text>";
            xml += "            <text hint-align=\"center\" id=\"8\">" + date.Day + "</text>";
            xml += "            <text hint-align=\"center\" id=\"9\">" + Data.data.monthnames[DateTime.Now.Month % 12] + "</text>";
            xml += "        </binding>";
            xml += "        <binding template=\"TileWide\" hint-lockDetailedStatus1=\"" + rowOnLockScreen1 + "\" hint-lockDetailedStatus2=\"" + rowOnLockScreen2 + "\" hint-lockDetailedStatus3=\"" + rowOnLockScreen3 + "\">";
            xml += "            <group>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">" + Data.data.vakatNames[0] + "</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + Data.VakatTime(date, 0).ToString("HH:mm") + "</text>";
            xml += "                </subgroup>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">" + Data.data.vakatNames[1].Replace("Izlazak", "Izl.") + "</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + Data.VakatTime(date, 1).ToString("HH:mm") + "</text>";
            xml += "                </subgroup>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">" + Data.data.vakatNames[2] + "</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + Data.VakatTime(date, 2).ToString("HH:mm") + "</text>";
            xml += "                </subgroup>";
            xml += "            </group>";
            xml += "            <group>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">" + Data.data.vakatNames[3] + "</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + Data.VakatTime(date, 3).ToString("HH:mm") + "</text>";
            xml += "                </subgroup>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">" + Data.data.vakatNames[4] + "</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + Data.VakatTime(date, 4).ToString("HH:mm") + "</text>";
            xml += "                </subgroup>";
            xml += "                <subgroup hint-weight=\"1\">";
            xml += "                    <text hint-align=\"center\">" + Data.data.vakatNames[5] + "</text>";
            xml += "                    <text hint-align=\"center\" hint-style=\"body\">" + Data.VakatTime(date, 5).ToString("HH:mm") + "</text>";
            xml += "                </subgroup>";
            xml += "            </group>";
            xml += "        </binding>";
            xml += "        <binding template=\"TileMedium\" hint-textStacking=\"center\">";
            xml += "            <text hint-style=\"body\" hint-align=\"center\">" + nextPrayer.name.Replace("Izlazak", "Izl.") + "</text>";
            xml += "            <text hint-style=\"subtitle\" hint-align=\"center\">" + nextPrayer.time.ToString("HH:mm") + "</text>";
            xml += "        </binding>";
            xml += "        <binding template=\"TileSmall\" hint-textStacking=\"center\">";
            xml += "            <text hint-align=\"center\">" + nextPrayer.name.Replace("Izlazak", "Izl.") + "</text>";
            xml += "            <text hint-style=\"body\" hint-align=\"center\">" + nextPrayer.time.ToString("HH:mm") + "</text>";
            xml += "        </binding>";
            xml += "    </visual>";
            xml += "</tile>";

            return xml;
        }
    }
}
