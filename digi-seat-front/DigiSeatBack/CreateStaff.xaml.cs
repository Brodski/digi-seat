using DigiSeatShared;
using DigiSeatShared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiSeatBack
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateStaff : Page
    {
        private Integration integration;
        private string _name = "";

        public CreateStaff()
        {
            this.InitializeComponent();
            integration = new Integration();
        }

        private async void Go_Back_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GoToStaffManage();
        }

        private async void Create_Tapped(object sender, RoutedEventArgs e)
        {
            Bindings.Update();
            var staff = new Staff { Name = _name };
            var success = await integration.CreateStaff(staff);
            if (success)
            {
                GoToStaffManage();
            }
//            if(result == HttpStatusCode.Ok)
  //          {
    //            GoToStaffManage();
      //      }
        }

        private void GoToStaffManage()
        {
            Frame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());
        }
    }
}
