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

        private int maxTime { get; set; }

        private string[] BuildingValues;

        public Quick()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            myClient = (SystemClient)e.Parameter;
            maxTime = 0;
            CheckMaxTime();
            LoadBuilding();
        }

        private async void CheckMaxTime()
        {
            try
            {
                HttpResponseMessage response = await myClient.httpClient.GetAsync(new Uri("https://" + myClient.serverAddr + "/rest/v2/allowedHours?token=" + myClient.token));
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(response.Content.ToString());
                    JsonObject JSResponse = JsonObject.Parse(response.Content.ToString());
                    string returnStatus = JSResponse.GetNamedString("status");
                    Debug.WriteLine(returnStatus);
                    if (returnStatus.Equals("success"))
                    {
                        JsonObject JSData = JSResponse.GetNamedObject("data");
                        maxTime = int.Parse(JSData.GetNamedString("maxFree"));
                        Debug.WriteLine(maxTime.ToString());
                    }
                    else
                    {
                        string returnMessage = JSResponse.GetNamedString("message");
                        myClient.ShowMessage("获取最大可预约时间失败", returnMessage);
                    }
                }
                else
                    myClient.ShowMessage("获取最大可预约时间", "网络错误！");
            }
            catch (Exception e)
            {
                //throw;
                myClient.ShowMessage("获取最大可预约时间", "网络错误！");
            }

            DrawButtons();
        }

        private async void LoadBuilding()
        {
            // 用以填充下拉列表
            string[] BuildingNames;

            // 获取可用场馆
            try
            {
                Dictionary<string, string> tokens = new Dictionary<string, string>();
                tokens.Add("token", myClient.token);

                HttpResponseMessage response = await myClient.httpClient.PostAsync(new Uri("https://" + myClient.serverAddr + "/rest/v2/free/filters"), new HttpFormUrlEncodedContent(tokens));
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(response.Content.ToString());
                    JsonObject JSResponse = JsonObject.Parse(response.Content.ToString());
                    string returnStatus = JSResponse.GetNamedString("status");
                    Debug.WriteLine(returnStatus);
                    if (returnStatus.Equals("success"))
                    {
                        JsonObject JSData = JSResponse.GetNamedObject("data");
                        JsonArray jsABuildings = JSData.GetNamedArray("buildings");
                        BuildingNames = new string[jsABuildings.Count + 1];
                        BuildingValues = new string[jsABuildings.Count + 1];

                        // 开始填充下拉列表数组
                        BuildingNames[0] = "不限场馆选座";
                        BuildingValues[0] = "";
                        for (int i = 0; i < jsABuildings.Count; i++)
                        {
                            BuildingNames[i + 1] = jsABuildings.GetArrayAt((uint)i).GetStringAt((uint)1);
                            BuildingValues[i + 1] = jsABuildings.GetArrayAt((uint)i).GetNumberAt((uint)0).ToString();
                        }

                        // 绑定下拉列表数据
                        selectBuilding.ItemsSource = BuildingNames;
                        selectBuilding.SelectedIndex = 0;
                    }
                    else
                    {
                        string returnMessage = JSResponse.GetNamedString("message");
                        myClient.ShowMessage("获取场馆列表失败", returnMessage);
                    }
                }
                else
                    myClient.ShowMessage("获取场馆列表", "网络错误！");
            }
            catch (Exception e)
            {
                //throw;
                myClient.ShowMessage("获取场馆列表", "网络错误！");
            }
        }

        private void DrawButtons()
        {
            for (int i = 0; i < maxTime; i++)
            {
                Button book = new Button();
                book.Name = (i + 1).ToString();
                book.Content = "预约" + (i + 1).ToString() + "小时";
                book.Click += Book_Click;
                book.Margin = new Thickness(5, 5, 5, 5);
                book.Width = 150;
                book.Height = 30;

                buttonContainer.Children.Add(book);
                book.SetValue(Grid.RowProperty, i / 2);
                book.SetValue(Grid.ColumnProperty, i % 2);
            }
        }

        private async void Book_Click(object sender, RoutedEventArgs e)
        {
            Button book = (Button)sender;
            string time = book.Name;

            try
            {
                Dictionary<string, string> test = new Dictionary<string, string>();
                test.Add("building", BuildingValues[selectBuilding.SelectedIndex]);
                test.Add("hour", time);
                test.Add("token", myClient.token);

                HttpResponseMessage response = await myClient.httpClient.PostAsync(new Uri("https://" + myClient.serverAddr + "/rest/v2/quickBook"), new HttpFormUrlEncodedContent(test));
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(response.Content.ToString());
                    JsonObject JSResponse = JsonObject.Parse(response.Content.ToString());
                    string returnStatus = JSResponse.GetNamedString("status");
                    Debug.WriteLine(returnStatus);
                    if (returnStatus.Equals("success"))
                    {
                        JsonObject JSData = JSResponse.GetNamedObject("data");
                        JsonObject tmp = JSData.GetNamedObject("reservation");

                        myClient.ShowMessage("预约成功", "凭证号：" + tmp.GetNamedString("receipt") + "\n日期：" + tmp.GetNamedString("onDate") + "\n时间：" + tmp.GetNamedString("begin") + " - " + tmp.GetNamedString("end") + "\n地点：" + tmp.GetNamedString("location"));
                    }
                    else
                    {
                        string returnMessage = JSResponse.GetNamedString("message");
                        myClient.ShowMessage("预约失败", returnMessage);
                    }
                }
                else
                    myClient.ShowMessage("快速预约", "网络错误！");
            }
            catch (Exception err)
            {
                //throw;
                myClient.ShowMessage("快速预约", "网络错误！");
            }
        }
    }
}
