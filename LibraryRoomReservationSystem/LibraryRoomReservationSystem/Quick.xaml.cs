using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace LibraryRoomReservationSystem
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Quick : Page
    {
        private SystemClient myClient;

        public Quick()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            myClient = (SystemClient)e.Parameter;
            LoadBuilding();
        }

        private void LoadBuilding()
        {
            string[] buildingDisp = {"信息部图书馆"};
            selectBuilding.ItemsSource = buildingDisp;
            selectBuilding.SelectedIndex = 0;
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            Test();
        }

        private async void Test()
        {
            try
            {
                Dictionary<string, string> test = new Dictionary<string, string>();
                test.Add("building", "1");
                test.Add("hour", "1");
                test.Add("token", myClient.token);

                HttpResponseMessage response = await myClient.httpClient.PostAsync(new Uri("http://seat." + myClient.serverAddr + ".edu.cn/rest/v2/quickBook"), new HttpFormUrlEncodedContent(test));
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(response.Content.ToString());
                    JsonObject JSResponse = JsonObject.Parse(response.Content.ToString());
                    string returnStatus = JSResponse.GetNamedString("status");
                    Debug.WriteLine(returnStatus);
                    if (returnStatus.Equals("success"))
                    {
                        // Check in success
                        JsonObject JSData = JSResponse.GetNamedObject("data");
                        JsonObject tmp = JSData.GetNamedObject("reservation");
                        //myClient.reservationID = Convert.ToInt32(tmp.GetNamedNumber("id"));
                        //myClient.status = tmp.GetNamedString("status");
                        myClient.ShowMessage("预约成功", "凭证号：" + tmp.GetNamedString("receipt") + "\n日期：" + tmp.GetNamedString("onDate") + "\n时间：" + tmp.GetNamedString("begin") + " - " + tmp.GetNamedString("end") + "\n地点：" + tmp.GetNamedString("location"));
                    }
                    else
                    {
                        // Fail to check in
                        string returnMessage = JSResponse.GetNamedString("message");
                        myClient.ShowMessage("预约失败", returnMessage);
                    }
                }
                else
                    myClient.ShowMessage("快速预约", "网络错误！");
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
