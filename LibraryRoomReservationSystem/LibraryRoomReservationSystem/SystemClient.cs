using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using System.Windows.Input;
using Windows.Web.Http.Filters;
using System.Diagnostics;

namespace LibraryRoomReservationSystem
{
    class SystemClient
    {
        public SystemClient()
        {
            httpFilter = new HttpBaseProtocolFilter();
            httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
            httpFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
            httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.InvalidName);
            httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Untrusted);
            httpClient = new HttpClient(httpFilter);
            serverAddr = "seat.lib.whu.edu.cn:8443";
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

            try
            {
                await dialog.ShowAsync();

            }
            catch (Exception e)
            {
                //
            }
        }
    }
}
