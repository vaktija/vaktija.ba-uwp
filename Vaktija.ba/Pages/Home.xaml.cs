using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vaktija.ba.Helpers;
using Vaktija.ba.Views;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Vaktija.ba.Pages
{
    public sealed partial class Home : Page
    {
        List<DispatcherTimer> dts = new List<DispatcherTimer>();

        bool calendarShowed = false;

        public Home()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += (sender, args) =>
            {
                Header_Set();
                Body_Set();
            };
            Loaded += (sendeer, args) =>
            {
            };
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Application.Current.Resuming += (s, ar) =>
            {
                try
                {
                    if (Memory.Live_Tile) LiveTile.Update(); /* Update / reset live tile */
                    else LiveTile.Reset();

                    Header_Set();
                    Body_Set();
                }
                catch { }
            };

            var cmp = Windows.Devices.Sensors.Compass.GetDefault();
            if (cmp == null)
            {
                Compass_Btn.Visibility = Visibility.Collapsed;
                Compass_Btn_2.Visibility = Visibility.Collapsed;
            }
            else
            {
                Compass_Btn.Visibility = Visibility.Visible;
                Compass_Btn_2.Visibility = Visibility.Visible;
            }

            if (Memory.Live_Tile) LiveTile.Update(); /* Update / reset live tile */
            else LiveTile.Reset();

            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.ShareTextHandler);

            if (Fixed.IsPhone)
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            Header_Set();
            Body_Set();

            if (MainPage.firstUpad) //ako ulazi u app postavi grupne notifijacije
            {
                await Task.Delay(2000);
                MainPage.firstUpad = false;
                Set.Group_Notifications(2, 0, false);
            }

        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (Fixed.IsPhone)
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (calendarShowed)
            {
                e.Handled = true;
                Hide_Calendar();
                return;
            }
            if (MainPage.Theme_Changed)
            {
                e.Handled = true;
                Application.Current.Exit();
            }
        }

        private void Refresh_Btn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
        private void Calendar_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (calendarShowed)
            {
                Hide_Calendar();
                return;
            }
            Make_Calendar();
        }
        private void Compass_Btn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(QiblaCompass));
        }
        private void Settings_Btn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Settings));
        }
        private void Share_Btn_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            DateTime dt = DateTime.Now.AddDays(Memory.HijriException);
            request.Data.Properties.Title = "Vaktija za " + Memory.location.ime.Replace("ž", "z").Replace("ć", "c").Replace("č", "c").Replace("š", "s").Replace("đ", "dj").Replace("Ž", "Z").Replace("Ć", "C").Replace("Č", "C").Replace("Š", "S").Replace("Đ", "Dj");
            string desc = dt.Day + ". " + Get.Name_Of_Month(dt.Month) + " " + dt.Year + "." + "\n" + HijriDate.Get(dt).day + ". " + Get.Name_Of_Month_Hijri(HijriDate.Get(dt).month) + " " + HijriDate.Get(dt).year + ".";
            Day dan = Year.year.months[dt.Month - 1].days[dt.Day - 1];
            string content_string = desc;
            foreach (var it in dan.vakti)
            {
                content_string += "\n" + it.time.ToString("HH:mm") + " " + it.name;
            }
            content_string += "\n" + "https://www.vaktija.ba";
            content_string = content_string.Replace("ž", "z").Replace("ć", "c").Replace("č", "c").Replace("š", "s").Replace("đ", "dj").Replace("Ž", "Z").Replace("Ć", "C").Replace("Č", "C").Replace("Š", "S").Replace("Đ", "Dj").Replace("Izlazak Sunca", "Izl Sunca");
            request.Data.SetText(content_string.ToLower());
        }
        private async void Rate_Btn_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName));
        }

        private void Header_Set()
        {
            string date_str = Get.Name_Of_Day_In_Week((int)DateTime.Now.DayOfWeek) + ", " + DateTime.Now.Day + ". " + Get.Name_Of_Month(DateTime.Now.Month) + " " + DateTime.Now.Year + ".";
            if (Memory.Hijri_Date_In_App)
            {
                date_str += " / " + HijriDate.Get(DateTime.Now).day + ". " + Get.Name_Of_Month_Hijri(HijriDate.Get(DateTime.Now).month) + " " + HijriDate.Get(DateTime.Now).year + ".";
            }
            if (!Memory.Date_Show_In_App) date_str = "";

            ApplicationView currentView = ApplicationView.GetForCurrentView();
            if (currentView.Orientation == ApplicationViewOrientation.Portrait)
            {
                contentPanel.RowDefinitions.Clear();
                contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                Set.PageHeader(this, header_Grid, date_str.ToLower(), Memory.location.ime.ToLower(), "ChooseLocation", true);
            }
            else if (currentView.Orientation == ApplicationViewOrientation.Landscape)
            {
                contentPanel.RowDefinitions.Clear();
                contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Star) });
                contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                Set.PageHeader(this, header_Grid, date_str.ToLower(), Memory.location.ime.ToLower(), "ChooseLocation", false);
            }
        }
        private void Body_Set()
        {
            ApplicationView currentView = ApplicationView.GetForCurrentView();
            if (currentView.Orientation == ApplicationViewOrientation.Portrait)
            {
                Body_Set_For_Portrait();
            }
            else if (currentView.Orientation == ApplicationViewOrientation.Landscape)
            {
                Body_Set_For_Landscape();
            }
        }

        //Portrait
        private async void Body_Set_For_Portrait()
        {
            BottomAppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Minimal;
            gl_SV.DisplayMode = SplitViewDisplayMode.Overlay;
            main_Grid.Children.Clear();

            Grid content_Grid = new Grid();
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            main_Grid.Children.Add(content_Grid);

            Day danas = Year.year.months[DateTime.Now.Month - 1].days[DateTime.Now.Day - 1];
            Day sutra = Year.year.months[DateTime.Now.AddDays(1).Month - 1].days[DateTime.Now.AddDays(1).Day - 1];

            for (int i = 0; i < danas.vakti.Count; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50));
                if (danas.vakti[i].time > DateTime.Now)
                {
                    Show_Content_For_Portrait(content_Grid, danas.vakti[i]);
                }
                else
                {
                    Show_Content_For_Portrait(content_Grid, sutra.vakti[i]);
                }
            }
        }
        private void Show_Content_For_Portrait(Grid _listview, Vakat v)
        {
            try
            {
                string tema = "";
                if (Fixed.IsDarkTheme) tema = "_white"; //dodatak na imena ikonica

                Vakat vakat = Year.year.months[DateTime.Now.Month - 1].days[DateTime.Now.Day - 1].vakti[v.rbr];

                if (vakat.time.DayOfWeek == DayOfWeek.Friday && vakat.rbr == 2) vakat.name = "Podne (Džuma)"; //Ako je petak i vakat podna, postavit dzumu

                #region ListViewItem
                ListViewItem _ListViewItem = new ListViewItem
                {
                    Tag = v,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    Padding = new Thickness(6, 0, 6, 0),
                    Margin = new Thickness(6, 0, 6, 0),
                    Opacity = 0,
                    MinHeight = 0,
                    MinWidth = 0,
                };
                #region border line
                if (vakat.rbr != 5)
                {
                    if (Fixed.IsDarkTheme)
                        _ListViewItem.BorderBrush = new SolidColorBrush(Color.FromArgb(18, 255, 255, 255));
                    else
                        _ListViewItem.BorderBrush = new SolidColorBrush(Color.FromArgb(18, 0, 0, 0));
                    _ListViewItem.BorderThickness = new Thickness(0, 0, 0, 1);
                }
                #endregion
                _ListViewItem.Tapped += (s, e) =>
                {
                    ListViewItem gr = s as ListViewItem;
                    Vakat vkt = (Vakat)gr.Tag;
                    if (DateTime.Now < vkt.time)
                        Open_Edit_Panel(vkt);
                };
                if (Fixed.IsPhone)
                    _ListViewItem.Holding += async (s, e) =>
                    {
                        PopupMenu menu = new PopupMenu();

                        ListViewItem gr = s as ListViewItem;
                        Vakat vkt = (Vakat)gr.Tag;

                        e.Handled = true;

                        var pointTransform = ((ListViewItem)s).TransformToVisual(Window.Current.Content);
                        double w = e.GetPosition((ListViewItem)s).X;
                        double h = e.GetPosition((ListViewItem)s).Y;

                        if (e.HoldingState == HoldingState.Started)
                        {
                            menu.Commands.Clear();

                            #region Alarm
                            if (!Memory.alarmBlocked[vkt.rbr] && !AlarmSkipping.IsSkipped(vkt))
                            {
                                menu.Commands.Add(new UICommand
                                {
                                    Id = vkt,
                                    Label = "Preskoči alarm",
                                    Invoked = AlarmOff
                                });
                            }
                            else
                            {
                                menu.Commands.Add(new UICommand
                                {
                                    Id = vkt,
                                    Label = "Uključi alarm",
                                    Invoked = AlarmOn
                                });
                            }
                            #endregion

                            #region Toast
                            if (!Memory.toastBlocked[vkt.rbr] && !ToastSkipping.IsSkipped(vkt))
                            {
                                menu.Commands.Add(new UICommand
                                {
                                    Id = vkt,
                                    Label = "Preskoči notifikaciju",
                                    Invoked = ToastOff,
                                });
                            }
                            else
                            {
                                menu.Commands.Add(new UICommand
                                {
                                    Id = vkt,
                                    Label = "Uključi notifikaciju",
                                    Invoked = ToastOn,
                                });
                            }
                            #endregion

                            Point screenCoords = new Point();
                            screenCoords = pointTransform.TransformPoint(new Point(w, h));

                            await menu.ShowForSelectionAsync(new Rect { X = screenCoords.X, Y = screenCoords.Y });
                        }
                    };
                else
                    _ListViewItem.RightTapped += async (s, e) =>
                    {
                        ListViewItem gr = s as ListViewItem;
                        Vakat vkt = (Vakat)gr.Tag;

                        PopupMenu menu = new PopupMenu();

                        #region Alarm
                        if (!Memory.alarmBlocked[vkt.rbr] && !AlarmSkipping.IsSkipped(vkt))
                        {
                            menu.Commands.Add(new UICommand
                            {
                                Id = vkt,
                                Label = "Preskoči alarm",
                                Invoked = AlarmOff
                            });
                        }
                        else
                        {
                            menu.Commands.Add(new UICommand
                            {
                                Id = vkt,
                                Label = "Uključi alarm",
                                Invoked = AlarmOn
                            });
                        }
                        #endregion

                        #region Toast
                        if (!Memory.toastBlocked[vkt.rbr] && !ToastSkipping.IsSkipped(vkt))
                        {
                            menu.Commands.Add(new UICommand
                            {
                                Id = vkt,
                                Label = "Preskoči notifikaciju",
                                Invoked = ToastOff,
                            });
                        }
                        else
                        {
                            menu.Commands.Add(new UICommand
                            {
                                Id = vkt,
                                Label = "Uključi notifikaciju",
                                Invoked = ToastOn,
                            });
                        }
                        #endregion

                        var pointTransform = ((ListViewItem)s).TransformToVisual(Window.Current.Content);
                        double w = e.GetPosition((ListViewItem)s).X;
                        double h = e.GetPosition((ListViewItem)s).Y;

                        Point screenCoords = new Point();
                        screenCoords = pointTransform.TransformPoint(new Point(w, h));

                        await menu.ShowForSelectionAsync(new Rect { X = screenCoords.X, Y = screenCoords.Y });

                        e.Handled = true;
                    };

                _ListViewItem.SetValue(Grid.RowProperty, v.rbr);
                _listview.Children.Add(_ListViewItem);
                #endregion

                double dif_bet_date = (v.time - DateTime.Now).TotalSeconds;
                string toastkey = Notification.Create_Key(v, "toast");
                string alarmkey = Notification.Create_Key(v, "alarm");
                Vakat current_prayer = Get.Current_Prayer_Time();
                Vakat next_prayer = Get.Next_Prayer_Time();

                Grid Article_Grid = new Grid
                {
                    Background = new SolidColorBrush(Colors.White),
                };
                if (Fixed.IsDarkTheme) Article_Grid.Background = new SolidColorBrush(Colors.Black);

                Grid _Grid = new Grid { VerticalAlignment = VerticalAlignment.Center, };
                _Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star), });
                _Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star), });
                _Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto), });
                _Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto), });

                #region 1st column - clock
                Viewbox clock_Vb = new Viewbox
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(6),
                    Stretch = Stretch.Fill,
                };

                TextBlock clock_TB = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 44,
                    Text = vakat.time.ToString("HH:mm"),
                    FontFamily = new FontFamily("Segoe UI Semilight"),
                    OpticalMarginAlignment = OpticalMarginAlignment.TrimSideBearings,
                    FontWeight = FontWeights.Light,
                    TextAlignment = TextAlignment.Center,
                };
                if (Fixed.IsDarkTheme)
                {
                    clock_TB.Foreground = new SolidColorBrush(Colors.DarkGray);
                }
                else
                {
                    clock_TB.Foreground = new SolidColorBrush(Colors.LightGray);
                }
                Day yesterday = Year.year.months[DateTime.Now.AddDays(-1).Month - 1].days[DateTime.Now.AddDays(-1).Day - 1];
                Day today = Year.year.months[DateTime.Now.AddDays(0).Month - 1].days[DateTime.Now.AddDays(0).Day - 1];
                Day tomorow = Year.year.months[DateTime.Now.AddDays(1).Month - 1].days[DateTime.Now.AddDays(1).Day - 1];
                if (current_prayer == v || current_prayer == yesterday.vakti[v.rbr] || current_prayer == today.vakti[v.rbr] || current_prayer == tomorow.vakti[v.rbr])
                {
                    clock_TB.Foreground = new SolidColorBrush(Color.FromArgb(255, 157, 157, 0));
                    clock_TB.FontWeight = FontWeights.Normal;
                }
                clock_Vb.SetValue(Grid.ColumnProperty, 0);
                clock_Vb.Child = clock_TB;
                _Grid.Children.Add(clock_Vb);
                #endregion
                #region 2nd column - title and subtitle
                Grid center_Grid = new Grid { VerticalAlignment = VerticalAlignment.Center, };
                center_Grid.SetValue(Grid.ColumnProperty, 1);

                StackPanel center_SP = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

                TextBlock title_TB = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    FontSize = 22,
                    Text = vakat.name.ToLower(),
                    FontFamily = new FontFamily("Segoe UI Semilight"),
                };
                center_SP.Children.Add(title_TB);

                TextBlock subtitle_TB = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    FontSize = 20,
                    Text = (int)(dif_bet_date / 3600) + ":" + ((int)((dif_bet_date / 60) % 60)).ToString("00") + ":" + ((int)(dif_bet_date % 60)).ToString("00"),
                    Foreground = new SolidColorBrush(Colors.LightGray),
                    Opacity = 0,
                    FontFamily = new FontFamily("Segoe UI Semilight"),

                };
                if (Fixed.IsDarkTheme)
                {
                    subtitle_TB.Foreground = new SolidColorBrush(Colors.DarkGray);
                }
                else
                {
                    subtitle_TB.Foreground = new SolidColorBrush(Colors.LightGray);
                }
                if (next_prayer == v)
                {
                    center_SP.Children.Add(subtitle_TB);
                    subtitle_TB.Opacity = 1;
                    DispatcherTimer dt = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
                    dt.Tick += async (s, e) =>
                    {
                        //if (Get.Difference_Between_Times(DateTime.Now, v.time) < 0) { dt.Stop(); Frame.Navigate(typeof(MainPage)); }
                        dif_bet_date = (v.time - DateTime.Now).TotalSeconds;
                        if (dif_bet_date <= 0)
                        {
                            dt.Stop();
                            await Task.Delay(1000);
                            Body_Set();
                        }
                        subtitle_TB.Text = (int)(dif_bet_date / 3600) + ":" + ((int)((dif_bet_date / 60) % 60)).ToString("00") + ":" + ((int)(dif_bet_date % 60)).ToString("00");
                    };
                    dt.Start();
                }
                else
                {
                    subtitle_TB.Text = "-:--:--";
                }

                center_Grid.Children.Add(center_SP);
                _Grid.Children.Add(center_Grid);
                #endregion
                #region 3rd column - alarm icon
                Image alarm_Image = new Image
                {
                    Width = 28,
                    Height = 28,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(3, 0, 3, 0),
                    Opacity = 0,
                    Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/alarm" + tema + ".png")),
                };
                alarm_Image.SetValue(Grid.ColumnProperty, 3);
                _Grid.Children.Add(alarm_Image);

                if (!Memory.alarmBlocked[v.rbr])
                {
                    alarm_Image.Opacity = 1;
                    if (v.alarmSkipped)
                    {
                        alarm_Image.Opacity = 0.15;
                    }
                }
                #endregion
                #region 4th column - toast icon
                Image toast_Image = new Image
                {
                    Width = 28,
                    Height = 28,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(3, 0, 3, 0),
                    Opacity = 0,
                    Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/notification" + tema + ".png")),
                };
                toast_Image.SetValue(Grid.ColumnProperty, 3);
                _Grid.Children.Add(toast_Image);

                if (!Memory.toastBlocked[v.rbr])
                {
                    alarm_Image.SetValue(Grid.ColumnProperty, 2);
                    toast_Image.Opacity = 1;
                    if (v.toastSkipped)
                    {
                        toast_Image.Opacity = 0.15;
                    }
                }
                else if (Memory.alarmBlocked[v.rbr])
                {
                    alarm_Image.SetValue(Grid.ColumnProperty, 2);
                }
                else
                {
                    toast_Image.SetValue(Grid.ColumnProperty, 2);
                }
                #endregion

                Article_Grid.Children.Add(_Grid);
                _ListViewItem.Content = Article_Grid;

                #region Storyboard
                Duration duration = new Duration(TimeSpan.FromMilliseconds(250));

                DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();

                myDoubleAnimation1.Duration = duration;

                Storyboard sb = new Storyboard();
                sb.Duration = duration;

                sb.Children.Add(myDoubleAnimation1);

                Storyboard.SetTarget(myDoubleAnimation1, _ListViewItem);

                Storyboard.SetTargetProperty(myDoubleAnimation1, "Opacity");

                myDoubleAnimation1.To = 1;

                _ListViewItem.Resources.Add("unique_id", sb);
                #endregion

                sb.Begin();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Greška pri prikazivanju " + v.name + " u vrijeme " + v.time + " (" + ex.Message + ")");
            }
        }
        //Landscape
        private async void Body_Set_For_Landscape()
        {
            gl_SV.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            BottomAppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;
            main_Grid.Children.Clear();

            Grid content_Grid = new Grid();
            content_Grid.ColumnDefinitions.Add(new ColumnDefinition());
            content_Grid.ColumnDefinitions.Add(new ColumnDefinition());
            content_Grid.ColumnDefinitions.Add(new ColumnDefinition());
            content_Grid.ColumnDefinitions.Add(new ColumnDefinition());
            content_Grid.ColumnDefinitions.Add(new ColumnDefinition());
            content_Grid.ColumnDefinitions.Add(new ColumnDefinition());
            main_Grid.Children.Add(content_Grid);

            Day danas = Year.year.months[DateTime.Now.Month - 1].days[DateTime.Now.Day - 1];
            Day sutra = Year.year.months[DateTime.Now.AddDays(1).Month - 1].days[DateTime.Now.AddDays(1).Day - 1];

            for (int i = 0; i < danas.vakti.Count; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50));
                if (danas.vakti[i].time > DateTime.Now)
                {
                    Show_Content_Landscape(content_Grid, danas.vakti[i]);
                }
                else
                {
                    Show_Content_Landscape(content_Grid, sutra.vakti[i]);
                }
            }
        }
        private void Show_Content_Landscape(Grid _listview, Vakat v)
        {
            try
            {
                string tema = "";
                if (Fixed.IsDarkTheme) tema = "_white"; //dodatak na imena ikonica

                Vakat vakat = Year.year.months[DateTime.Now.Month - 1].days[DateTime.Now.Day - 1].vakti[v.rbr];

                if (vakat.time.DayOfWeek == DayOfWeek.Friday && vakat.rbr == 2) vakat.name = "Podne (Džuma)"; //Ako je petak i vakat podna, postavit dzumu

                #region ListViewItem
                ListViewItem _ListViewItem = new ListViewItem
                {
                    Tag = v,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),
                    Opacity = 0,
                    MinHeight = 0,
                    MinWidth = 0,
                };

                _ListViewItem.Tapped += (s, e) =>
                {
                    ListViewItem gr = s as ListViewItem;
                    Vakat vkt = (Vakat)gr.Tag;
                    if (DateTime.Now < vkt.time)
                        Open_Edit_Panel(vkt);
                };
                if (Fixed.IsPhone)
                    _ListViewItem.Holding += async (s, e) =>
                    {
                        PopupMenu menu = new PopupMenu();

                        ListViewItem gr = s as ListViewItem;
                        Vakat vkt = (Vakat)gr.Tag;

                        e.Handled = true;

                        var pointTransform = ((ListViewItem)s).TransformToVisual(Window.Current.Content);
                        double w = e.GetPosition((ListViewItem)s).X;
                        double h = e.GetPosition((ListViewItem)s).Y;

                        if (e.HoldingState == HoldingState.Started)
                        {
                            menu.Commands.Clear();

                            #region Alarm
                            if (!Memory.alarmBlocked[vkt.rbr] && !AlarmSkipping.IsSkipped(vkt))
                            {
                                menu.Commands.Add(new UICommand
                                {
                                    Id = vkt,
                                    Label = "Preskoči alarm",
                                    Invoked = AlarmOff
                                });
                            }
                            else
                            {
                                menu.Commands.Add(new UICommand
                                {
                                    Id = vkt,
                                    Label = "Uključi alarm",
                                    Invoked = AlarmOn
                                });
                            }
                            #endregion

                            #region Toast
                            if (!Memory.toastBlocked[vkt.rbr] && !ToastSkipping.IsSkipped(vkt))
                            {
                                menu.Commands.Add(new UICommand
                                {
                                    Id = vkt,
                                    Label = "Preskoči notifikaciju",
                                    Invoked = ToastOff,
                                });
                            }
                            else
                            {
                                menu.Commands.Add(new UICommand
                                {
                                    Id = vkt,
                                    Label = "Uključi notifikaciju",
                                    Invoked = ToastOn,
                                });
                            }
                            #endregion

                            Point screenCoords = new Point();
                            screenCoords = pointTransform.TransformPoint(new Point(w, h));

                            await menu.ShowForSelectionAsync(new Rect { X = screenCoords.X, Y = screenCoords.Y });
                        }
                    };
                else
                    _ListViewItem.RightTapped += async (s, e) =>
                    {
                        ListViewItem gr = s as ListViewItem;
                        Vakat vkt = (Vakat)gr.Tag;

                        PopupMenu menu = new PopupMenu();

                        #region Alarm
                        if (!Memory.alarmBlocked[vkt.rbr] && !AlarmSkipping.IsSkipped(vkt))
                        {
                            menu.Commands.Add(new UICommand
                            {
                                Id = vkt,
                                Label = "Preskoči alarm",
                                Invoked = AlarmOff
                            });
                        }
                        else
                        {
                            menu.Commands.Add(new UICommand
                            {
                                Id = vkt,
                                Label = "Uključi alarm",
                                Invoked = AlarmOn
                            });
                        }
                        #endregion

                        #region Toast
                        if (!Memory.toastBlocked[vkt.rbr] && !ToastSkipping.IsSkipped(vkt))
                        {
                            menu.Commands.Add(new UICommand
                            {
                                Id = vkt,
                                Label = "Preskoči notifikaciju",
                                Invoked = ToastOff,
                            });
                        }
                        else
                        {
                            menu.Commands.Add(new UICommand
                            {
                                Id = vkt,
                                Label = "Uključi notifikaciju",
                                Invoked = ToastOn,
                            });
                        }
                        #endregion

                        var pointTransform = ((ListViewItem)s).TransformToVisual(Window.Current.Content);
                        double w = e.GetPosition((ListViewItem)s).X;
                        double h = e.GetPosition((ListViewItem)s).Y;

                        Point screenCoords = new Point();
                        screenCoords = pointTransform.TransformPoint(new Point(w, h));

                        await menu.ShowForSelectionAsync(new Rect { X = screenCoords.X, Y = screenCoords.Y });

                        e.Handled = true;
                    };

                _ListViewItem.SetValue(Grid.ColumnProperty, v.rbr);
                _listview.Children.Add(_ListViewItem);
                #endregion

                double dif_bet_date = (v.time - DateTime.Now).TotalSeconds;
                string toastkey = Notification.Create_Key(v, "toast");
                string alarmkey = Notification.Create_Key(v, "alarm");
                Vakat current_prayer = Get.Current_Prayer_Time();
                Vakat next_prayer = Get.Next_Prayer_Time();


                Grid Article_Grid = new Grid
                {
                    Background = new SolidColorBrush(Colors.White),
                };
                if (Fixed.IsDarkTheme) Article_Grid.Background = new SolidColorBrush(Colors.Black);

                Grid _Grid = new Grid { VerticalAlignment = VerticalAlignment.Center, Padding = new Thickness(0, 12, 0, 12), };
                if (v.rbr != 5)
                {
                    if (Fixed.IsDarkTheme)
                        _Grid.BorderBrush = new SolidColorBrush(Color.FromArgb(18, 255, 255, 255));
                    else
                        _Grid.BorderBrush = new SolidColorBrush(Color.FromArgb(18, 0, 0, 0));
                    _Grid.BorderThickness = new Thickness(0, 0, 1, 0);
                }

                if (Fixed.IsDarkTheme)
                {
                    _Grid.Background = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    _Grid.Background = new SolidColorBrush(Colors.White);
                }

                _Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star), });
                _Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3, GridUnitType.Star), });
                _Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star), });

                #region 1st column - clock
                Viewbox clock_Vb = new Viewbox
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(6, 12, 6, 12),
                };

                TextBlock clock_TB = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 46,
                    Text = vakat.time.ToString("HH:mm"),
                    FontFamily = new FontFamily("Segoe UI Semilight"),
                    OpticalMarginAlignment = OpticalMarginAlignment.TrimSideBearings,
                    FontWeight = FontWeights.Light,
                };
                if (Fixed.IsDarkTheme)
                {
                    clock_TB.Foreground = new SolidColorBrush(Colors.DarkGray);
                }
                else
                {
                    clock_TB.Foreground = new SolidColorBrush(Colors.LightGray);
                }
                if (Fixed.IsPhone)
                {
                    clock_TB.FontSize = 32;
                }
                Day yesterday = Year.year.months[DateTime.Now.AddDays(-1).Month - 1].days[DateTime.Now.AddDays(-1).Day - 1];
                Day today = Year.year.months[DateTime.Now.AddDays(0).Month - 1].days[DateTime.Now.AddDays(0).Day - 1];
                Day tomorow = Year.year.months[DateTime.Now.AddDays(1).Month - 1].days[DateTime.Now.AddDays(1).Day - 1];
                if (current_prayer == v || current_prayer == yesterday.vakti[v.rbr] || current_prayer == today.vakti[v.rbr] || current_prayer == tomorow.vakti[v.rbr])
                {
                    clock_TB.Foreground = new SolidColorBrush(Color.FromArgb(255, 157, 157, 0));
                    clock_TB.FontWeight = FontWeights.Normal;
                }
                clock_Vb.SetValue(Grid.RowProperty, 0);
                clock_Vb.Child = clock_TB;
                _Grid.Children.Add(clock_Vb);
                #endregion
                #region 2nd column - title and subtitle
                Grid center_Grid = new Grid { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, };
                center_Grid.SetValue(Grid.RowProperty, 1);

                StackPanel center_SP = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

                TextBlock title_TB = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    FontSize = 18,
                    Text = vakat.name.ToLower(),
                    FontFamily = new FontFamily("Segoe UI Semilight"),
                };
                if (Fixed.IsPhone || this.ActualWidth <= 720)
                {
                    title_TB.Text = title_TB.Text.Replace("izlazak sunca", "izl. sunca");
                }

                TextBlock subtitle_TB = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    FontSize = 18,
                    Text = (int)(dif_bet_date / 3600) + ":" + ((int)((dif_bet_date / 60) % 60)).ToString("00") + ":" + ((int)(dif_bet_date % 60)).ToString("00"),
                    Foreground = new SolidColorBrush(Colors.LightGray),
                    Opacity = 0,
                    FontFamily = new FontFamily("Segoe UI Semilight"),

                };
                if (Fixed.IsDarkTheme)
                {
                    subtitle_TB.Foreground = new SolidColorBrush(Colors.DarkGray);
                }
                else
                {
                    subtitle_TB.Foreground = new SolidColorBrush(Colors.LightGray);
                }

                if (next_prayer == v)
                {
                    subtitle_TB.Opacity = 1;
                    DispatcherTimer dt = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
                    dt.Tick += async (s, e) =>
                    {
                        //if (Get.Difference_Between_Times(DateTime.Now, v.time) < 0) { dt.Stop(); Frame.Navigate(typeof(MainPage)); }
                        dif_bet_date = (v.time - DateTime.Now).TotalSeconds;
                        if (dif_bet_date <= 0)
                        {
                            dt.Stop();
                            await Task.Delay(1000);
                            Body_Set();
                        }
                        subtitle_TB.Text = (int)(dif_bet_date / 3600) + ":" + ((int)((dif_bet_date / 60) % 60)).ToString("00") + ":" + ((int)(dif_bet_date % 60)).ToString("00");
                    };
                    dt.Start();
                }
                else
                {
                    subtitle_TB.Text = "-:--:--";
                }

                center_SP.Children.Add(title_TB);
                center_SP.Children.Add(subtitle_TB);

                center_Grid.Children.Add(center_SP);
                _Grid.Children.Add(center_Grid);
                #endregion
                #region 3th column - alarm and toast icon
                Grid notifGrid = new Grid { HorizontalAlignment = HorizontalAlignment.Center };
                notifGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                notifGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                notifGrid.SetValue(Grid.RowProperty, 2);
                _Grid.Children.Add(notifGrid);

                #region alarm
                Image alarm_Image = new Image
                {
                    Width = 28,
                    Height = 28,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(6, 0, 6, 0),
                    Opacity = 0,
                    Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/alarm" + tema + ".png")),
                };
                alarm_Image.SetValue(Grid.ColumnProperty, 0);

                if (!Memory.alarmBlocked[v.rbr])
                {
                    notifGrid.Children.Add(alarm_Image);
                    alarm_Image.Opacity = 1;
                    if (v.alarmSkipped)
                    {
                        alarm_Image.Opacity = 0.15;
                    }
                }
                #endregion
                #region toast
                Image toast_Image = new Image
                {
                    Width = 28,
                    Height = 28,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(6, 0, 6, 0),
                    Opacity = 0,
                    Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/notification" + tema + ".png")),
                };
                toast_Image.SetValue(Grid.ColumnProperty, 1);

                if (!Memory.toastBlocked[v.rbr])
                {
                    notifGrid.Children.Add(toast_Image);
                    toast_Image.Opacity = 1;
                    if (v.toastSkipped)
                    {
                        toast_Image.Opacity = 0.15;
                    }
                }
                else if (Memory.alarmBlocked[v.rbr])
                {
                    notifGrid.Children.Add(alarm_Image);
                }
                #endregion
                #endregion

                Article_Grid.Children.Add(_Grid);
                _ListViewItem.Content = Article_Grid;

                #region Storyboard
                Duration duration = new Duration(TimeSpan.FromMilliseconds(250));

                DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();

                myDoubleAnimation1.Duration = duration;

                Storyboard sb = new Storyboard();
                sb.Duration = duration;

                sb.Children.Add(myDoubleAnimation1);

                Storyboard.SetTarget(myDoubleAnimation1, _ListViewItem);

                Storyboard.SetTargetProperty(myDoubleAnimation1, "Opacity");

                myDoubleAnimation1.To = 1;

                _ListViewItem.Resources.Add("unique_id", sb);
                #endregion

                sb.Begin();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Greška pri prikazivanju " + v.name + " u vrijeme " + v.time + " (" + ex.Message + ")");
            }
        }

        private void AlarmOff(IUICommand command)
        {
            var a = (Vakat)command.Id;
            AlarmSkipping.SkipAlarm(a);
            Notification.Delete(Notification.Create_Key(a, "alarm"));
            Body_Set();
        }
        private void AlarmOn(IUICommand command)
        {
            var a = (Vakat)command.Id;
            if (!Memory.alarmBlocked[a.rbr])
            {
                AlarmSkipping.StayAlarm(a);
                Notification.Create_New_Alarm(a);
                Body_Set();
            }
            else
            {
                Open_Edit_Panel(a);
            }
        }
        private void ToastOff(IUICommand command)
        {
            var a = (Vakat)command.Id;
            ToastSkipping.SkipToast(a);
            Notification.Delete(Notification.Create_Key(a, "toast"));
            Body_Set();
        }
        private void ToastOn(IUICommand command)
        {
            var a = (Vakat)command.Id;
            if (!Memory.toastBlocked[a.rbr])
            {
                ToastSkipping.StayToast(a);
                Notification.Create_New_Toast(a);
                Body_Set();
            }
            else
            {
                Open_Edit_Panel(a);
            }
        }
        private void Open_Edit_Panel(Vakat a)
        {
            Grid mainGrid = new Grid { MaxWidth = 360, };
            Flyout mainFlyout = new Flyout { Content = mainGrid, Placement = Windows.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Full };
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
                Text = a.time.Hour.ToString("00") + ":" + a.time.Minute.ToString("00") + " " + a.name,
                FontSize = 22,
                FontWeight = FontWeights.Bold,
            };
            panel.Children.Add(title);

            double do_vakta = Get.Difference_Between_Times(DateTime.Now, a.time);

            #region Alarm settings
            TextBlock Alarm_TextBlock = new TextBlock
            {
                FontSize = 18,
            };

            double pred_alarm = Memory.predAlarm[a.rbr];

            Alarm_TextBlock.Text = Get.Minutes_Double_Format(pred_alarm) + " prije nastupa"; //time before prayer-time

            Slider Alarm_Slider = new Slider
            {
                Minimum = 1,
                Maximum = 60,
                Value = pred_alarm,
            };
            Alarm_Slider.ValueChanged += (s, e) =>
            {
                Alarm_TextBlock.Text = Get.Minutes_Double_Format(Alarm_Slider.Value) + " prije nastupa"; //time before prayer-time
            };

            ToggleSwitch Alarm_ToggleSwitch = new ToggleSwitch
            {
                OffContent = "Isključeno",
                OnContent = "Uključeno",
            };
            Alarm_ToggleSwitch.Tag = Alarm_Slider;
            Alarm_ToggleSwitch.IsOn = !Memory.alarmBlocked[a.rbr];
            Alarm_Slider.IsEnabled = !Memory.alarmBlocked[a.rbr];

            TextBlock Alarm_ToggleSwitch_Header = new TextBlock
            {
                FontSize = 18,
                Text = "Alarm",
            };
            Alarm_ToggleSwitch.SetValue(ToggleSwitch.HeaderProperty, Alarm_ToggleSwitch_Header);
            Alarm_ToggleSwitch.Toggled += (s, e) =>
            {
                Alarm_Slider.IsEnabled = Alarm_ToggleSwitch.IsOn;
            };
            panel.Children.Add(Alarm_ToggleSwitch);
            panel.Children.Add(Alarm_TextBlock);
            panel.Children.Add(Alarm_Slider);
            #endregion

            #region Toast settings
            TextBlock Toast_TextBlock = new TextBlock
            {
                FontSize = 18,
            };

            double pred_toast = Memory.predToast[a.rbr];

            Toast_TextBlock.Text = Get.Minutes_Double_Format(pred_toast) + " prije nastupa"; //time before prayer-time

            Slider Toast_Slider = new Slider
            {
                Minimum = 1,
                Maximum = 60,
                Value = pred_toast,
            };
            Toast_Slider.ValueChanged += (s, e) =>
            {
                Toast_TextBlock.Text = Get.Minutes_Double_Format(Toast_Slider.Value) + " prije nastupa"; //time before prayer-time
            };

            ToggleSwitch Toast_ToggleSwitch = new ToggleSwitch
            {
                OffContent = "Isključeno",
                OnContent = "Uključeno",
            };
            Toast_ToggleSwitch.Tag = Toast_Slider;
            Toast_ToggleSwitch.IsOn = !Memory.toastBlocked[a.rbr];
            Toast_Slider.IsEnabled = !Memory.toastBlocked[a.rbr];

            TextBlock Toast_ToggleSwitch_Header = new TextBlock
            {
                FontSize = 18,
                Text = "Notifikacija",
            };
            Toast_ToggleSwitch.SetValue(ToggleSwitch.HeaderProperty, Toast_ToggleSwitch_Header);
            Toast_ToggleSwitch.Toggled += (s, e) =>
            {
                Toast_Slider.IsEnabled = Toast_ToggleSwitch.IsOn;
            };
            panel.Children.Add(Toast_ToggleSwitch);
            panel.Children.Add(Toast_TextBlock);
            panel.Children.Add(Toast_Slider);
            #endregion

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
            lBtn.Click += (s, e) =>
            {
                if (Alarm_ToggleSwitch.IsOn)
                {
                    var tmp1 = Memory.predAlarm;
                    var tmp2 = Memory.alarmBlocked;
                    tmp1[a.rbr] = (int)Alarm_Slider.Value;
                    tmp2[a.rbr] = false;
                    Memory.predAlarm = tmp1;
                    Memory.alarmBlocked = tmp2;
                    Notification.Create_New_Alarm(a);
                }
                else
                {
                    var tmp2 = Memory.alarmBlocked;
                    tmp2[a.rbr] = true;
                    Memory.alarmBlocked = tmp2;
                    AlarmSkipping.StayAlarm(a);
                }

                if (Toast_ToggleSwitch.IsOn)
                {
                    var tmp1 = Memory.predToast;
                    var tmp2 = Memory.toastBlocked;
                    tmp1[a.rbr] = (int)Toast_Slider.Value;
                    tmp2[a.rbr] = false;
                    Memory.predToast = tmp1;
                    Memory.toastBlocked = tmp2;
                    Notification.Create_New_Toast(a);
                }
                else
                {
                    var tmp2 = Memory.toastBlocked;
                    tmp2[a.rbr] = true;
                    Memory.toastBlocked = tmp2;
                    ToastSkipping.StayToast(a);
                }
                mainFlyout.Hide();
                loader.IsActive = true;
                Set.Group_Notifications(2, 0, false, a.rbr);
                loader.IsActive = false;
                Body_Set();
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
            rBtn.Click += (s, e) =>
            {
                mainFlyout.Hide();
            };
            rBtn.SetValue(Grid.ColumnProperty, 1);
            keyGrid.Children.Add(rBtn);
            #endregion

            #region Show dialog in Pane and what if closed Pane
            mainFlyout.ShowAt(this);
            #endregion
        }

        private void Make_Calendar()
        {
            Grid mainGrid = new Grid { HorizontalAlignment = HorizontalAlignment.Stretch, MaxWidth = 720, Background = new SolidColorBrush(Color.FromArgb(255, 31, 31, 31)), };
            if (!Fixed.IsDarkTheme) mainGrid.Background = new SolidColorBrush(Color.FromArgb(255, 244, 244, 244));

            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            mainGrid.RowDefinitions.Add(new RowDefinition());

            #region Hide dialog button
            ListViewItem hideLVI = new ListViewItem
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                MinHeight = 0,
                MinWidth = 0,
                Background = null,
                Padding = new Thickness(0),
                Margin = new Thickness(0),
                BorderBrush = new SolidColorBrush(Color.FromArgb(31, 0, 0, 0)),
                BorderThickness = new Thickness(1),
            };
            hideLVI.SetValue(Grid.RowProperty, 0);
            mainGrid.Children.Add(hideLVI);

            TextBlock hideTB = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 18,
                FontFamily = new FontFamily("Segoe UI Symbol"),
                Text = "",
                VerticalAlignment = VerticalAlignment.Center,
            };
            hideLVI.Content = hideTB;
            hideLVI.Tapped += (s, e) =>
            {
                Hide_Calendar();
            };

            #region Manipulation
            calendar_Grid.ManipulationMode = ManipulationModes.TranslateY;
            calendar_Grid.ManipulationDelta += (s, e) =>
            {
                calendarCT.TranslateY += e.Delta.Translation.Y;
                if (calendarCT.TranslateY < 0)
                    calendarCT.TranslateY = 0;

                if (calendarCT.TranslateY > 50)
                {
                    hideTB.Text = "";
                }
                else
                {
                    hideTB.Text = "";
                }
            };
            calendar_Grid.ManipulationCompleted += (s, e) =>
            {
                if (calendarCT.TranslateY > 50)
                {
                    Hide_Calendar();
                }
                else
                {
                    calendarCT.TranslateY = 0;
                }
                hideTB.Text = "";
            };
            #endregion
            #endregion

            #region Setup panel
            var panel = new StackPanel
            {
                Padding = new Thickness(12),
            };
            panel.SetValue(Grid.RowProperty, 1);
            mainGrid.Children.Add(panel);

            StackPanel headerSp = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
            DatePicker dp = new DatePicker
            {
                Margin = new Thickness(0, 12, 0, 0),
                Header = "Vaktija za datum:",
                Date = DateTime.Now,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            headerSp.Children.Add(dp);

            TextBlock hijriTB = new TextBlock
            {
                Margin = new Thickness(12),
                FontSize = 20,
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            headerSp.Children.Add(hijriTB);
            panel.Children.Add(headerSp);
            #endregion

            #region Content panel
            Grid content_Grid = new Grid();
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.SetValue(Grid.RowProperty, 2);
            mainGrid.Children.Add(content_Grid);
            #endregion

            #region Events
            dp.DateChanged += (s, args) =>
            {
                DateTime dttt = new DateTime(dp.Date.Year, dp.Date.Month, dp.Date.Day);
                Show_Special_Prayers_For_Custom_Date(content_Grid, dttt);
                hijriTB.Text = Get.Name_Of_Day_In_Week((int)dttt.AddDays(Memory.HijriException).DayOfWeek).ToLower() + ", " + HijriDate.Get(dttt).day + ". " + Get.Name_Of_Month_Hijri(HijriDate.Get(dttt).month) + " " + HijriDate.Get(dttt).year + ".";
            };
            #endregion

            #region Show dialog
            Show_Special_Prayers_For_Custom_Date(content_Grid, DateTime.Now);

            hijriTB.Text = Get.Name_Of_Day_In_Week((int)DateTime.Now.DayOfWeek).ToLower() + ", " + HijriDate.Get(DateTime.Now).day + ". " + Get.Name_Of_Month_Hijri(HijriDate.Get(DateTime.Now).month) + " " + HijriDate.Get(DateTime.Now).year + ".";

            Show_Calendar(mainGrid);
            #endregion
        }
        private void Show_Calendar(Grid grid)
        {
            calendarShowed = true;
            Calendar_Btn.Icon = new SymbolIcon(Symbol.CalendarReply);
            Calendar_Btn_InPane.Icon = new SymbolIcon(Symbol.CalendarReply);
            calendar_Grid.Children.Clear();
            calendar_Grid.Children.Add(grid);
            if (calendarCT.TranslateY == 0)
                calendarCT.TranslateY = 1000;
            //calendar_Grid.Visibility = Visibility.Visible;

            #region Storyboard

            Duration duration = new Duration(TimeSpan.FromMilliseconds(250));

            DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();

            myDoubleAnimation1.Duration = duration;

            Storyboard sb = new Storyboard();
            sb.Duration = duration;

            sb.Children.Add(myDoubleAnimation1);

            Storyboard.SetTarget(myDoubleAnimation1, calendarCT);

            Storyboard.SetTargetProperty(myDoubleAnimation1, "TranslateY");

            myDoubleAnimation1.To = 0;

            sb.Begin();
            #endregion
        }
        private void Hide_Calendar()
        {
            calendarShowed = false;
            Calendar_Btn.Icon = new SymbolIcon(Symbol.Calendar);
            Calendar_Btn_InPane.Icon = new SymbolIcon(Symbol.Calendar);

            #region Storyboard
            Duration duration = new Duration(TimeSpan.FromMilliseconds(250));

            DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();

            myDoubleAnimation1.Duration = duration;

            Storyboard sb = new Storyboard();
            sb.Duration = duration;

            sb.Children.Add(myDoubleAnimation1);

            Storyboard.SetTarget(myDoubleAnimation1, calendarCT);

            Storyboard.SetTargetProperty(myDoubleAnimation1, "TranslateY");

            myDoubleAnimation1.To = 1000;
            sb.Completed += (s, e) =>
            {
                //calendar_Grid.Visibility = Visibility.Collapsed;
            };
            sb.Begin();
            #endregion
        }
        private async void Show_Special_Prayers_For_Custom_Date(Grid lv, DateTime dateTime)
        {
            lv.Children.Clear();
            Day day = Year.year.months[dateTime.Month - 1].days[dateTime.Day - 1];

            foreach (var it in day.vakti)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                Add_Vakat_On_Board(lv, it);
            }
        }
        private void Add_Vakat_On_Board(Grid lv, Vakat vakat)
        {
            if (vakat.time.DayOfWeek == DayOfWeek.Friday && vakat.rbr == 2)
                vakat.name = "Podne (Džuma)";

            #region ListViewItem
            ListViewItem Article_Item = new ListViewItem
            {
                Tag = vakat,
                Margin = new Thickness(12, 0, 12, 0),
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Padding = new Thickness(6),
                Opacity = 0,
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(0, 0, 0, 1),
            };
            #endregion

            #region Main Grid
            Grid _Grid = new Grid
            {
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush(Color.FromArgb(51, 255, 255, 255)),
            };
            ColumnDefinition cd0 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), };
            ColumnDefinition cd1 = new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star), };
            ColumnDefinition cd2 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto), };
            _Grid.ColumnDefinitions.Add(cd0);
            _Grid.ColumnDefinitions.Add(cd1);
            _Grid.ColumnDefinitions.Add(cd2);

            #endregion

            #region Clock TextBlock
            TextBlock Clock_TextBlock = new TextBlock
            {
                Margin = new Thickness(3),
                Text = vakat.time.Hour.ToString("00") + ":" + vakat.time.Minute.ToString("00"),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 28,
                TextAlignment = TextAlignment.Right,
            };
            Clock_TextBlock.SetValue(Grid.ColumnProperty, 0);
            _Grid.Children.Add(Clock_TextBlock);
            #endregion

            #region Name TextBlock
            TextBlock Name_TextBLock = new TextBlock
            {
                Text = vakat.name,
                FontSize = 22,
                Margin = new Thickness(3),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Name_TextBLock.SetValue(Grid.ColumnProperty, 1);
            _Grid.Children.Add(Name_TextBLock);
            #endregion

            Article_Item.Content = _Grid;

            #region Storyboard
            // Create a duration of 2 seconds.
            Duration duration = new Duration(TimeSpan.FromMilliseconds(500));

            // Create two DoubleAnimations and set their properties.
            DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();

            myDoubleAnimation1.Duration = duration;

            Storyboard sb = new Storyboard();
            sb.Duration = duration;

            sb.Children.Add(myDoubleAnimation1);

            Storyboard.SetTarget(myDoubleAnimation1, Article_Item);

            // Set the attached properties of Canvas.Left and Canvas.Top
            // to be the target properties of the two respective DoubleAnimations.
            Storyboard.SetTargetProperty(myDoubleAnimation1, "Opacity");

            myDoubleAnimation1.To = 1;

            // Make the Storyboard a resource.
            Article_Item.Resources.Add("unique_id", sb);
            #endregion

            Article_Item.SetValue(Grid.RowProperty, vakat.rbr);
            lv.Children.Add(Article_Item);

            // Begin the animation.
            sb.Begin();
        }

        private void Location_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(ChooseLocation));
        }
        private void Hamburger_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gl_SV.IsPaneOpen = !gl_SV.IsPaneOpen;
        }
        private void Calendar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gl_SV.IsPaneOpen = false;
            Calendar_Btn_Click(sender, e);
        }
        private void Compass_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gl_SV.IsPaneOpen = false;
            Compass_Btn_Click(sender, e);
        }
        private void Share_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gl_SV.IsPaneOpen = false;
            Share_Btn_Click(sender, e);
        }
        private void Rate_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gl_SV.IsPaneOpen = false;
            Rate_Btn_Click(sender, e);
        }
        private void Settings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gl_SV.IsPaneOpen = false;
            Settings_Btn_Click(sender, e);
        }
    }
}