using System;
using Vaktija.ba.Views;
using Windows.Data.Xml.Dom;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Vaktija.ba.Helpers
{
    class Set
    {
        public static async void System_Tray(string message = "")
        {
            if (Fixed.IsPhone)
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                if (Fixed.IsDarkTheme)
                {
                    statusBar.BackgroundColor = Color.FromArgb(255, 31, 31, 31);
                    statusBar.ForegroundColor = Colors.White;
                }
                else
                {
                    statusBar.BackgroundColor = Color.FromArgb(255, 244, 244, 244);
                    statusBar.ForegroundColor = Colors.Black;
                }
                statusBar.BackgroundOpacity = 1;
                statusBar.ProgressIndicator.ProgressValue = 0;
                statusBar.ProgressIndicator.Text = message;
                if(message != "")
                {
                    statusBar.BackgroundColor = Color.FromArgb(255, 157, 157, 0);
                    statusBar.ForegroundColor = Colors.Black;
                }
                await statusBar.ProgressIndicator.ShowAsync();
            }
            else
            {
                var v = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
                if (Fixed.IsDarkTheme)
                {
                    v.TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 31, 31, 31);
                    v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 31, 31, 31);
                    v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.White;
                    v.TitleBar.ForegroundColor = Windows.UI.Colors.White;
                }
                else
                {
                    v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 244, 244, 244);
                    v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.Black;
                    v.TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 244, 244, 244);
                    v.TitleBar.ForegroundColor = Windows.UI.Colors.Black;
                }
                if (message != "")
                {
                    v.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 157, 157, 0);
                    v.TitleBar.ButtonForegroundColor = Windows.UI.Colors.Black;
                    v.TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 157, 157, 0);
                    v.TitleBar.ForegroundColor = Windows.UI.Colors.Black;
                }
                v.Title = message;
            }
        }

        public static void PageHeader(Page mainPage, Grid grid, string title, string subtitle, string toPage, bool portrait)
        {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Add(new RowDefinition { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.Margin = new Windows.UI.Xaml.Thickness(6, 6, 6, 0);

            #region title
            Viewbox titleVB = new Viewbox()
            {
                MaxHeight = 20,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right,
            };
            grid.Children.Add(titleVB);

            TextBlock titleTB = new TextBlock
            {
                Text = title,
                FontFamily = new Windows.UI.Xaml.Media.FontFamily("Segoe UI Light"),
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right,
                Foreground = new SolidColorBrush(Colors.Black),
            };
            if (Fixed.IsDarkTheme) titleTB.Foreground = new SolidColorBrush(Colors.White);
            if (title == "") titleTB.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            titleVB.Child = titleTB;
            #endregion

            #region subtitle
            ListViewItem subLVI = new ListViewItem
            {
                MinHeight = 0,
                MinWidth = 0,
                Padding = new Windows.UI.Xaml.Thickness(0, 0, 0, 0),
                VerticalContentAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch,
                HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                FontSize = 48,
            };
            subLVI.SetValue(Grid.RowProperty, 1);
            if(toPage != "")
            {
                subLVI.Tapped += (s, e) =>
                {
                    if (toPage == "ChooseLocation")
                    {
                        try { mainPage.Frame.Navigate(typeof(Pages.ChooseLocation)); }
                        catch { }
                    }
                };
            }
            grid.Children.Add(subLVI);

            Grid subGrid = new Grid { Background = new SolidColorBrush(Colors.White) };
            if(Fixed.IsDarkTheme) subGrid.Background = new SolidColorBrush(Colors.Black);
            subLVI.Content = subGrid;

            Viewbox subVB = new Viewbox
            {
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
            };

            if (portrait)
            {
                subVB.MaxHeight = 66;
            }
            else
            {
                grid.Margin = new Thickness(12, 6, 6, 0);
            }
            subGrid.Children.Add(subVB);

            TextBlock subTB = new TextBlock
            {
                Text = subtitle,
                FontFamily = new Windows.UI.Xaml.Media.FontFamily("Segoe UI Semilight"),
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                Foreground = new SolidColorBrush(Colors.LightGray),
                OpticalMarginAlignment = OpticalMarginAlignment.TrimSideBearings,
                FontWeight = FontWeights.Light,
            };
            subVB.Child = subTB;
            #endregion
        }

        public static async void Group_Notifications(int broj_dana, int od = 0, bool obnovi = true, int rbrv = 6, int AorT = 0, bool firstDel = false)
        {
            System.Diagnostics.Debug.WriteLine("Učitavanje podataka. Lokacija " + Memory.location.ime);
            System_Tray("Učitavanje podataka...");
            await System.Threading.Tasks.Task.Delay(20);
            if (obnovi)
                Year.Set();

            for (int i = od; i <= broj_dana; i++)
            {
                Day dan = Year.year.months[DateTime.Now.AddDays(i).Month - 1].days[DateTime.Now.AddDays(i).Day - 1];
                Notifications_On_Day(dan, rbrv, AorT, firstDel);
            }

            Last_Notification(Year.year.months[DateTime.Now.AddDays(broj_dana).Month - 1].days[DateTime.Now.AddDays(broj_dana).Day - 1]);
            await System.Threading.Tasks.Task.Delay(20);
            System_Tray();
        }
        public static void Notifications_On_Day(Day day, int rbrv, int AorT, bool firstDel)
        {
            if (rbrv < 6)
            {
                try
                {
                    if (AorT == 0)
                    {
                        if (!Memory.alarmBlocked[rbrv])
                        {
                            if (!day.vakti[rbrv].alarmSkipped)
                                Notification.Create_New_Alarm(day.vakti[rbrv], firstDel);
                            else
                                Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "alarm"));
                        }
                        else
                            Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "alarm"));

                        if (!Memory.toastBlocked[rbrv])
                        {
                            if (!day.vakti[rbrv].toastSkipped)
                                Notification.Create_New_Toast(day.vakti[rbrv], firstDel);
                            else
                                Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "toast"));
                        }
                        else
                            Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "toast"));
                    }
                    else if (AorT == 1)
                    {
                        if (!Memory.alarmBlocked[rbrv])
                        {
                            if (!day.vakti[rbrv].alarmSkipped)
                                Notification.Create_New_Alarm(day.vakti[rbrv], firstDel);
                            else
                                Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "alarm"));
                        }
                        else
                            Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "alarm"));

                    }
                    else if (AorT == 2)
                    {
                        if (!Memory.toastBlocked[rbrv])
                        {
                            if (!day.vakti[rbrv].toastSkipped)
                                Notification.Create_New_Toast(day.vakti[rbrv], firstDel);
                            else
                                Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "toast"));
                        }
                        else
                            Notification.Delete(Notification.Create_Key(day.vakti[rbrv], "toast"));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Greška pri registraciji grupe notifikacija " + "(" + ex.Message + ")");
                }
            }
            else //All
            {
                foreach (var it in day.vakti)
                {
                    try
                    {
                        if (AorT == 0)
                        {
                            if (!Memory.alarmBlocked[it.rbr])
                            {
                                if (!it.alarmSkipped)
                                    Notification.Create_New_Alarm(it, firstDel);
                                else
                                    Notification.Delete(Notification.Create_Key(it, "alarm"));
                            }
                            else
                            {
                                Notification.Delete(Notification.Create_Key(it, "alarm"));
                            }

                            if (!Memory.toastBlocked[it.rbr])
                            {
                                if (!it.toastSkipped)
                                    Notification.Create_New_Toast(it, firstDel);
                                else
                                    Notification.Delete(Notification.Create_Key(it, "toast"));
                            }
                            else
                            {
                                Notification.Delete(Notification.Create_Key(it, "toast"));
                            }
                        }
                        else if (AorT == 1)
                        {
                            if (!Memory.alarmBlocked[it.rbr])
                            {
                                if (!it.alarmSkipped)
                                    Notification.Create_New_Alarm(it, firstDel);
                                else
                                    Notification.Delete(Notification.Create_Key(it, "alarm"));
                            }
                            else
                            {
                                Notification.Delete(Notification.Create_Key(it, "alarm"));
                            }
                        }
                        else if (AorT == 2)
                        {
                            if (!Memory.toastBlocked[it.rbr])
                            {
                                if (!it.toastSkipped)
                                    Notification.Create_New_Toast(it, firstDel);
                                else
                                    Notification.Delete(Notification.Create_Key(it, "toast"));
                            }
                            else
                            {
                                Notification.Delete(Notification.Create_Key(it, "toast"));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Greška pri registraciji grupe notifikacija " + "(" + ex.Message + ")");
                    }
                }
            }
        }

        public static void Last_Notification(Day day)
        {
            try
            {
                Notification.Delete("lastnotif");
                ScheduledToastNotification scheduledToast = new ScheduledToastNotification(Last_Notification_XML(), day.vakti[day.vakti.Count - 1].time.AddMinutes(-5));
                scheduledToast.Id = "lastnotif";
                scheduledToast.SuppressPopup = true;
                ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Greška pri registraciji upozoravajuće notifikacije " + "(" + ex.Message + ")");
            }
        }
        private static XmlDocument Last_Notification_XML()
        {
            string toastXmlString =
                        "<toast launch=\"lastnotif\">\n" +
                            "<visual>\n" +
                                "<binding template=\"ToastGeneric\">\n" +
                                    "<text>O vjernici, molitvu obavljajte i Gospodaru svome se klanjajte, i dobra djela činite da biste postigli ono što želite...</text>\n" +
                                    "<text>Sura 22:77 (El-Hadždž – Hadž, Medina)</text>\n" +
                                "</binding>\n" +
                            "</visual>\n" +
                            "<audio silent=\"true\" />\n" +
                       "</toast>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(toastXmlString);
            return doc;
        }

        public static System.Collections.Generic.List<T> JsonToArray<T>(string jsonString)
        {
            jsonString = jsonString.Replace("č", "\u010d").Replace("ć", "\u0107").Replace("đ", "\u0111").Replace("š", "\u0161").Replace("ž", "\u017e").Replace("Č", "\u010c").Replace("Ć", "\u0106").Replace("Đ", "\u0110").Replace("Š", "\u0160").Replace("Ž", "\u017d");
            if (jsonString.Length < 1) return new System.Collections.Generic.List<T>();
            if (jsonString[0] != '[') jsonString = "[" + jsonString + "]";
            try
            {
                System.Collections.Generic.List<T> trs = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<T>>(jsonString);
                return trs;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("greska: " + ex.Message);
                return new System.Collections.Generic.List<T>();
            }
        }
    }
}