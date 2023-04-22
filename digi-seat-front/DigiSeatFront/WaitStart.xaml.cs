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
    public sealed partial class WaitStart : Page
    {
        private string WaitText = "";
        private string PhoneNumber = "";
        private string CustomerName = "";
        private int? WaitMintutes;
        private int PartySize = 0;
        private bool _phoneNumberStarted = false;
        private Integration integration;

        public WaitStart()
        {
            this.InitializeComponent();
            integration = new Integration();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var parameter = (CheckTableResponse)e.Parameter;

            WaitText = $"We have a {parameter.MinutesWait} minute wait.";
            WaitMintutes = parameter.MinutesWait;
            PartySize = parameter.PartySize;
            Bindings.Update();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO: Input mask
            //if (sender is TextBox tb)
            //{
            //    if (!_phoneNumberStarted)
            //    {
            //        PhoneNumber = string.Concat("(",PhoneNumber);
            //        _phoneNumberStarted = true;
            //    }

            //    else if (PhoneNumber.Length == 4)
            //    {
            //        PhoneNumber = String.Concat(")-", PhoneNumber);
            //    }

            //    else if (PhoneNumber.Length == 9)
            //    {
            //        PhoneNumber = String.Concat(PhoneNumber, "-");
            //    }
            //}
        }

        private async void Continue_Clicked(object sender, RoutedEventArgs e)
        {
            Bindings.Update();
            var wait = new Wait() { EstimatedWait = (int)WaitMintutes, Phone = PhoneNumber, Name = CustomerName, PartySize = PartySize };

            await integration.PostWait(wait);

            Frame.Navigate(typeof(WaitSubmitted), null, new DrillInNavigationTransitionInfo());
        }

        private void NoThanks_Clicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());
        }
    }
}
