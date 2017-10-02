using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.Data.Json;
using System.Diagnostics;
using Windows.System;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace LibraryRoomReservationSystem
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private SystemClient myClient = new SystemClient();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            txtUsername.Text = "";
            pwdPassword.Password = "";
            progrLogin.IsEnabled = false;
            txtUsername.IsEnabled = true;
            pwdPassword.IsEnabled = true;
        }

        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            base.OnKeyUp(e);
            if(e.Key.Equals(VirtualKey.Enter))
            {
                progrLogin.IsEnabled = true;
                txtUsername.IsEnabled = false;
                pwdPassword.IsEnabled = false;
                Login();
            }

        }

        private async void Login()
        {
            try
            {
                HttpResponseMessage response = await myClient.httpClient.GetAsync(new Uri("https://" + myClient.serverAddr + "/rest/auth?password=" + pwdPassword.Password + "&username=" + txtUsername.Text));
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(response.Content.ToString());
                    JsonValue JSResponse = JsonValue.Parse(response.Content.ToString());
                    string returnStatus = JSResponse.GetObject().GetNamedString("status");
                    Debug.WriteLine(returnStatus);
                    if (string.Equals("success", returnStatus))
                    {
                        // Login successfully
                        JsonValue JSData = JSResponse.GetObject().GetNamedValue("data");
                        myClient.token = JSData.GetObject().GetNamedString("token");
                        Debug.WriteLine(myClient.token);
                        this.Frame.Navigate(typeof(BoneFrame), myClient);
                    }
                    else
                    {
                        myClient.ShowMessage("登陆失败", JSResponse.GetObject().GetNamedString("message"));
                        progrLogin.IsEnabled = false;
                        txtUsername.IsEnabled = true;
                        pwdPassword.IsEnabled = true;
                    }

                }
                else
                {
                    myClient.ShowMessage("出错啦", "程序异常，请检查网络状态！");
                    progrLogin.IsEnabled = false;
                    txtUsername.IsEnabled = true;
                    pwdPassword.IsEnabled = true;
                }
            }
            catch (Exception e)
            {
                myClient.ShowMessage("出错啦", "程序异常，请检查网络状态！");
                //throw;
                progrLogin.IsEnabled = false;
                txtUsername.IsEnabled = true;
                pwdPassword.IsEnabled = true;
            }
        }

        private void bLogin_Click(object sender, RoutedEventArgs e)
        {
            progrLogin.IsEnabled = true;
            txtUsername.IsEnabled = false;
            pwdPassword.IsEnabled = false;
            Login();
        }

        private async void Server_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "服务器设置",
                Content = new ServerSettingPanel(),
                PrimaryButtonText = "确认",
                SecondaryButtonText = "取消"
            };

            await dialog.ShowAsync();
        }
    }
}
