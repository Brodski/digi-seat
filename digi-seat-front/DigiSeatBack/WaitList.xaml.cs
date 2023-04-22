using DigiSeatShared;
using DigiSeatShared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiSeatBack
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WaitList : Page
    {
        private Integration integration;

        public WaitList()
        {
            this.InitializeComponent();
            integration = new Integration();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            BuildWaitList();
        }

        private async Task BuildWaitList()
        {
            var waitlist = await integration.GetAllWaits();

            if (waitlist.Any())
            {
                waitView.Items.Clear();
                foreach (var waitPerson in waitlist)
                {
                    var waitingTime = (int)(DateTime.UtcNow - waitPerson.Created).TotalMinutes; 
                    var listBoxItem = new ListBoxItem {
                                   FontSize = 32,
                                   Content = $"{waitPerson.Name} | Status: {waitPerson.State} | Wait Time: {waitingTime} | Estimated Wait: {waitPerson.EstimatedWait}",
                                   Tag = waitPerson.Id //Tag is an arbitrary object for the programer's use.
                             };
                    listBoxItem.Tapped += Item_Tapped;
                    waitView.Items.Add(listBoxItem);
                                        
                }
            }
        }

        private void Go_Back_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());
        }

        private async void Item_Tapped(object sender, RoutedEventArgs e)
        {
            var item = (ListBoxItem)sender;
            var dialog = new ContentDialog
            {
                Content = $"Delete person: {item.Content} \nID: {item.Tag}",
                IsSecondaryButtonEnabled = false,
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Primary
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var result2 = await integration.DeleteWaitPerson((int) item.Tag);
                await BuildWaitList();
            }

        }
    }
}
