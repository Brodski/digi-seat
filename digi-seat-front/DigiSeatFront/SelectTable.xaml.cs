using DigiSeatShared;
using DigiSeatShared.Implementations.BackgroundWorker;
using DigiSeatShared.Implementations.Lights;
using DigiSeatShared.Interfaces;
using DigiSeatShared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace DigiSeatFront
{
    public sealed partial class SelectTable : Page
    {
        private Integration integration;
        private Dialogues dialogues;
        private readonly ILightIntegration _light;
        public Table Table { get; set; }
        public string TableDescription = "";
        public string TableMap = @"Assets\Loading_icon.gif";

        public SelectTable()
        {
            this.InitializeComponent();
            integration = new Integration();
            dialogues = new Dialogues();
            if (Settings.LightType == "playbulb")
            {
                _light = new PlayBulb();
            }
            else
            {
                _light = new PhilipsHue();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var parameter = (Table)e.Parameter;
            Table = parameter;
            TableDescription = $"Table {Table.Number} - {Table.TableType}";
            TableMap = $"{Settings.ApiBaseUrl}/TableMaps/{Table.RestaurantId}/Table{Table.Number}.png";
            Bindings.Update();
        }

        private async void Accept_Table_Tap(object sender, RoutedEventArgs e)
        {
            var sitResponse = new SitResponse();

            try
            {
                sitResponse = await integration.SitTable(Table.Id, Table.PartySize);
            }

            catch
            {
                await dialogues.ShowErrorDialog("We were unable to reserve your table.");
            }
            await LightQueueReader.AddItem(new LightQueueCommand { Address = Table.LightAddress, TurnOn = true, Color = sitResponse.LightColor });
            Frame.Navigate(typeof(AcceptedTable), sitResponse, new DrillInNavigationTransitionInfo());
        }

        private async void Browse_Tables_Tapped(object sender, RoutedEventArgs e)
        {
            try
            {
                 integration.OpenTable(Table.Id);
            }
            catch
            {
                //Do nothing, not customer's problem
            }
            Frame.Navigate(typeof(BrowseTables), Table, new DrillInNavigationTransitionInfo());
        }

        private async void Go_Home_Tapped(object sender, RoutedEventArgs e)
        {
            try
            {
                integration.OpenTable(Table.Id);
            }
            catch
            {
                //Do nothing, not customer's problem
            }
            Frame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());
        }
    }
}
