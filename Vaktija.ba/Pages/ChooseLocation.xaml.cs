using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vaktija.ba.Helpers;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Vaktija.ba.Pages
{
    public sealed partial class ChooseLocation : Page
    {
        public ChooseLocation()
        {
            this.InitializeComponent();

            DataContext = new OdabirLokacije();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Fixed.IsPhone)
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += (s, args) =>
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    Frame.Navigate(typeof(Home));
                };
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (Fixed.IsPhone)
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;

            Frame.Navigate(typeof(Home));
        }

        private void Odaberi_Btn_Click(object sender, RoutedEventArgs e)
        {
            Memory.location = locationsListBox.SelectedIndex;

            LiveTile.Update();
            Notification.Set(Data.Obavijest.All);

            Frame.Navigate(typeof(Home));
        }

        private void locationsListBox_Loaded(object sender, RoutedEventArgs e)
        {
            locationsListBox.ScrollIntoView((sender as ListBox).SelectedItem);
        }
    }

    class OdabirLokacije
    {
        public Brush background
        {
            get
            {
                if (Fixed.IsDarkTheme) return new SolidColorBrush(Color.FromArgb(255, 31, 31, 31));
                else return new SolidColorBrush(Color.FromArgb(255, 241, 241, 241));
            }
        }
        public int selectedIndex { get => Memory.location; }
        public List<Lokacije> locations
        {
            get
            {
                List<Lokacije> loka = new List<Lokacije>();
                for (int i = 0; i < Data.data.locations.Count; i++)
                    loka.Add(new Lokacije { name = Data.data.locations[i], selected = Memory.location == i });
                return loka;
            }
        }
    }
    class Lokacije
    {
        public string name { get; set; }
        public bool selected { get; set; }
    }

}