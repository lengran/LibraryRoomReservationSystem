using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using System.Windows.Input;
using Windows.Web.Http.Filters;

namespace LibraryRoomReservationSystem
{
    class SystemClient
    {
        public SystemClient()
        {
            httpFilter = new HttpBaseProtocolFilter();
            httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
            httpFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
            httpClient = new HttpClient(httpFilter);
            serverAddr = "lib.whu";
        }

        ~SystemClient()
        {
            httpClient.Dispose();
            httpFilter.Dispose();
        }

        private HttpBaseProtocolFilter httpFilter;

        public HttpClient httpClient;
        public string serverAddr { get; set; }
        public string token { get; set; }
        public string status { get; set; }
        public int reservationID { get; set; }
        public async void ShowMessage(string title, object message)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "确认"
            };

            await dialog.ShowAsync();
        }
    }
}
