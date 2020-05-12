using System;
using System.Collections.Generic;
using Vaktija.ba.Helpers;
using Windows.ApplicationModel;
using Windows.Phone.UI.Input;
using Windows.Storage;
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
        public Settings()
        {
            this.InitializeComponent();

            DataContext = new Postavke();
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
                    Frame.Navigate(typeof(Pages.Home));
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
            Frame.Navigate(typeof(Pages.Home));
        }

        private void location_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(ChooseLocation));
        }

        private void dateShowInApp_TS_Toggled(object sender, RoutedEventArgs e)
        {
            Memory.Date_Show_In_App = dateShowInApp_TS.IsOn;
            hijriDateShowInApp_TS.IsEnabled = dateShowInApp_TS.IsOn;
        }

        private void hijriDateShowInApp_TS_Toggled(object sender, RoutedEventArgs e)
        {
            Memory.Hijri_Date_In_App = hijriDateShowInApp_TS.IsOn;
        }

        private void stdPodneCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (stdPodneCB.SelectedIndex == 0)
                Memory.Std_Podne = true;
            else
                Memory.Std_Podne = false;

            Notification.Set(Data.Obavijest.All);
        }

        private void themeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Memory.Theme == themeCB.SelectedIndex) return;

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
        }

        private async System.Threading.Tasks.Task<List<StorageFile>> Alarm_Sounds()
        {
            var assetsFolder = await Package.Current.InstalledLocation.GetFolderAsync(Fixed.App_Assets_Folder);
            var soundsFolder = await assetsFolder.GetFolderAsync(Fixed.App_Sound_Folder);
            var soundsFiles = await soundsFolder.GetFilesAsync();

            List<StorageFile> sounds = new List<StorageFile>();
            foreach (var a in soundsFiles)
                sounds.Add(a);

            return sounds;
        }

        private async void alarmsound_Tapped(object sender, TappedRoutedEventArgs e)
        {
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

            var alarmSounds = await Alarm_Sounds();
            List<RadioButton> rbtns = new List<RadioButton>();
            foreach (var it in alarmSounds)
            {
                RadioButton rbtn = new RadioButton
                {
                    Content = it.DisplayName,
                    Tag = "ms-appx:///" + Fixed.App_Assets_Folder + "/" + Fixed.App_Sound_Folder + "/" + it.Name,
                    Padding = new Thickness(6),
                    FontSize = 18,
                    IsChecked = false
                };
                lv.Items.Add(rbtn);
                if (Memory.Alarm_Sound.Contains(it.Name)) rbtn.IsChecked = true;
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
                Notification.Set(Data.Obavijest.All);
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

        private async void toastsound_Tapped(object sender, TappedRoutedEventArgs e)
        {
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

            var toastSounds = await Alarm_Sounds();
            List<RadioButton> rbtns = new List<RadioButton>();
            foreach (var it in toastSounds)
            {
                RadioButton rbtn = new RadioButton
                {
                    Content = it.DisplayName,
                    Tag = "ms-appx:///" + Fixed.App_Assets_Folder + "/" + Fixed.App_Sound_Folder + "/" + it.Name,
                    Padding = new Thickness(6),
                    FontSize = 18,
                    IsChecked = false
                };
                lv.Items.Add(rbtn);
                if (Memory.Toast_Sound.Contains(it.Name)) rbtn.IsChecked = true;
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

                Notification.Set(Data.Obavijest.All);
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
            Memory.Live_Tile = livetile_TS.IsOn;

            LiveTile.Update();
        }

        private async void contactBtn_Click(object sender, RoutedEventArgs e)
        {
            string mail_subject = "Vaktija za Windows 10" + " v" + Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor + "." + Package.Current.Id.Version.Build;
            await Windows.System.Launcher.LaunchUriAsync(new Uri("mailto:windows@vaktija.ba?subject=" + mail_subject));
        }

        private async void rateBtn_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName));
        }
    }

    class Postavke
    {
        public Brush background {
            get
            {
                if (Fixed.IsDarkTheme) return new SolidColorBrush(Color.FromArgb(255, 31, 31, 31));
                else return new SolidColorBrush(Color.FromArgb(255, 241, 241, 241));
            }
        }
        public string location { get => Data.data.locations[Memory.location]; }
        public bool dateInApp { get => Memory.Date_Show_In_App; }
        public bool hijriDateInApp { get => Memory.Hijri_Date_In_App; }
        public int stdPodne
        {
            get
            {
                if (Memory.Std_Podne) return 0;
                else return 1;
            }
        }
        public int themeSelect { get => Memory.Theme; }
        public string alarmName { get => Memory.Alarm_Sound.Replace("ms-appx:///Assets/Sounds/", "").Replace(".wav", "").Replace(".mp3", "").Replace(".ogg", "").Replace("ms-winsoundevent:Notification.Looping.", "").Replace("ms-winsoundevent:Notification.", ""); }
        public string toastName { get => Memory.Toast_Sound.Replace("ms-appx:///Assets/Sounds/", "").Replace(".wav", "").Replace(".mp3", "").Replace(".ogg", "").Replace("ms-winsoundevent:Notification.Looping.", "").Replace("ms-winsoundevent:Notification.", ""); }
        public bool livetile { get => Memory.Live_Tile; }
        public string appName { get => Fixed.App_Name; }
        public string appVersion { get => "v " + Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor + "." + Package.Current.Id.Version.Build; }
        public string appDevs { get => "by " + Fixed.App_Developer; }
    }
}
