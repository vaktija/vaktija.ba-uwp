using System;
using Vaktija.ba.Helpers;
using Vaktija.ba.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Vaktija.ba
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            ///Set a theme
            ///
            try
            {
                if (Memory.Theme == 1)
                    Application.Current.RequestedTheme = ApplicationTheme.Light;
                else if (Memory.Theme == 2)
                    Application.Current.RequestedTheme = ApplicationTheme.Dark;
            }
            catch
            {
                Application.Current.RequestedTheme = ApplicationTheme.Light;
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Set.System_Tray();

            RegisterBackgroundTask_TimeTrigger();
            RegisterBackgroundTask_ToastNotificationHistoryChangedTrigger();

            try
            {
                Data.data = Set.JsonToArray<Data>(await Get.Read_Data_To_String())[0];
            }
            catch
            {
                ContentDialog cd = new ContentDialog { Title = "Dogodila se greška pri učitavanju podataka. Pokušajte ponovo!", PrimaryButtonText = "ok", };
                await cd.ShowAsync();
            }

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            try
            {
                if (Memory.Live_Tile) /* Update / reset live tile */
                    LiveTile.Update();
                else
                    LiveTile.Reset();
            }
            finally
            {
                deferral.Complete();
            }
        }

        private void RegisterBackgroundTask_ToastNotificationHistoryChangedTrigger()
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == "ToastNotificationHistoryChangedTrigger")
                {
                    cur.Value.Unregister(true);
                }
            }

            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            builder.Name = "ToastNotificationHistoryChangedTrigger";
            builder.TaskEntryPoint = "BackgroundTask.ToastNotificationHistoryChangedTrigger";
            builder.SetTrigger(new ToastNotificationHistoryChangedTrigger());
            BackgroundTaskRegistration registration = builder.Register();
        }
        private void RegisterBackgroundTask_TimeTrigger()
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == "ByTimer")
                {
                    cur.Value.Unregister(true);
                }
            }

            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            builder.Name = "ByTimer";
            builder.TaskEntryPoint = "BackgroundTask.ByTimer";
            builder.SetTrigger(new TimeTrigger(30, false));
            BackgroundTaskRegistration registration = builder.Register();
        }
    }
}
