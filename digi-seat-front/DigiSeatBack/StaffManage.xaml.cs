using DigiSeatShared;
using DigiSeatShared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiSeatBack
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StaffManage : Page
    {
        private Restaurant _restaurant;
        private ObservableCollection<Staff> _staffOnList = new ObservableCollection<Staff>();
        private ObservableCollection<Staff> _staffOffList = new ObservableCollection<Staff>();
        private Integration integration;
        private Dialogues dialogues;

        public StaffManage()
        {
            this.InitializeComponent();
            integration = new Integration();
            dialogues = new Dialogues();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                var rest = await integration.GetRestaurant();
                _restaurant = rest;
                foreach (var staff in rest.Staff)
                {
                    if (staff.State == "on")
                    {
                        _staffOnList.Add(staff);
                    }
                    else
                    {
                        _staffOffList.Add(staff);
                    }
                }
            }
            catch (Exception ex)
            {
                await dialogues.ShowErrorDialog("Unable to retrieve Restaurant.");
            }
        }

        private void lv_off_Loaded(object sender, RoutedEventArgs e)
        {
            var listView = (ListView)sender;
            listView.ItemsSource = _staffOffList;
        }

        private void lv_on_Loaded(object sender, RoutedEventArgs e)
        {
            var listView = (ListView)sender;
            listView.ItemsSource = _staffOnList;
        }

        private async void Go_Back_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());
        }

        private async void AddServer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateStaff), null, new DrillInNavigationTransitionInfo());
        }

        private async void ClockedIn_clicked(object sender, TappedRoutedEventArgs e)
        {
            var param = sender as Button;
            var staff = (Staff)param.DataContext;

            staff.State = "on";
            var success = await integration.UpdateStaff(staff);
            if (success)
            {
                _staffOffList.Remove(staff);
                _staffOnList.Add(staff);
            }
            else
            {
                await dialogues.ShowErrorDialog("Unable to clock in server.");
            }

        }

        private async void Delete_clicked(object sender, TappedRoutedEventArgs e) 
        {
            var param = sender as Button;
            var staff = (Staff)param.DataContext;

            var success = await integration.DeleteStaff(staff.Id);
            if (success)
            {
                _staffOffList.Remove(staff);
            }
            else
            {
                await dialogues.ShowErrorDialog("Unable to delete");
            }

        }

        private async void ClockOut_clicked(object sender, TappedRoutedEventArgs e)
        {
            
            var param = sender as Button;
            var staff = (Staff)param.DataContext;

            staff.State = "off";
            var success = await integration.UpdateStaff(staff);
            if (success)
            {
                _staffOnList.Remove(staff);
                _staffOffList.Add(staff);
            }
            else
            {
                await dialogues.ShowErrorDialog("Unable to clock out server.");
            }
        }
    }
}
