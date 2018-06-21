using BackgroundTask.Helpers;
using BackgroundTask.Views;
using Windows.ApplicationModel.Background;

namespace BackgroundTask
{
    public sealed class ByTimer : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var def = taskInstance.GetDeferral();
            try
            {
                Data.data = Set.JsonToArray<Data>(await Get.Read_Data_To_String())[0];

                Year.Set();

                if (Memory.Live_Tile)
                    LiveTile.Update();
                else
                    LiveTile.Reset();

                Set.Group_Notifications(2, 0, false);
            }
            finally
            {
                def.Complete();
            }
        }
    }
}
