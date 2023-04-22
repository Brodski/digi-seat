using DigiSeatShared;
using DigiSeatShared.Models;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiSeatFront
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowseTables : Page
    {
        private Integration integration;
        private List<int> _tableExclusionList;
        private Table _table;

        public BrowseTables()
        {
            this.InitializeComponent();
            _tableExclusionList = new List<int>();
            integration = new Integration();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _table = (Table)e.Parameter;
            _tableExclusionList.Add(_table.Id);
        }

        private async void Best_Booth_Tapped(object sender, RoutedEventArgs e)
        {
            var response = await integration.GetTable(_table.PartySize, "booth", _tableExclusionList);
            NavigateToSelectTable(response);
        }
        private async void Best_Table_Tapped(object sender, RoutedEventArgs e)
        {
            var response = await integration.GetTable(_table.PartySize, "table", _tableExclusionList);
            NavigateToSelectTable(response);
        }
        private void Other_Needs_Tapped(object sender, RoutedEventArgs e)
        {
            specialTableFlyout.ShowAt(baseGrid);
        }

        private void NavigateToSelectTable(CheckTableResponse tableResponse)
        {
            if (tableResponse.Status == "wait")
            {
                Frame.Navigate(typeof(WaitStart), tableResponse, new DrillInNavigationTransitionInfo());
                return;
            }

            Frame.Navigate(typeof(SelectTable), tableResponse.BestTable, new DrillInNavigationTransitionInfo());
        }
    }
}
