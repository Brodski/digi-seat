using DigiSeatShared;
using DigiSeatShared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
//using Windows.Web.Http;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiSeatBack
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateTable : Page
    {

        private Table _addTable = new Table();
        private BluetoothLEAdvertisementWatcher BleWatcher = null;
        private bool LightFound = false;
        private bool Editing = false;
        private Integration integration;

        public CreateTable()
        {
            this.InitializeComponent();
            integration = new Integration();
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter != null)
            {
                _addTable = (Table)e.Parameter;
                Editing = true;
                CheckTableType(_addTable.TableType);
                CheckTableShape(_addTable.Shape);
                Bindings.Update();
            }
        }

        private void CheckTableType(string type)
        {
            switch (type)
            {
                case "booth":
                    Radio_Booth.IsChecked = true;
                    return;
                case "table":
                    Radio_Table.IsChecked = true;
                    return;
                default:
                    Radio_Split.IsChecked = true;
                    return;
            }
        }


        private void CheckTableShape(string type)
        {
            switch (type)
            {
                case "round":
                    Radio_Round.IsChecked = true;
                    return;
                case "square":
                    Radio_Square.IsChecked = true;
                    return;
                default:
                    Radio_Rectangle.IsChecked = true;
                    return;
            }
        }

        private void Add_Table_Type_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            _addTable.TableType = rb.Tag.ToString();

        }

        private void Add_Table_Shape_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            _addTable.Shape = rb.Tag.ToString();
        }

        private async void SaveTable_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Bindings.Update();
            if (_addTable.Number.Length > 0 && _addTable.Capacity > 0 && !string.IsNullOrEmpty(_addTable.LightAddress) && _addTable.TableType.Length > 0)
            { 
                var client = integration.GetClient();
                var request = new HttpRequestMessage();

                //Editing relies on _addTable having all of the data about the table from the api
                if (Editing)
                {
                    request.Method = HttpMethod.Put;
                    request.RequestUri = new Uri($"{Settings.ApiBaseUrl}/api/tables/save");
                    request.Content = integration.GetContentString(new List<Table> { _addTable });
                }

                else
                {
                    _addTable.State = "open";
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri($"{Settings.ApiBaseUrl}/api/tables");
                    request.Content = integration.GetContentString(_addTable);
                }

                //var response = await client.SendRequestAsync(request);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    _addTable.Number = "";
                    _addTable.Capacity = 0;
                    _addTable.LightAddress = "";
                    _addTable.TableType = "";
                    Bindings.Update();
                    Frame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());
                }
            }
        }

        private async void Go_Back_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());
        }

        void SyncLight_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Discover();
        }

        void Discover()
        {
            BleWatcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Active
            };
            BleWatcher.Received += OnAdvertisementReceived;
            BleWatcher.Start();
        }


        private async void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            var device = await BluetoothLEDevice.FromBluetoothAddressAsync(eventArgs.BluetoothAddress);
            if(device != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (device.Name.Contains("PLAYBULB"))
                    {
                        _addTable.LightAddress = device.DeviceId;
                        Bindings.Update();
                        LightFound = true;
                        BleWatcher.Stop();
                    }
                });
            }
        }

        private void CloseLightSearch_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LightSearch.Hide();
        }

        private void LightSearch_Closed(object sender, object e)
        {
            BleWatcher.Stop();
            LightFound = false;
        }
    }
}
