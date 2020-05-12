namespace Vaktija.ba.Helpers
{
    class Fixed
    {
        public static string App_Name = "Vaktija.ba";
        public static string App_Developer = "Vaktija.ba team";

        public static string App_Data_File = "ms-appx:///Files/data.json";

        public static string App_Assets_Folder = "Assets";
        public static string App_Sound_Folder = "Sounds";

        public static bool IsPhone
        {
            get
            {
                return Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");
            }
        }

        public static bool IsDarkTheme
        {
            get
            {
                return (bool)Windows.UI.Xaml.Application.Current.Resources["IsDarkTheme"];
            }
        }
    }
}
