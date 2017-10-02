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
using System.Windows.Input;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace LibraryRoomReservationSystem
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Main : Page
    {
        private SystemClient myClient;

        public Main()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            myClient = (SystemClient)e.Parameter;
            GetStatus();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetStatus();
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            StopUsing();
        }

        private void TimeExtend_Click(object sender, RoutedEventArgs e)
        {
            TimeExtend();
        }

        private void CheckIn_Click(object sender, RoutedEventArgs e)
        {
            CheckIn();
        }

        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            Leave();
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            View(myClient.reservationID);
        }

        private async void StopUsing()
        {
            try
            {
                HttpResponseMessage response = await myClient.httpClient.GetAsync(new Uri("https://" + myClient.serverAddr + "/rest/v2/stop?token=" + myClient.token));
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(response.Content.ToString());
                    JsonObject JSResponse = JsonObject.Parse(response.Content.ToString());
                    string returnStatus = JSResponse.GetNamedString("status");
                    Debug.WriteLine(returnStatus);
                    if (returnStatus.Equals("success"))
                    {
                        // Stop using success
                        string returnMessage = JSResponse.GetNamedString("message");
                        myClient.ShowMessage("停止使用成功", returnMessage);
                    }
                    else
                    {
                        // Fail to stop using
                        string returnMessage = JSResponse.GetNamedString("message");
                        myClient.ShowMessage("停止使用失败", returnMessage);
                    }
                    GetStatus();
                }
                else
                    myClient.ShowMessage("结束使用", "网络错误！");
            }
            catch (Exception e)
            {
                //throw;
                myClient.ShowMessage("结束使用", "网络错误！");
            }
        }

        private async void CheckIn()
        {
            try
            {
                HttpResponseMessage response = await myClient.httpClient.GetAsync(new Uri("https://" + myClient.serverAddr + "/rest/v2/checkIn?token=" + myClient.token));
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(response.Content.ToString());
                    JsonObject JSResponse = JsonObject.Parse(response.Content.ToString());
                    string returnStatus = JSResponse.GetNamedString("status");
                    Debug.WriteLine(returnStatus);
                    if (returnStatus.Equals("success"))
                    {
                        // Check in success
                        string returnMessage = JSResponse.GetNamedString("message");
                        myClient.ShowMessage("签到成功", returnMessage);
                    }
                    else
                    {
                        // Fail to check in
                        string returnMessage = JSResponse.GetNamedString("message");
                        myClient.ShowMessage("签到失败", returnMessage);
                    }
                    GetStatus();
                }
                else
                    myClient.ShowMessage("签到", "网络错误！");
            }
            catch (Exception e)
            {
                //throw;
                myClient.ShowMessage("签到", "网络错误！");
            }
        }

        private async void Leave()
        {
            try
            {
                HttpResponseMessage response = await myClient.httpClient.GetAsync(new Uri("https://" + myClient.serverAddr + "/rest/v2/leave?token=" + myClient.token));
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(response.Content.ToString());
                    JsonObject JSResponse = JsonObject.Parse(response.Content.ToString());
                    string returnStatus = JSResponse.GetNamedString("status");
                    Debug.WriteLine(returnStatus);
                    if (returnStatus.Equals("success"))
                    {
                        // Check in success
                        string returnMessage = JSResponse.GetNamedString("message");
                        myClient.ShowMessage("暂离成功", returnMessage);
                    }
                    else
                    {
                        // Fail to check in
                        string returnMessage = JSResponse.GetNamedString("message");
                        myClient.ShowMessage("暂离失败", returnMessage);
                    }
                    GetStatus();
                }
                else
                    myClient.ShowMessage("暂离", "网络错误！");
            }
            catch (Exception e)
            {
                //throw;
                myClient.ShowMessage("暂离", "网络错误！");
            }
        }

        private async void TimeExtend()
        { 
            if(!myClient.reservationID.Equals(-1.0))
            {
                try
                {
                    HttpResponseMessage response = await myClient.httpClient.GetAsync(new Uri("https://" + myClient.serverAddr + "/rest/v2/timeExtend/" + myClient.reservationID.ToString() + "?token=" + myClient.token));
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine(response.Content.ToString());
                        JsonObject JSResponse = JsonObject.Parse(response.Content.ToString());
                        string returnStatus = JSResponse.GetNamedString("status");
                        Debug.WriteLine(returnStatus);
                        if (returnStatus.Equals("success"))
                        {
                            // Time extend success
                            string returnMessage = JSResponse.GetNamedString("message");
                            myClient.ShowMessage("续座成功", returnMessage);
                        }
                        else
                        {
                            // Fail to extend time
                            string returnMessage = JSResponse.GetNamedString("message");
                            myClient.ShowMessage("续座失败", returnMessage);
                        }
                        GetStatus();
                    }
                    else
                        myClient.ShowMessage("续座", "网络错误！");
                }
                catch (Exception e)
                {
                    //throw;
                    myClient.ShowMessage("续座", "网络错误！");
                }
            }
            else
            {
                myClient.ShowMessage("续座", "当前没有正在使用的预约！");
            }
        }

        //private async void CheckStatus()
        //{
        //    try
        //    {
        //        HttpResponseMessage response = await myClient.httpClient.GetAsync(new Uri("http://seat." + myClient.serverAddr + ".edu.cn/rest/v2/user/reservations?token=" + myClient.token));
        //        if (response.IsSuccessStatusCode)
        //        {
        //            Debug.WriteLine(response.Content.ToString());
        //            JsonObject JSResponse = JsonObject.Parse(response.Content.ToString());
        //            JsonValue JSData = JSResponse.GetNamedValue("data");

        //            if (JSData.ValueType.Equals(JsonValueType.Null))
        //            {
        //                myClient.reservationID = 0;
        //                bCheckIn.IsEnabled = false;
        //                bTimeExtend.IsEnabled = false;
        //                bStopUsing.IsEnabled = false;
        //            }
        //            else
        //            {
        //                JsonObject tmp = JSData.GetArray().GetObjectAt(0);
        //                myClient.reservationID = Convert.ToInt32(tmp.GetNamedNumber("id"));
        //                //myClient.reservationID = JSData.GetString();
        //                Debug.WriteLine(myClient.reservationID.ToString());
        //                bCheckIn.IsEnabled = true;
        //                bTimeExtend.IsEnabled = true;
        //                bStopUsing.IsEnabled = true;
        //            }
        //        }
        //        else
        //            myClient.ShowMessage("获取预约状态", "网络错误！");
        //    }
        //    catch (Exception e)
        //    {
        //        //throw;
        //        myClient.ShowMessage("获取预约状态", "网络错误！");
        //    }
        //}

        private async void View(double id)
        {
            if (!myClient.reservationID.Equals(0))
            {
                try
                {
                    HttpResponseMessage response = await myClient.httpClient.GetAsync(new Uri("https://" + myClient.serverAddr + "/rest/view/" + id.ToString() + "?token=" + myClient.token));
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine(response.Content.ToString());
                        JsonObject JSResponse = JsonObject.Parse(response.Content.ToString());
                        ContentDialog dialog = new ContentDialog()
                        {
                            Title = "查看当前预约",
                            Content = "凭证号：" + JSResponse.GetNamedString("receipt") + "\n日期：" + JSResponse.GetNamedString("onDate") + "\n时间：" + JSResponse.GetNamedString("begin") + " - " + JSResponse.GetNamedString("end") + "\n地点：" + JSResponse.GetNamedString("location"),
                            PrimaryButtonText = "确认",
                        };
                        if (myClient.status.Equals("RESERVE"))
                        {
                            dialog.SecondaryButtonText = "取消预约";
                        }
                        ContentDialogResult cancel = await dialog.ShowAsync();
                        if (cancel == ContentDialogResult.Secondary)
                        {
                            try
                            {
                                response = await myClient.httpClient.GetAsync(new Uri("https://" + myClient.serverAddr + "/rest/v2/cancel/" + myClient.reservationID + "?token=" + myClient.token));
                                if (response.IsSuccessStatusCode)
                                {
                                    Debug.WriteLine(response.Content.ToString());
                                    JSResponse = JsonObject.Parse(response.Content.ToString());
                                    string returnStatus = JSResponse.GetNamedString("status");
                                    Debug.WriteLine(returnStatus);
                                    if (returnStatus.Equals("success"))
                                    {
                                        // Cancel success
                                        string returnMessage = JSResponse.GetNamedString("message");
                                        myClient.ShowMessage("取消预约成功", returnMessage);
                                    }
                                    else
                                    {
                                        // Fail to cancel
                                        string returnMessage = JSResponse.GetNamedString("message");
                                        myClient.ShowMessage("取消预约失败", returnMessage);
                                    }
                                    GetStatus();
                                }
                                else
                                    myClient.ShowMessage("取消预约", "网络错误！");
                            }
                            catch (Exception e)
                            {
                                //throw;
                                myClient.ShowMessage("取消预约", "网络错误！");
                            }
                        }
                    }
                    else
                        myClient.ShowMessage("查看当前预约", "网络错误！");
                }
                catch (Exception e)
                {
                    //throw;
                    myClient.ShowMessage("查看当前预约", "网络错误！");
                }
            }
            else
            {
                myClient.ShowMessage("查看当前预约", "当前没有正在使用的预约！");
            }
        }       

        private async void GetStatus()
        {
            try
            {
                HttpResponseMessage response = await myClient.httpClient.GetAsync(new Uri("https://" + myClient.serverAddr + "/rest/v2/user/reservations?token=" + myClient.token));
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(response.Content.ToString());
                    JsonObject JSResponse = JsonObject.Parse(response.Content.ToString());
                    if (JSResponse.GetNamedString("status").Equals("success"))
                    {
                        JsonValue JSData = JSResponse.GetNamedValue("data");

                        if (JSData.ValueType.Equals(JsonValueType.Null))
                        {
                            myClient.reservationID = 0;
                            myClient.status = "none";
                            bCheckIn.IsEnabled = false;
                            bTimeExtend.IsEnabled = false;
                            bStopUsing.IsEnabled = false;
                            bLeave.IsEnabled = false;
                        }
                        else
                        {
                            JsonObject tmp = JSData.GetArray().GetObjectAt(0);
                            myClient.reservationID = Convert.ToInt32(tmp.GetNamedNumber("id"));
                            myClient.status = tmp.GetNamedString("status");
                        }
                        switch (myClient.status)
                        {
                            case "none":
                                break;
                            case "RESERVE":
                                bCheckIn.Content = "签到";
                                bCheckIn.IsEnabled = true;
                                bLeave.IsEnabled = false;
                                bTimeExtend.IsEnabled = false;
                                bStopUsing.IsEnabled = false;
                                break;
                            case "CHECK_IN":
                                bCheckIn.Content = "签到";
                                bCheckIn.IsEnabled = false;
                                bLeave.IsEnabled = true;
                                bTimeExtend.IsEnabled = true;
                                bStopUsing.IsEnabled = true;
                                break;
                            case "AWAY":
                                bCheckIn.Content = "返回";
                                bCheckIn.IsEnabled = true;
                                bLeave.IsEnabled = false;
                                bTimeExtend.IsEnabled = true;
                                bStopUsing.IsEnabled = true;
                                break;
                            default:
                                myClient.ShowMessage("状态载入错误", "没有预料到您现在的状态呢，欢迎反馈到邮箱里，现在已经给您开放所有按钮了！");
                                bCheckIn.Content = "签到 & 返回";
                                bCheckIn.IsEnabled = true;
                                bLeave.IsEnabled = true;
                                bTimeExtend.IsEnabled = true;
                                bStopUsing.IsEnabled = true;
                                break;
                        }
                    }
                    else
                    {
                        myClient.ShowMessage("获取状态失败", JSResponse.GetNamedString("message"));
                    }
                }
                else
                    myClient.ShowMessage("获取状态", "网络错误！");
            }
            catch (Exception e)
            {
                //throw;
                myClient.ShowMessage("获取状态", "网络错误！");
            }
        }
    }
}
