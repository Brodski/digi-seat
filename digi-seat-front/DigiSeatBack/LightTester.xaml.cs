using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiSeatBack
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LightTester : Page
    {
        private BluetoothLEAdvertisementWatcher BleWatcher = null;
        private string LightState = "Light color not retrieved";
        private List<BluetoothLEDevice> Devices = new List<BluetoothLEDevice>();
        private string Light1DeviceId = "BluetoothLE#BluetoothLEb4:ae:2b:e0:10:5e-4e:5a:4b:13:ac:e6";
        private string Saturation = "0";
        private string R = "0";
        private string G = "0";
        private string B = "0";
        private string Effect = "0";
        private string Speed = "0";


        public LightTester()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //FindDevices();
        }
        private async void FindDevices()
        {
            var device = await BluetoothLEDevice.FromIdAsync(Light1DeviceId);
            Devices = new List<BluetoothLEDevice> { device };
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
            if(device.ConnectionStatus != BluetoothConnectionStatus.Connected)
            {
                return;
            }
            Debug.WriteLine($"BLEWATCHER Found: {device.Name}");

            if(!Devices.Where(x => x.DeviceId == device.DeviceId).Any())
            {
                Devices.Add(device);
            }
            else
            {
                var replace = Devices.FirstOrDefault(x => x.DeviceId == device.DeviceId);
                replace = device;
            }
        }

        private async void GetColorAndEffect()
        {
            var device = await BluetoothLEDevice.FromIdAsync(Light1DeviceId);
            if(device == null)
            {
                return;
            }
            var result = await device.GetGattServicesAsync();
            if (result.Status == Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
            {
                var services = result.Services;
                GattCharacteristic characteristic = null;
                var charList = new List<Guid>();
                if (services.Any())
                {
                    foreach (var service in services)
                    {
                        var chars = await service.GetCharacteristicsAsync();
                        if (chars.Characteristics.Any())
                        {


                            foreach (var character in chars.Characteristics)
                            {
                                if (character.Uuid == new Guid("0000fffb-0000-1000-8000-00805f9b34fb"))
                                //var handle = $"0x{character.AttributeHandle.ToString("x4")}";
                                //if (handle == "0x0002")
                                {
                                    characteristic = character;
                                    continue;
                                }
                            }
                            if (characteristic != null)
                            {
                                continue;
                            }
                        }
                    }
                }
                var writer = new DataWriter();
                Bindings.Update();
                var sat = Convert.ToByte(Saturation);
                var r = Convert.ToByte(R);
                var data = new List<byte>
                {
                    Convert.ToByte(Saturation),
                    Convert.ToByte(R),
                    Convert.ToByte(G),
                    Convert.ToByte(B),
                    Convert.ToByte(Effect),
                    0x00,
                    Convert.ToByte(Speed),
                    0x00
                };
                writer.WriteBytes(data.ToArray());
                var writeres = await characteristic.WriteValueAsync(writer.DetachBuffer());
                var res = await characteristic.ReadValueAsync();
                var val = $"handle = 0x{characteristic.AttributeHandle:x4}";
                var prop = characteristic.CharacteristicProperties;
                var prop2 = characteristic.ProtectionLevel;
                var bytearray = res.Value.ToArray();
                var text = BitConverter.ToString(bytearray).Replace("-", "");
                LightState = text;
                Bindings.Update();
            }
        }

        private void Get_Device_Color_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GetColorAndEffect();
        }
    }
}
