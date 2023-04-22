using DigiSeatShared;
using DigiSeatShared.Models;
using Newtonsoft.Json;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiSeatFront
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EnterCode : Page
    {
        private string Code;
        private Integration integration;

        public EnterCode()
        {
            this.InitializeComponent();
            integration = new Integration();
        }

        private async void Next_Clicked(object sender, RoutedEventArgs e)
        {
            Wait wait = await integration.GetWaitWithCode(Code);
            Frame.Navigate(typeof(SelectTable), wait.Table, new DrillInNavigationTransitionInfo());
        }
        private void GoBack_Clicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());
        }
    }
}
