using System.Threading.Tasks;
using Vaktija.ba.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Vaktija.ba
{
    public sealed partial class MainPage : Page
    {
        public static bool Theme_Changed = false;
        public static bool firstTime = false;
        public static bool firstUpad = false;

        public MainPage()
        {
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            bool ft = false;
            Application.Current.Resuming += (s, ar) =>
            {
                try { } catch { }
            };

            try
            {
                ft = Memory.First_Time();
                Views.Year.Set();
            }
            catch
            {
            }
            await Task.Delay(250);
            Zavrsi_Ucitavanje(ft);
        }
        private void Zavrsi_Ucitavanje(bool ft)
        {
            if (ft)
            {
                firstTime = true;
                Frame.Navigate(typeof(Pages.ChooseLocation));
            }
            else
            {
                firstUpad = true;
                Frame.Navigate(typeof(Pages.Home));
            }
        }
    }
}
