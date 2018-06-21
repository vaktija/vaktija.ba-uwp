using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vaktija.ba.Helpers;
using Vaktija.ba.Views;
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
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Application.Current.Resuming += (s, ar) =>
            {
                try { } catch { }
            };
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

            try
            {
                _pivot.SelectedIndex = 0;
            }
            catch
            {

            }

            Prikazi_Lokacije();
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (Fixed.IsPhone)
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            if (!MainPage.firstTime)
                Frame.GoBack();
            else
            {
                MainPage.firstTime = false;
                Frame.Navigate(typeof(Home));
            }
        }

        private async void Odaberi_Btn_Click(object sender, RoutedEventArgs e)
        {
            Location loc = new Location();
            if (_pivot.SelectedIndex == 0)
            {
                try
                {
                    loc = (Location)(((ListBoxItem)(lv1.SelectedItem)).Tag);
                }
                catch { return; }
            }
            else
            {
                try
                {
                    loc = (Location)(((ListBoxItem)(lv2.SelectedItem)).Tag);
                }
                catch { return; }
            }

            Memory.location = loc;

            loader.IsActive = true;
            await Task.Delay(50);
            Set.Group_Notifications(2);
            loader.IsActive = false;
            MainPage.firstTime = false;
            Frame.Navigate(typeof(Pages.Home));
        }

        private async void Prikazi_Lokacije()
        {
            if (Fixed.IsDarkTheme)
            {
                _pivot.Background = new SolidColorBrush(Color.FromArgb(255, 31, 31, 31));
            }
            else
            {
                _pivot.Background = new SolidColorBrush(Color.FromArgb(255, 241, 241, 241));
            }

            lv1.Items.Clear();
            lv2.Items.Clear();

            List<Location> gradovi = new List<Location>();

            foreach (var it in Data.data.gradovi)
            {
                ListBoxItem item = new ListBoxItem
                {
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    Tag = it,
                    Padding = new Thickness(3),
                };

                Grid gr_item = new Grid();

                TextBlock tb = new TextBlock
                {
                    Text = it.ime,
                    //FontSize = 26,
                    //Foreground = new SolidColorBrush(Colors.Black),
                };
                //if (Constant.IsDarkTheme) tb.Foreground = new SolidColorBrush(Colors.White);
                gr_item.Children.Add(tb);
                item.Content = gr_item;


                if (it.id <= 107)
                {
                    lv1.Items.Add(item);
                }
                else
                {
                    lv2.Items.Add(item);
                }
            }
            await Task.Delay(100);
            foreach (var ata in lv1.Items)
            {
                var it = (Location)(((ListBoxItem)ata).Tag);

                if (it.id == Memory.location.id)
                {
                    lv1.SelectedItem = ata;
                    lv1.ScrollIntoView(ata);
                    try
                    {
                        _pivot.SelectedIndex = 0;
                    }
                    catch { }
                }
            }
            foreach (var ata in lv2.Items)
            {
                var it = (Location)(((ListBoxItem)ata).Tag);

                if (it.id == Memory.location.id)
                {
                    lv2.SelectedItem = ata;
                    lv2.ScrollIntoView(ata);
                    try
                    {
                        _pivot.SelectedIndex = 1;
                    }
                    catch { }
                }
            }
        }
    }
}
