using System;
using Vaktija.ba.Helpers;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Vaktija.ba.Pages
{
    public sealed partial class QiblaCompass : Page
    {
        double od_kompasa = 0;
        double qibla_angle = 0;
        MagnetometerAccuracy accuracy = new MagnetometerAccuracy();
        bool ucitano = false;
        DispatcherTimer timer = new DispatcherTimer();
        Compass _compass = Compass.GetDefault();

        public QiblaCompass()
        {
            this.InitializeComponent();

            DataContext = new HomePageShow();
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

            ucitano = false;

            _compass = Windows.Devices.Sensors.Compass.GetDefault();

            if (_compass == null)
            {
                NoCompass_Grid.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                _compass.ReadingChanged += _compass_ReadingChanged;

                try
                {
                    Postavi_Stranicu(Memory.location, Data.data.coordinates[Memory.location].ime, Data.data.coordinates[Memory.location].latitude, Data.data.coordinates[Memory.location].longitude);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Greška pri postavljanju stranice " + " (" + ex.Message + ")");
                }
            }
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            timer.Stop();
            if (Fixed.IsPhone)
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Frame.GoBack();
        }

        private void Postavi_Stranicu(int id, string ime, double lat1, double long1)
        {
            System.Diagnostics.Debug.WriteLine("Coordinates " + " (" + lat1 + " : " + long1 + ")");

            if (Fixed.IsDarkTheme)
                _pivot.Background = new SolidColorBrush(Color.FromArgb(255, 31, 31, 31));
            else
                _pivot.Background = new SolidColorBrush(Color.FromArgb(255, 241, 241, 241));

            try
            {
                kompasiFlipView.SelectedIndex = Memory.KompasBroj;
            }
            catch { }

            ucitano = true;

            #region Računanje ugla između pravca prema sjeveru i prema Kibli
            double lat2 = 21.4225; //Qibla's geocoordinates
            double long2 = 39.8261;

            double ugao = Math.Atan(Math.Sin(long2 - long1) / (Math.Cos(lat1) * Math.Tan(lat2) - Math.Sin(lat1) * Math.Cos(long2 - long1)));
            ugao *= 180 / Math.PI;

            double pox = long1 - long2;
            double poy = lat1 - lat2;
            double poc = Math.Sqrt((long1 - long2)*(long1 - long2) + (lat1 - lat2)*(lat1 - lat2));
            qibla_angle = Math.Acos(pox / poc) * 180 / Math.PI; //Ugao između pravaca prema sjeveru i prema Kibli za koordinate (lat1, long1)
            #endregion

            locationPivotItem.Header = ime;

            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void _compass_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            od_kompasa = args.Reading.HeadingTrueNorth.Value - qibla_angle;
            accuracy = args.Reading.HeadingAccuracy;
        }
        private void Timer_Tick(object sender, object e)
        {
            compassCT1.Rotation = -od_kompasa;
            statusTB.Text = "Ugao: " + (int)compassCT1.Rotation + "°";

            if ((int)compassCT1.Rotation == 0)
            {
                kompasImg.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/za_kompas.png"));
            }
            else if ((int)compassCT1.Rotation > -2 && (int)compassCT1.Rotation < 2)
            {
                kompasImg.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/za_kompas.png"));
            }
            else
            {
                kompasImg.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/za_kompas_gray.png"));
            }

            if (accuracy != MagnetometerAccuracy.High)
            {
                Calibration_Grid.Visibility = Visibility.Visible;
            }
            else
            {
                Calibration_Grid.Visibility = Visibility.Collapsed;
            }
        }

        private async void Locate_Me_Btn_Click(object sender, RoutedEventArgs e)
        {
            var cmp = Compass.GetDefault();

            if (cmp == null) return;
            try
            {
                Geolocator geolocator = new Geolocator();
                Geoposition geoposition = await geolocator.GetGeopositionAsync();
                Postavi_Stranicu(-1, "Moja lokacija", geoposition.Coordinate.Point.Position.Latitude, geoposition.Coordinate.Point.Position.Longitude);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Greška pri uzimanju lokacije uređaja " + "(" + ex.Message + ")");

                #region Poruka za korisnika
                MessageDialog ms = new MessageDialog("Vaše lokacijske postavke su isključene.\nŽelite li uključiti lokacijske postavke uređaja?", Fixed.App_Name);
                UICommand settingsBtn = new UICommand("da");
                settingsBtn.Invoked = settingsBtnClick;
                ms.Commands.Add(settingsBtn);
                UICommand noBtn = new UICommand("ne");
                noBtn.Invoked = NoBtnClick;
                ms.Commands.Add(noBtn);
                await ms.ShowAsync();
                #endregion
            }
        }
        private void NoBtnClick(IUICommand command)
        {

        }
        private async void settingsBtnClick(IUICommand command)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
        }

        private void kompasiFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ucitano)
                Memory.KompasBroj = kompasiFlipView.SelectedIndex;
        }
    }
    class ChooseLocationShow
    {
        public Brush foregroundColor
        {
            get
            {
                if (Fixed.IsDarkTheme) return new SolidColorBrush(Colors.LightGray);
                else return new SolidColorBrush(Colors.DarkGray);
            }
        }
        public Brush backgroundColor
        {
            get
            {
                if (Fixed.IsDarkTheme) return new SolidColorBrush(Colors.Black);
                else return new SolidColorBrush(Colors.White);
            }
        }
        public string lokacija
        {
            get
            {
                return Data.data.locations[Memory.location];
            }
        }
    }

}
