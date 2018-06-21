namespace BackgroundTask.Helpers
{
    class Fixed
    {
        public static string[] timeNames = { "Zora", "Izlazak Sunca", "Podne", "Ikindija", "Akšam", "Jacija" };

        public static string App_Name = "Vaktija.ba";
        public static string App_Developer = "Vaktija.ba team";

        public static bool IsPhone
        {
            get
            {
                return Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");
            }
        }
    }
}
