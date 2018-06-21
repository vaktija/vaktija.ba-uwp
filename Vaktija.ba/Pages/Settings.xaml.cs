using System;
using System.Collections.Generic;
using Vaktija.ba.Helpers;
using Windows.ApplicationModel;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Vaktija.ba.Pages
{
    public sealed partial class Settings : Page
    {
        bool ucitavam = true;
        public Settings()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ucitavam = true;
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
                    Frame.Navigate(typeof(Pages.Home));
                };
            }

            Postavi_Stranicu();
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (Fixed.IsPhone)
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Frame.Navigate(typeof(Pages.Home));
        }

        private void Postavi_Stranicu()
        {
            ucitavam = true;

            if (Fixed.IsDarkTheme)
            {
                _pivot.Background = new SolidColorBrush(Color.FromArgb(255, 31, 31, 31));
            }
            else
            {
                _pivot.Background = new SolidColorBrush(Color.FromArgb(255, 241, 241, 241));
            }

            //Location
            locationHB.Text = Memory.location.ime;

            //Date in app
            dateShowInApp_TS.IsOn = Memory.Date_Show_In_App;

            //Hijri date in app
            hijriDateShowInApp_TS.IsEnabled = Memory.Date_Show_In_App;
            hijriDateShowInApp_TS.IsOn = Memory.Hijri_Date_In_App;

            //Standard zuhr time
            if (Memory.Std_Podne)
                stdPodneCB.SelectedIndex = 0;
            else
                stdPodneCB.SelectedIndex = 1;

            //Theme
            themeCB.SelectedIndex = Memory.Theme;

            //Alarm sound
            if (!Fixed.IsPhone) alarmLVI.Visibility = Visibility.Collapsed;
            alarmsoundHB.Text = Daj_Ime_Zvuka_Notifikacije(Memory.Alarm_Sound);

            //Toast sound
            toastsoundHB.Text = Daj_Ime_Zvuka_Notifikacije(Memory.Toast_Sound);

            //Live tile
            livetile_TS.IsOn = Memory.Live_Tile;

            //App info
            versionTB.Text = "v " + Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor + "." + Package.Current.Id.Version.Build;

            ucitavam = false;
        }

        private string Daj_Ime_Zvuka_Notifikacije(string x, bool star = false)
        {
            if (star)
            {
                if (x == Memory.Alarm_Sound)
                    star = true;
                else
                    star = false;
            }
            if (x.Contains("ms-appx"))
            {
                x = x.Replace("ms-appx:///Assets/Sounds/", "").Replace(".wav", "").Replace(".mp3", "").Replace(".ogg", "").Trim();
            }
            else if (x.Contains("Notification.Looping"))
            {
                x = x.Replace("ms-winsoundevent:Notification.Looping.", "").Trim();
            }
            else
            {
                x = x.Replace("ms-winsoundevent:Notification.", "").Trim();
            }
            if (star) x += " (*)";
            return x;
        }
        private List<string> Paths_To_All_Alarm_Sounds()
        {
            List<string> sounds = new List<string>();
            sounds.Add("ms-appx:///Assets/Sounds/BeepBeep.mp3");
            sounds.Add("ms-appx:///Assets/Sounds/Beep-Beep-Beep.mp3");
            sounds.Add("ms-appx:///Assets/Sounds/Buzzer.mp3");
            sounds.Add("ms-appx:///Assets/Sounds/Piezo.mp3");
            sounds.Add("ms-appx:///Assets/Sounds/Ringing.mp3");
            sounds.Add("ms-appx:///Assets/Sounds/Roster.mp3");
            return sounds;
        }
        private List<string> Paths_To_All_Toast_Sounds()
        {
            List<string> sounds = new List<string>();
            sounds.Add("ms-appx:///Assets/Sounds/BeepBeep.mp3");
            sounds.Add("ms-appx:///Assets/Sounds/Beep-Beep-Beep.mp3");
            sounds.Add("ms-appx:///Assets/Sounds/Buzzer.mp3");
            sounds.Add("ms-appx:///Assets/Sounds/Piezo.mp3");
            sounds.Add("ms-appx:///Assets/Sounds/Ringing.mp3");
            sounds.Add("ms-appx:///Assets/Sounds/Roster.mp3");
            return sounds;
        }

        private void location_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ucitavam) return;
            Frame.Navigate(typeof(ChooseLocation));
        }

        private void dateShowInApp_TS_Toggled(object sender, RoutedEventArgs e)
        {
            if (ucitavam) return;
            Memory.Date_Show_In_App = dateShowInApp_TS.IsOn;
            Postavi_Stranicu();
        }

        private void hijriDateShowInApp_TS_Toggled(object sender, RoutedEventArgs e)
        {
            if (ucitavam) return;
            Memory.Hijri_Date_In_App = hijriDateShowInApp_TS.IsOn;
            Postavi_Stranicu();
        }

        private void stdPodneCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ucitavam) return;
            if (stdPodneCB.SelectedIndex == 0)
            {
                Memory.Std_Podne = true;
            }
            else
            {
                Memory.Std_Podne = false;
            }
            Set.Group_Notifications(2, 0, true, 2, 0, true);
            Postavi_Stranicu();
        }

        private void themeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ucitavam) return;
            MainPage.Theme_Changed = true;

            Memory.Theme = themeCB.SelectedIndex;

            Toast_TextBlock.Text = "Promjene će biti vidljive pri sljedećem pokretanju aplikacije";
            Toast_Grid.Visibility = Visibility.Visible;
            DispatcherTimer dt = new DispatcherTimer { Interval = new TimeSpan(0, 0, 7) };
            dt.Tick += (s, args) =>
            {
                Toast_TextBlock.Text = "";
                Toast_Grid.Visibility = Visibility.Collapsed;
                dt.Stop();
            };
            dt.Start();
            Postavi_Stranicu();
        }

        private void alarmsound_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ucitavam) return;
            Grid mainGrid = new Grid { MinWidth = 320, };
            Flyout mainFlyout = new Flyout { Content = mainGrid, Placement = Windows.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Top };
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            // Setup Content
            var panel = new StackPanel
            {
                Padding = new Thickness(12),
            };
            panel.SetValue(Grid.RowProperty, 0);
            mainGrid.Children.Add(panel);

            TextBlock title = new TextBlock
            {
                Text = "Zvuk alarma",
                FontSize = 22,
                FontWeight = FontWeights.Bold,
            };
            panel.Children.Add(title);

            ListView lv = new ListView
            {
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                SelectionMode = ListViewSelectionMode.None,
            };
            panel.Children.Add(lv);

            List<string> svi = Paths_To_All_Alarm_Sounds();
            List<RadioButton> rbtns = new List<RadioButton>();
            foreach (var it in svi)
            {
                RadioButton rbtn = new RadioButton
                {
                    Content = Daj_Ime_Zvuka_Notifikacije(it),
                    Tag = it,
                    Padding = new Thickness(6),
                    FontSize = 18,
                };
                lv.Items.Add(rbtn);
                if (it == Memory.Alarm_Sound) rbtn.IsChecked = true;
                else rbtn.IsChecked = false;
                rbtns.Add(rbtn);

                rbtn.Checked += (s, args) =>
                {
                    foreach (var rb in rbtns)
                        if (rb != rbtn)
                            rb.IsChecked = false;

                    audio_prev.Source = new Uri(rbtn.Tag.ToString());
                    audio_prev.AutoPlay = true;
                    audio_prev.Stop();
                };
            }

            #region Buttons
            Grid keyGrid = new Grid { };
            keyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            keyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            keyGrid.SetValue(Grid.RowProperty, 1);
            mainGrid.Children.Add(keyGrid);

            Button lBtn = new Button
            {
                Padding = new Thickness(6),
                Margin = new Thickness(3),
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(1),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = "sačuvaj",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            lBtn.Click += (s, arg) =>
            {
                foreach (var rb in rbtns)
                    if (rb.IsChecked == true)
                        Memory.Alarm_Sound = rb.Tag.ToString();
                audio_prev.AutoPlay = false;
                audio_prev.Stop();
                Postavi_Stranicu();
                Set.Group_Notifications(2, 0, false, 6, 1, true);
                mainFlyout.Hide();
            };
            lBtn.SetValue(Grid.ColumnProperty, 0);
            keyGrid.Children.Add(lBtn);

            Button rBtn = new Button
            {
                Padding = new Thickness(6),
                Margin = new Thickness(3),
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(1),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = "poništi",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            rBtn.Click += (s, arg) =>
            {
                audio_prev.AutoPlay = false;
                audio_prev.Stop();
                mainFlyout.Hide();
            };
            mainFlyout.Closed += (s, arg) =>
            {
                audio_prev.AutoPlay = false;
                audio_prev.Stop();
            };
            rBtn.SetValue(Grid.ColumnProperty, 1);
            keyGrid.Children.Add(rBtn);
            #endregion

            mainFlyout.ShowAt(this);
        }

        private void toastsound_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ucitavam) return;
            Grid mainGrid = new Grid { MinWidth = 320, };
            Flyout mainFlyout = new Flyout { Content = mainGrid, Placement = Windows.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Top };
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            // Setup Content
            var panel = new StackPanel
            {
                Padding = new Thickness(12),
            };
            panel.SetValue(Grid.RowProperty, 0);
            mainGrid.Children.Add(panel);

            TextBlock title = new TextBlock
            {
                Text = "Zvuk notifikacije",
                FontSize = 22,
                FontWeight = FontWeights.Bold,
            };
            panel.Children.Add(title);

            ListView lv = new ListView
            {
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                SelectionMode = ListViewSelectionMode.None,
            };
            panel.Children.Add(lv);

            List<string> svi = Paths_To_All_Toast_Sounds();
            List<RadioButton> rbtns = new List<RadioButton>();
            foreach (var it in svi)
            {
                RadioButton rbtn = new RadioButton
                {
                    Content = Daj_Ime_Zvuka_Notifikacije(it),
                    Tag = it,
                    Padding = new Thickness(6),
                    FontSize = 18,
                };
                lv.Items.Add(rbtn);
                if (it == Memory.Toast_Sound) rbtn.IsChecked = true;
                else rbtn.IsChecked = false;
                rbtns.Add(rbtn);

                rbtn.Checked += (s, args) =>
                {
                    foreach (var rb in rbtns)
                        if (rb != rbtn)
                            rb.IsChecked = false;

                    audio_prev.Source = new Uri(rbtn.Tag.ToString());
                    audio_prev.AutoPlay = true;
                    audio_prev.Stop();
                };
            }

            #region Buttons
            Grid keyGrid = new Grid { };
            keyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            keyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            keyGrid.SetValue(Grid.RowProperty, 1);
            mainGrid.Children.Add(keyGrid);

            Button lBtn = new Button
            {
                Padding = new Thickness(6),
                Margin = new Thickness(3),
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(1),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = "sačuvaj",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            lBtn.Click += (s, arg) =>
            {
                foreach (var rb in rbtns)
                    if (rb.IsChecked == true)
                        Memory.Toast_Sound = rb.Tag.ToString();
                audio_prev.AutoPlay = false;
                audio_prev.Stop();
                Postavi_Stranicu();

                Set.Group_Notifications(2, 0, false, 6, 2, true);
                mainFlyout.Hide();
            };
            lBtn.SetValue(Grid.ColumnProperty, 0);
            keyGrid.Children.Add(lBtn);

            Button rBtn = new Button
            {
                Padding = new Thickness(6),
                Margin = new Thickness(3),
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(1),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = "poništi",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            rBtn.Click += (s, arg) =>
            {
                audio_prev.AutoPlay = false;
                audio_prev.Stop();
                mainFlyout.Hide();
            };
            mainFlyout.Closed += (s, arg) =>
            {
                audio_prev.AutoPlay = false;
                audio_prev.Stop();
            };
            rBtn.SetValue(Grid.ColumnProperty, 1);
            keyGrid.Children.Add(rBtn);
            #endregion

            mainFlyout.ShowAt(this);
        }

        private void livetile_TS_Toggled(object sender, RoutedEventArgs e)
        {
            if (ucitavam) return;
            Memory.Live_Tile = livetile_TS.IsOn;

            if (Memory.Live_Tile) LiveTile.Update();
            else LiveTile.Reset();

            Postavi_Stranicu();
        }

        private async void contactBtn_Click(object sender, RoutedEventArgs e)
        {
            string mail_subject = "Vaktija za Windows10";
            await Windows.System.Launcher.LaunchUriAsync(new Uri("mailto:windows@vaktija.ba?subject=" + mail_subject));
        }
        private async void rateBtn_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName));
        }
    }
}
