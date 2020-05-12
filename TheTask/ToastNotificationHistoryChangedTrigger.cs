using BackgroundTask.Helpers;
using Windows.ApplicationModel.Background;

namespace BackgroundTask
{
    public sealed class ToastNotificationHistoryChangedTrigger : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var def = taskInstance.GetDeferral();
            try
            {
                Data.data = Data.JsonToArray<Data>(await Data.Read_Data_To_String(Fixed.App_Data_File))[0];

                LiveTile.Update();

                Notification.Set(Data.Obavijest.All);
            }
            finally
            {
                def.Complete();
            }
        }
    }
}