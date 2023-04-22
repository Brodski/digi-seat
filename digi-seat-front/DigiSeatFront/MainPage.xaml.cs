using DigiSeatShared;
using DigiSeatShared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DigiSeatFront
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Integration integration;
        private Dialogues dialogues;

        public MainPage()
        {
            this.InitializeComponent();
            integration = new Integration();
            dialogues = new Dialogues();
        }

        public async void One_Tapped(object sender, TappedRoutedEventArgs e)
        { 
            var table = await integration.GetTable(1);
            NavigateToSelectTable(table);
        }

        private async void Two_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var table = await integration.GetTable(2);
            NavigateToSelectTable(table);
        }

        private async void Three_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var table = await integration.GetTable(3);
            NavigateToSelectTable(table);
        }
        private async void Four_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var table = await integration.GetTable(4);
            NavigateToSelectTable(table);
        }
        private async void Five_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var table = await integration.GetTable(5);
            NavigateToSelectTable(table);
        }
        private async void Six_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var table = await integration.GetTable(6);
            NavigateToSelectTable(table);
        }
        private async void Seven_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var table = await integration.GetTable(7);
            NavigateToSelectTable(table);
        }
        private async void Eight_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var table = await integration.GetTable(8);
            NavigateToSelectTable(table);
        }

        private async void Plus_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int result = await dialogues.GetUserInput("Enter your party size");
            if (result > 0)
            {
                var table = await integration.GetTable(result);
                NavigateToSelectTable(table);
            }
            else if (result != -1 ) // doesnt quite work as intended. 
            {
                await dialogues.ShowErrorDialog("You entered an invalid number");
            }
        }

        private void HaveCode_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(EnterCode), null, new DrillInNavigationTransitionInfo());
        }

        private void NavigateToSelectTable(CheckTableResponse tableResponse)
        {
            if(tableResponse.Status == "wait")
            {
                Frame.Navigate(typeof(WaitStart), tableResponse, new DrillInNavigationTransitionInfo());
                return;
            }
            if (tableResponse.BestTable == null)
            {
                dialogues.ShowErrorDialog("No tables could support a party of that size");
            }
            else
            {
                Frame.Navigate(typeof(SelectTable), tableResponse.BestTable, new DrillInNavigationTransitionInfo());
            }
            
        }
    }
}
