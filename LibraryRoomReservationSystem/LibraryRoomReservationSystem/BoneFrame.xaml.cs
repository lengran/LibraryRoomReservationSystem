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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace LibraryRoomReservationSystem
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BoneFrame : Page
    {
        private SystemClient myClient;

        public BoneFrame()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            myClient = (SystemClient)e.Parameter;
            mainFrame.Navigate(typeof(Main), myClient);
        }

        private void Status_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(typeof(Main), myClient);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(typeof(About), myClient);
        }

        private void CampusCard_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(typeof(CampusCardService));
        }

        private void Quick_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(typeof(Quick), myClient);
        }
        // Unfinished Module
        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), myClient);
        }

        private void Map_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(typeof(About), myClient);
        }
    }
}
