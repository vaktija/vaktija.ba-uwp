using System;
using System.Threading.Tasks;
using Vaktija.ba.Helpers;
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
        private int alarmslidervalue = 0;
        private int toastslidervalue = 0;
        private bool alarmonoff = false;
        private bool toastonoff = false;

        public Home()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += (sender, args) =>
            {
                Postavi_Stranicu();
            };

            DataContext = new HomePageShow();
        }

        public async void Postavi_Stranicu()
        {
            main_Grid.Children.Clear();

            Grid content_Grid = new Grid();

            if (ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape)
            {
                contentPanel.RowDefinitions.Clear();
                contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Star) });
                contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

                gl_SV.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                BottomAppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;

                content_Grid.ColumnDefinitions.Add(new ColumnDefinition());
                content_Grid.ColumnDefinitions.Add(new ColumnDefinition());
                content_Grid.ColumnDefinitions.Add(new ColumnDefinition());
                content_Grid.ColumnDefinitions.Add(new ColumnDefinition());
                content_Grid.ColumnDefinitions.Add(new ColumnDefinition());
                content_Grid.ColumnDefinitions.Add(new ColumnDefinition());

            }
            else
            {
                contentPanel.RowDefinitions.Clear();
                contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(6, GridUnitType.Star) });
                contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

                gl_SV.DisplayMode = SplitViewDisplayMode.Overlay;
                BottomAppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Minimal;

                content_Grid.RowDefinitions.Add(new RowDefinition());
                content_Grid.RowDefinitions.Add(new RowDefinition());
                content_Grid.RowDefinitions.Add(new RowDefinition());
                content_Grid.RowDefinitions.Add(new RowDefinition());
                content_Grid.RowDefinitions.Add(new RowDefinition());
                content_Grid.RowDefinitions.Add(new RowDefinition());

            }

            main_Grid.Children.Add(content_Grid);

            for (int i = 0; i < Data.data.vakatNames.Count; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50));
                content_Grid.Children.Add(Get_Main_ListViewItem(new Vakat { name = Data.data.vakatNames[i], rbr = i, time = Data.VakatTime(DateTime.Now, i) }));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.ShareTextHandler);

            if (Fixed.IsPhone)
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            Postavi_Stranicu();

            Notification.Set(Data.Obavijest.All); // Update notifications

            LiveTile.Update(); // Update live tile
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (Fixed.IsPhone)
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;

            if (calendarCT.TranslateY == 0)
            {
                Hide_Calendar();
                return;
            }

            Application.Current.Exit();
        }

        #region MAIN CONTENT PANEL
        private ListViewItem Get_Main_ListViewItem(Vakat vakat)
        {
            if (vakat.time.DayOfWeek == DayOfWeek.Friday && vakat.rbr == 2)
                vakat.name = "Podne (Džuma)";

            ListViewItem _ListViewItem = new ListViewItem
            {
                Tag = vakat,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                Opacity = 0,
                MinHeight = 0,
                MinWidth = 0,
            };

            _ListViewItem.Tapped += Vakat_ListViewItem_Tapped;

            if (Fixed.IsPhone)
                _ListViewItem.Holding += Vakat_ListViewItem_Holding;
            else
                _ListViewItem.RightTapped += Vakat_ListViewItem_RightTapped;

            _ListViewItem.SetValue(Grid.ColumnProperty, vakat.rbr);
            _ListViewItem.SetValue(Grid.RowProperty, vakat.rbr);

            _ListViewItem.Content = Get_Main_Item_Grid(vakat);

            foreach(var i in Resources)
            {
                if(i.Key.ToString() == vakat.MemoryKey)
                {
                    Resources.Remove(vakat.MemoryKey);
                }
            }

            this.Resources.Add(vakat.MemoryKey, Get_Storyboard_For_Vakat(_ListViewItem)); // Storyboard animation - FADE IN

            return _ListViewItem;
        }

        private Grid Get_Main_Item_Grid(Vakat vakat)
        {
            Grid Article_Grid = new Grid
            {
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                BorderBrush = new SolidColorBrush(Color.FromArgb(18, 0, 0, 0)),
            };

            if (Fixed.IsDarkTheme)
            {
            }

            Grid _Grid = new Grid
            {
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(0),
                BorderBrush = new SolidColorBrush(Color.FromArgb(18, 0, 0, 0)),
            };

            if(ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape)
            {
                Article_Grid.BorderThickness = new Thickness(0);

                _Grid.BorderThickness = new Thickness(0, 0, 1, 0);

                _Grid.VerticalAlignment = VerticalAlignment.Center;

                _Grid.Padding = new Thickness(0, 12, 0, 12);

                _Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star), });
                _Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3, GridUnitType.Star), });
                _Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star), });
            }
            else
            {
                Article_Grid.BorderThickness = new Thickness(0, 0, 0, 1);

                _Grid.Padding = new Thickness(12, 0, 12, 0);

                _Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star), });
                _Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star), });
                _Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto), });
            }

            if (vakat.rbr == (Data.data.vakatNames.Count - 1))
            {
                Article_Grid.BorderThickness = new Thickness(0, 0, 0, 0);
                _Grid.BorderThickness = new Thickness(0, 0, 0, 0);
            }

            if (Fixed.IsDarkTheme)
            {
                Article_Grid.BorderBrush = new SolidColorBrush(Color.FromArgb(18, 255, 255, 255));
                Article_Grid.Background = new SolidColorBrush(Colors.Black);

                _Grid.BorderBrush = new SolidColorBrush(Color.FromArgb(18, 255, 255, 255));
                _Grid.Background = new SolidColorBrush(Colors.Black);
            }

            _Grid.Children.Add(Get_Time(vakat));

            _Grid.Children.Add(Get_Center_Item_Grid(vakat));

            _Grid.Children.Add(Get_Icons_Grid(vakat));

            Article_Grid.Children.Add(_Grid);

            return Article_Grid;
        }

        private Viewbox Get_Time(Vakat vakat)
        {
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
                Foreground = new SolidColorBrush(Colors.DarkGray),
            };

            if (Fixed.IsDarkTheme) clock_TB.Foreground = new SolidColorBrush(Colors.LightGray);

            if (Fixed.IsPhone) clock_TB.FontSize = 32;

            if (vakat.isNext)
            {
                clock_TB.Foreground = new SolidColorBrush(Color.FromArgb(255, 165, 149, 115));
                clock_TB.FontWeight = FontWeights.Normal;
            }

            clock_Vb.Child = clock_TB;

            clock_Vb.SetValue(Grid.RowProperty, 0);
            clock_Vb.SetValue(Grid.ColumnProperty, 0);

            return clock_Vb;
        }

        private Grid Get_Center_Item_Grid(Vakat vakat)
        {
            Grid center_Grid = new Grid { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, };

            StackPanel center_SP = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

            center_SP.Children.Add(Get_Title(vakat));

            center_SP.Children.Add(Get_Counter(vakat));

            center_Grid.Children.Add(center_SP);

            if(ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape)
            {
                center_Grid.SetValue(Grid.RowProperty, 1);
            }
            else
            {
                center_Grid.SetValue(Grid.ColumnProperty, 1);
            }

            return center_Grid;
        }

        private TextBlock Get_Title(Vakat vakat)
        {
            TextBlock title_TB = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontSize = 22,
                Text = vakat.name.ToLower(),
                FontFamily = new FontFamily("Segoe UI Semilight"),
            };
            if (Fixed.IsPhone || this.ActualWidth <= 720)
                title_TB.Text = title_TB.Text.Replace("izlazak sunca", "izl. sunca");

            return title_TB;
        }

        private TextBlock Get_Counter(Vakat vakat)
        {
            double dif_bet_date = (vakat.time - DateTime.Now).TotalSeconds;

            TextBlock subtitle_TB = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontSize = 18,
                Text = (int)(dif_bet_date / 3600) + ":" + ((int)((dif_bet_date / 60) % 60)).ToString("00") + ":" + ((int)(dif_bet_date % 60)).ToString("00"),
                Foreground = new SolidColorBrush(Colors.DarkGray),
                Opacity = 0,
                FontFamily = new FontFamily("Segoe UI Semilight"),
            };
            if (Fixed.IsDarkTheme)
                subtitle_TB.Foreground = new SolidColorBrush(Colors.LightGray);

            if (vakat.isNext)
            {
                subtitle_TB.Opacity = 1;
                DispatcherTimer dt = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
                dt.Tick += async (s, e) =>
                {
                    dif_bet_date = (vakat.time - DateTime.Now).TotalSeconds;
                    if (dif_bet_date <= 0)
                    {
                        dt.Stop();
                        await Task.Delay(1000);
                        Postavi_Stranicu();
                    }
                    subtitle_TB.Text = (int)(dif_bet_date / 3600) + ":" + ((int)((dif_bet_date / 60) % 60)).ToString("00") + ":" + ((int)(dif_bet_date % 60)).ToString("00");
                };
                dt.Start();
            }

            return subtitle_TB;
        }

        private Grid Get_Icons_Grid(Vakat vakat)
        {
            Grid notifGrid = new Grid { HorizontalAlignment = HorizontalAlignment.Center };

            notifGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            notifGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            if(ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape)
            {
                notifGrid.SetValue(Grid.RowProperty, 2);
            }
            else
            {
                notifGrid.SetValue(Grid.ColumnProperty, 2);
                notifGrid.HorizontalAlignment = HorizontalAlignment.Right;
            }


            notifGrid.Children.Add(Get_Alarm_Image(vakat));
            notifGrid.Children.Add(Get_Toast_Image(vakat));

            return notifGrid;
        }

        private Image Get_Alarm_Image(Vakat vakat)
        {
            if (DateTime.Now >= vakat.time)
                vakat = new Vakat { rbr = vakat.rbr, name = vakat.name, time = Data.VakatTime(vakat.time.AddDays(1), vakat.rbr) };

            Image alarm_Image = new Image
            {
                Width = 28,
                Height = 28,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(6, 0, 6, 0),
                Opacity = 1,
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/alarm.png")),
            };

            if (Fixed.IsDarkTheme)
                alarm_Image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/alarm_white.png"));

            if (vakat.blockedAlarm)
                alarm_Image.Opacity = 0;

            if (vakat.isSkipped(Data.Obavijest.Alarm))
                alarm_Image.Opacity = 0.15;

            if(ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Portrait)
            {
                if (vakat.blockedToast && !vakat.blockedAlarm)
                    alarm_Image.SetValue(Grid.ColumnProperty, 1);
            }
            else
            {
                if (!vakat.blockedToast && vakat.blockedAlarm)
                    alarm_Image.Visibility = Visibility.Collapsed;
            }


            return alarm_Image;
        }

        private Image Get_Toast_Image(Vakat vakat)
        {
            if (DateTime.Now >= vakat.time)
                vakat = new Vakat { rbr = vakat.rbr, name = vakat.name, time = Data.VakatTime(vakat.time.AddDays(1), vakat.rbr) };

            Image toast_Image = new Image
            {
                Width = 28,
                Height = 28,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(6, 0, 6, 0),
                Opacity = 1,
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/notification.png")),
            };

            if (Fixed.IsDarkTheme)
                toast_Image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/notification_white.png"));

            if (vakat.blockedToast)
                toast_Image.Opacity = 0;

            if (vakat.isSkipped(Data.Obavijest.Toast))
                toast_Image.Opacity = 0.15;

            toast_Image.SetValue(Grid.ColumnProperty, 1);

            if (ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Portrait)
            {
                if (vakat.blockedToast && !vakat.blockedAlarm)
                {
                    toast_Image.SetValue(Grid.ColumnProperty, 0);
                }
            }
            else
            {
                if (!vakat.blockedAlarm && vakat.blockedToast)
                    toast_Image.Visibility = Visibility.Collapsed;
            }


            return toast_Image;
        }

        private Storyboard Get_Storyboard_For_Vakat(ListViewItem ob)
        {
            Duration duration = new Duration(TimeSpan.FromMilliseconds(250));

            DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();

            myDoubleAnimation1.Duration = duration;

            Storyboard sb = new Storyboard();
            sb.Duration = duration;

            sb.Children.Add(myDoubleAnimation1);

            Storyboard.SetTarget(myDoubleAnimation1, ob);

            Storyboard.SetTargetProperty(myDoubleAnimation1, "Opacity");

            myDoubleAnimation1.To = 1;

            sb.Begin();

            return sb;
        }
        #endregion

        #region VAKAT - MANIPULATION EVENTS
        private async void Vakat_ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            ListViewItem gr = sender as ListViewItem;
            Vakat vkt = (Vakat)gr.Tag;

            if (DateTime.Now >= vkt.time)
            {
                Vakat vktTMP = new Vakat { rbr = vkt.rbr, name = vkt.name, time = Data.VakatTime(vkt.time.AddDays(1), vkt.rbr) };
                vkt = vktTMP;
            }

            PopupMenu menu = new PopupMenu();

            menu.Commands.Add(new UICommand
            {
                Id = vkt,
                Label = vkt.name,
                Invoked = OpenEditPanelByMenu
            });

            #region Alarm
            if (vkt.blockedAlarm || vkt.isSkipped(Data.Obavijest.Alarm))
            {
                menu.Commands.Add(new UICommand
                {
                    Id = vkt,
                    Label = "Uključi alarm",
                    Invoked = AlarmOn
                });
            }
            else
            {
                menu.Commands.Add(new UICommand
                {
                    Id = vkt,
                    Label = "Preskoči alarm",
                    Invoked = AlarmOff
                });
            }
            #endregion

            #region Toast
            if (vkt.blockedToast || vkt.isSkipped(Data.Obavijest.Toast))
            {
                menu.Commands.Add(new UICommand
                {
                    Id = vkt,
                    Label = "Uključi notifikaciju",
                    Invoked = ToastOn,
                });
            }
            else
            {
                menu.Commands.Add(new UICommand
                {
                    Id = vkt,
                    Label = "Preskoči notifikaciju",
                    Invoked = ToastOff,
                });
            }
            #endregion

            var pointTransform = ((ListViewItem)sender).TransformToVisual(Window.Current.Content);
            double w = e.GetPosition((ListViewItem)sender).X;
            double h = e.GetPosition((ListViewItem)sender).Y;

            Point screenCoords = new Point();
            screenCoords = pointTransform.TransformPoint(new Point(w, h));

            await menu.ShowForSelectionAsync(new Rect { X = screenCoords.X, Y = screenCoords.Y });

            e.Handled = true;
        }
        private async void Vakat_ListViewItem_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            PopupMenu menu = new PopupMenu();

            ListViewItem gr = sender as ListViewItem;
            Vakat vkt = (Vakat)gr.Tag;

            if (DateTime.Now >= vkt.time)
            {
                Vakat vktTMP = new Vakat { rbr = vkt.rbr, name = vkt.name, time = Data.VakatTime(vkt.time.AddDays(1), vkt.rbr) };
                vkt = vktTMP;
            }

            e.Handled = true;

            var pointTransform = ((ListViewItem)sender).TransformToVisual(Window.Current.Content);
            double w = e.GetPosition((ListViewItem)sender).X;
            double h = e.GetPosition((ListViewItem)sender).Y;

            if (e.HoldingState == HoldingState.Started)
            {
                menu.Commands.Clear();

                menu.Commands.Add(new UICommand
                {
                    Id = vkt,
                    Label = vkt.name,
                    Invoked = OpenEditPanelByMenu
                });


                #region Alarm
                if (vkt.blockedAlarm || vkt.isSkipped(Data.Obavijest.Alarm))
                {
                    menu.Commands.Add(new UICommand
                    {
                        Id = vkt,
                        Label = "Uključi alarm",
                        Invoked = AlarmOn
                    });
                }
                else
                {
                    menu.Commands.Add(new UICommand
                    {
                        Id = vkt,
                        Label = "Preskoči alarm",
                        Invoked = AlarmOff
                    });
                }

                #endregion

                #region Toast
                if (vkt.blockedToast || vkt.isSkipped(Data.Obavijest.Toast))
                {
                    menu.Commands.Add(new UICommand
                    {
                        Id = vkt,
                        Label = "Uključi notifikaciju",
                        Invoked = ToastOn,
                    });
                }
                else
                {
                    menu.Commands.Add(new UICommand
                    {
                        Id = vkt,
                        Label = "Preskoči notifikaciju",
                        Invoked = ToastOff,
                    });
                }
                #endregion

                Point screenCoords = new Point();
                screenCoords = pointTransform.TransformPoint(new Point(w, h));

                await menu.ShowForSelectionAsync(new Rect { X = screenCoords.X, Y = screenCoords.Y });
            }
        }
        private void Vakat_ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListViewItem gr = sender as ListViewItem;
            Vakat vkt = (Vakat)gr.Tag;
            if (DateTime.Now < vkt.time)
            {
                Open_Edit_Panel_Flyout(vkt);
            }
            else
            {
                Vakat vktTMP = new Vakat { rbr = vkt.rbr, name = vkt.name, time = Data.VakatTime(vkt.time.AddDays(1), vkt.rbr) };
                Open_Edit_Panel_Flyout(vktTMP);
            }
        }
        #endregion

        #region VAKAT - POPUP MENU COMMANDS
        private void AlarmOff(IUICommand command)
        {
            var vakat = (Vakat)command.Id;
            Notification.Obrisi_Obavijest(vakat.NotificationKey(Data.Obavijest.Alarm));
            vakat.skipNotification(Data.Obavijest.Alarm);
            Postavi_Stranicu();
        }
        private void AlarmOn(IUICommand command)
        {
            var vakat = (Vakat)command.Id;

            if (vakat.blockedAlarm)
            {
                Open_Edit_Panel_Flyout(vakat);
            }
            else
            {
                vakat.stayNotification(Data.Obavijest.Alarm);
                Postavi_Stranicu();
            }
        }
        private void ToastOff(IUICommand command)
        {
            var vakat = (Vakat)command.Id;
            Notification.Obrisi_Obavijest(vakat.NotificationKey(Data.Obavijest.Toast));
            vakat.skipNotification(Data.Obavijest.Toast);
            Postavi_Stranicu();
        }
        private void ToastOn(IUICommand command)
        {
            var vakat = (Vakat)command.Id;

            if (vakat.blockedToast)
            {
                Open_Edit_Panel_Flyout(vakat);
            }
            else
            {
                vakat.stayNotification(Data.Obavijest.Toast);
                Postavi_Stranicu();
            }
        }
        private void OpenEditPanelByMenu(IUICommand command)
        {
            var vakat = (Vakat)command.Id;

            Open_Edit_Panel_Flyout(vakat);
        }
        #endregion

        #region VAKAT - EDIT PANEL
        private void Open_Edit_Panel_Flyout(Vakat vakat)
        {
            Grid mainGrid = new Grid { MaxWidth = 360, };
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            // Setup Content
            var panel = new StackPanel { Padding = new Thickness(12) };
            panel.SetValue(Grid.RowProperty, 0);

            panel.Children.Add(Get_Edit_Panel_Title(vakat));

            panel.Children.Add(Get_Edit_Panel_Alarm_Slider(vakat));

            panel.Children.Add(Get_Edit_Panel_Toast_Slider(vakat));

            mainGrid.Children.Add(panel);

            Flyout mainFlyout = new Flyout { Content = mainGrid, Placement = Windows.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Full };

            mainGrid.Children.Add(Get_Edit_Panel_Buttons(vakat, mainFlyout));


            mainFlyout.ShowAt(this);
        }

        private TextBlock Get_Edit_Panel_Title(Vakat vakat)
        {
            TextBlock title = new TextBlock
            {
                Text = vakat.name,
                FontSize = 22,
                FontWeight = FontWeights.Bold,
            };
            return title;
        }

        private StackPanel Get_Edit_Panel_Alarm_Slider(Vakat vakat)
        {
            alarmonoff = vakat.blockedAlarm;
            alarmslidervalue = vakat.predAlarm;

            StackPanel sp = new StackPanel();

            TextBlock Alarm_TextBlock = new TextBlock { FontSize = 18 };

            Alarm_TextBlock.Text = vakat.predAlarm_String + " prije nastupa"; //time before prayer-time

            Slider Alarm_Slider = new Slider
            {
                Minimum = 1,
                Maximum = 60,
                Value = vakat.predAlarm,
            };
            Alarm_Slider.ValueChanged += (s, e) =>
            {
                alarmslidervalue = (int)((Slider)s).Value;
                Alarm_TextBlock.Text = ((int)((Slider)s).Value / 60).ToString("00") + ":" + ((int)((Slider)s).Value % 60).ToString("00") + " prije nastupa"; //time before prayer-time
            };

            ToggleSwitch Alarm_ToggleSwitch = new ToggleSwitch
            {
                OffContent = "Isključeno",
                OnContent = "Uključeno",
            };
            Alarm_ToggleSwitch.Tag = Alarm_Slider;

            Alarm_ToggleSwitch.IsOn = !vakat.blockedAlarm;
            Alarm_Slider.IsEnabled = !vakat.blockedAlarm;

            TextBlock Alarm_ToggleSwitch_Header = new TextBlock
            {
                FontSize = 18,
                Text = "Alarm",
            };
            Alarm_ToggleSwitch.SetValue(ToggleSwitch.HeaderProperty, Alarm_ToggleSwitch_Header);

            Alarm_ToggleSwitch.Toggled += (s, e) =>
            {
                alarmonoff = !Alarm_ToggleSwitch.IsOn;
                Alarm_Slider.IsEnabled = Alarm_ToggleSwitch.IsOn;
            };

            sp.Children.Add(Alarm_ToggleSwitch);
            sp.Children.Add(Alarm_TextBlock);
            sp.Children.Add(Alarm_Slider);

            return sp;
        }

        private StackPanel Get_Edit_Panel_Toast_Slider(Vakat vakat)
        {
            toastonoff = vakat.blockedToast;
            toastslidervalue = vakat.predToast;

            StackPanel sp = new StackPanel();

            TextBlock Toast_TextBlock = new TextBlock { FontSize = 18 };

            Toast_TextBlock.Text = vakat.predToast_String + " prije nastupa"; //time before prayer-time

            Slider Toast_Slider = new Slider
            {
                Minimum = 1,
                Maximum = 60,
                Value = vakat.predToast,
            };
            Toast_Slider.ValueChanged += (s, e) =>
            {
                toastslidervalue = (int)((Slider)s).Value;
                Toast_TextBlock.Text = ((int)((Slider)s).Value / 60).ToString("00") + ":" + ((int)((Slider)s).Value % 60).ToString("00") + " prije nastupa"; //time before prayer-time
            };

            ToggleSwitch Toast_ToggleSwitch = new ToggleSwitch
            {
                OffContent = "Isključeno",
                OnContent = "Uključeno",
            };
            Toast_ToggleSwitch.Tag = Toast_Slider;
            Toast_ToggleSwitch.IsOn = !vakat.blockedToast;
            Toast_Slider.IsEnabled = !vakat.blockedToast;

            TextBlock Toast_ToggleSwitch_Header = new TextBlock
            {
                FontSize = 18,
                Text = "Notifikacija",
            };
            Toast_ToggleSwitch.SetValue(ToggleSwitch.HeaderProperty, Toast_ToggleSwitch_Header);
            Toast_ToggleSwitch.Toggled += (s, e) =>
            {
                toastonoff = !Toast_ToggleSwitch.IsOn;
                Toast_Slider.IsEnabled = Toast_ToggleSwitch.IsOn;
            };
            sp.Children.Add(Toast_ToggleSwitch);
            sp.Children.Add(Toast_TextBlock);
            sp.Children.Add(Toast_Slider);

            return sp;
        }

        private Grid Get_Edit_Panel_Buttons(Vakat vakat, Flyout mainFlyout)
        {
            Grid keyGrid = new Grid { };
            keyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            keyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            keyGrid.SetValue(Grid.RowProperty, 1);

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
                mainFlyout.Hide();

                if(!alarmonoff)
                    vakat.predAlarm = alarmslidervalue;
                if (!toastonoff)
                    vakat.predToast = toastslidervalue;

                vakat.blockedAlarm = alarmonoff;
                vakat.blockedToast = toastonoff;

                Notification.Set(Data.Obavijest.All);

                Postavi_Stranicu();
            };
            lBtn.SetValue(Grid.ColumnProperty, 0);

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

            keyGrid.Children.Add(lBtn);
            keyGrid.Children.Add(rBtn);

            return keyGrid;
        }
        #endregion

        #region CALENDAR
        private async void Show_Calendar(DateTime date)
        {
            Calendar_Btn.Icon = new SymbolIcon(Symbol.CalendarReply);
            Calendar_Btn_InPane.Icon = new SymbolIcon(Symbol.CalendarReply);

            if (calendarCT.TranslateY != 0)
            {
                calendar_Grid.Children.Clear();

                calendar_Grid.Children.Add(await Get_Calendar_Grid(date));

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
            }
            else
            {
                Duration duration = new Duration(TimeSpan.FromMilliseconds(250));

                DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();
                myDoubleAnimation1.Duration = duration;
                myDoubleAnimation1.To = 0;

                DoubleAnimation myDoubleAnimation0 = new DoubleAnimation();
                myDoubleAnimation0.Duration = duration;
                myDoubleAnimation0.To = 1000;


                Storyboard sb1 = new Storyboard();
                sb1.Duration = duration;

                Storyboard sb0 = new Storyboard();
                sb0.Duration = duration;


                sb0.Children.Add(myDoubleAnimation0);
                sb1.Children.Add(myDoubleAnimation1);

                Storyboard.SetTarget(myDoubleAnimation0, calendarCT);
                Storyboard.SetTarget(myDoubleAnimation1, calendarCT);

                Storyboard.SetTargetProperty(myDoubleAnimation0, "TranslateY");
                Storyboard.SetTargetProperty(myDoubleAnimation1, "TranslateY");

                sb0.Completed += async (s, e) =>
                {
                    calendar_Grid.Children.Clear();

                    calendar_Grid.Children.Add(await Get_Calendar_Grid(date));

                    sb1.Begin();
                };

                sb0.Begin();
            }
        }

        private void Hide_Calendar()
        {
            Calendar_Btn.Icon = new SymbolIcon(Symbol.Calendar);
            Calendar_Btn_InPane.Icon = new SymbolIcon(Symbol.Calendar);

            Duration duration = new Duration(TimeSpan.FromMilliseconds(250));

            DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();

            myDoubleAnimation1.Duration = duration;

            Storyboard sb = new Storyboard();
            sb.Duration = duration;

            sb.Children.Add(myDoubleAnimation1);

            Storyboard.SetTarget(myDoubleAnimation1, calendarCT);

            Storyboard.SetTargetProperty(myDoubleAnimation1, "TranslateY");

            myDoubleAnimation1.To = 1000;

            sb.Begin();
        }

        private async Task<Grid> Get_Calendar_Grid(DateTime date)
        {
            Grid mainGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MaxWidth = 720,
                Background = new SolidColorBrush(Color.FromArgb(255, 31, 31, 31))
            };

            if (!Fixed.IsDarkTheme) mainGrid.Background = new SolidColorBrush(Color.FromArgb(255, 244, 244, 244));

            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            mainGrid.RowDefinitions.Add(new RowDefinition());

            mainGrid.Children.Add(Get_Calendar_Background_ListViewItem()); // 1st row

            mainGrid.Children.Add(Get_Calendar_Header_With_DatePicker(date)); // 2nd row

            mainGrid.Children.Add(await Get_Calendar_ContentPanel_Grid(date)); // 3rd row

            return mainGrid;
        }

        private ListViewItem Get_Calendar_Background_ListViewItem()
        {
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

            hideLVI.Content = Get_Calendar_Hide_Arrow();

            hideLVI.Tapped += (s, e) =>
            {
                Hide_Calendar();
            };

            return hideLVI;
        }

        private TextBlock Get_Calendar_Hide_Arrow()
        {
            TextBlock hideTB = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 18,
                FontFamily = new FontFamily("Segoe UI Symbol"),
                Text = "",
                VerticalAlignment = VerticalAlignment.Center,
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

            return hideTB;
        }

        private StackPanel Get_Calendar_Header_With_DatePicker(DateTime date)
        {
            var panel = new StackPanel
            {
                Padding = new Thickness(12),
            };

            panel.SetValue(Grid.RowProperty, 1);

            StackPanel headerSp = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
            DatePicker dp = new DatePicker
            {
                Margin = new Thickness(0, 12, 0, 0),
                Header = "Vaktija za " + Data.data.locationsDative[Memory.location],
                Date = date,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            headerSp.Children.Add(dp);

            TextBlock hijriTB = new TextBlock
            {
                Margin = new Thickness(12),
                FontSize = 20,
                Text = Data.Hijri_Date_To_String(date),
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            headerSp.Children.Add(hijriTB);

            panel.Children.Add(headerSp);

            #region Events
            dp.DateChanged += (s, args) =>
            {
                Show_Calendar(dp.Date.DateTime);
            };
            #endregion


            return panel;
        }

        private async Task<Grid> Get_Calendar_ContentPanel_Grid(DateTime date)
        {
            Grid content_Grid = new Grid();
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.RowDefinitions.Add(new RowDefinition());
            content_Grid.SetValue(Grid.RowProperty, 2);

            for (int i = 0; i < Data.data.vakatNames.Count; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50));

                content_Grid.Children.Add(Get_Calendar_Vakat_ListViewItem(new Vakat { rbr = i, name = Data.data.vakatNames[i], time = Data.VakatTime(date, i) }));
            }

            return content_Grid;
        }

        private ListViewItem Get_Calendar_Vakat_ListViewItem(Vakat vakat)
        {
            if (vakat.time.DayOfWeek == DayOfWeek.Friday && vakat.rbr == 2)
                vakat.name = "Podne (Džuma)";

            ListViewItem Article_Item = new ListViewItem
            {
                Tag = vakat,
                Padding = new Thickness(18, 6, 18, 6),
                Margin = new Thickness(0),
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(0, 1, 0, 0),
            };

            Article_Item.SetValue(Grid.RowProperty, vakat.rbr);

            Article_Item.Content = Get_Calendar_Vakat_Grid(vakat);

            return Article_Item;
        }

        private Grid Get_Calendar_Vakat_Grid(Vakat vakat)
        {
            Grid _Grid = new Grid
            {
                VerticalAlignment = VerticalAlignment.Center,
            };

            ColumnDefinition cd0 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), };
            ColumnDefinition cd1 = new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star), };
            ColumnDefinition cd2 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto), };
            _Grid.ColumnDefinitions.Add(cd0);
            _Grid.ColumnDefinitions.Add(cd1);
            _Grid.ColumnDefinitions.Add(cd2);

            _Grid.Children.Add(Get_Calendar_Vakat_Time(vakat));

            _Grid.Children.Add(Get_Calendar_Vakat_Name(vakat));

            return _Grid;
        }

        private TextBlock Get_Calendar_Vakat_Time(Vakat vakat)
        {
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

            return Clock_TextBlock;
        }

        private TextBlock Get_Calendar_Vakat_Name(Vakat vakat)
        {
            TextBlock Name_TextBLock = new TextBlock
            {
                Text = vakat.name,
                FontSize = 22,
                Margin = new Thickness(3),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Name_TextBLock.SetValue(Grid.ColumnProperty, 1);

            return Name_TextBLock;
        }
        #endregion

        #region BUTTON-CLICKS EVENTS
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

        private void Calendar_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (calendarCT.TranslateY == 0)
                Hide_Calendar();
            else
                Show_Calendar(DateTime.Now);
        }
        private async void Compass_Btn_Click(object sender, RoutedEventArgs e)
        {
            loader.IsActive = true;

            await Task.Delay(50);

            var cmp = Windows.Devices.Sensors.Compass.GetDefault();
            if (cmp == null)
            {
                loader.IsActive = false;

                ContentDialog cd = new ContentDialog { Title = "Vaš uređaj ne posjeduje kompas!", PrimaryButtonText = "ok", };
                await cd.ShowAsync();
            }
            else
            {
                loader.IsActive = false;

                Frame.Navigate(typeof(QiblaCompass));
            }
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
            request.Data.Properties.Title = "Vaktija za " + Data.data.locationsDative[Memory.location];
            string content_string = DateTime.Now.Day + ". " + Data.data.monthnames[DateTime.Now.Month % 12] + " " + DateTime.Now.Year + ".";

            if (Memory.Hijri_Date_In_App)
                content_string += Environment.NewLine + Data.Hijri_Date_To_String(DateTime.Now);

            for (int i = 0; i < Data.data.vakatNames.Count; i++)
                content_string += Environment.NewLine + Data.VakatTime(DateTime.Now, i).ToString("HH:mm") + " " + Data.data.vakatNames[i];

            content_string += Environment.NewLine + "www.vaktija.ba";
            request.Data.SetText(content_string);
        }
        private async void Rate_Btn_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName));
        }
        #endregion
    }

    class HomePageShow
    {
        public string datum
        {
            get
            {
                string date_str = "";
                if (Memory.Date_Show_In_App)
                {
                    date_str = Data.data.weekdays[(int)DateTime.Now.DayOfWeek % 7] + ", " + DateTime.Now.Day + ". " + Data.data.monthnames[DateTime.Now.Month % 12] + " " + DateTime.Now.Year + ".";

                    if (Memory.Hijri_Date_In_App)
                        date_str += " / " + Data.Hijri_Date_To_String(DateTime.Now);
                }
                return date_str.ToLower();
            }
        }
        public Visibility datumVisibility
        {
            get
            {
                if (Memory.Date_Show_In_App)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
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
                return Data.data.locations[Memory.location].ToLower();
            }
        }
    }
}