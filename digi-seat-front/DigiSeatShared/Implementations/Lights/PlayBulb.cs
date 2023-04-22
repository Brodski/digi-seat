
using DigiSeatShared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace DigiSeatShared.Implementations.Lights
{
    public class PlayBulb : ILightIntegration
    {
        public async Task TurnOn(string address, string color)
        {
            var lightConfig = new List<byte> { 0x05, 0x00, 0x00, 0x00 };
            var data = GetColorHex(color);
            data.AddRange(lightConfig);
            await SendCommand(data, address);
        }

        private List<byte> GetColorHex(string color)
        {
            switch (color)
            {
                case "Green":
                    return new List<byte> { 0x40, 0x00, 0xa3, 0x00 };
                case "Red":
                    return new List<byte> { 0x25, 0xff, 0x00, 0x00 };
                case "Yellow":
                    return new List<byte> { 0x80, 0xff, 0xff, 0x00 };
                case "Pink":
                    return new List<byte> { 0x00, 0xff, 0x64, 0x64 };
                case "Teal":
                    return new List<byte> { 0x00, 0x32, 0xe6, 0x50 };
                case "Orange":
                    return new List<byte> { 0x30, 0xff, 0x50, 0x00 };
                case "Blue":
                    return new List<byte> { 0x20, 0x00, 0x00, 0xff };
                case "Purple":
                    return new List<byte> { 0x00, 0x99, 0x00, 0xcc };
                case "LightBlue":
                    return new List<byte> { 0x00, 0x50, 0xb4, 0xff };
                case "LightGreen":
                    return new List<byte> { 0xa8, 0x1e, 0xff, 0x1e };
                default:
                    return new List<byte> { 0x00, 0xff, 0xff, 0xff };
            }
        }

        private async Task SendCommand(List<byte> command, string address)
        {
            var device = await BluetoothLEDevice.FromIdAsync(address);
            if (device == null)
            {
                return;
            }
            var result = await device.GetGattServicesAsync(BluetoothCacheMode.Uncached);
            if (result.Status == Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
            {
                var services = result.Services;
                GattCharacteristic characteristic = null;
                var charList = new List<Guid>();
                if (services.Any())
                {
                    foreach (var service in services)
                    {
                        var chars = await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
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
                writer.WriteBytes(command.ToArray());
                var writeres = await characteristic.WriteValueAsync(writer.DetachBuffer());
                device.Dispose();
                device = null;
                GC.Collect();
            }
        }

        public async Task TurnOff(string address)
        {
            var data = new List<byte>();
            if (Settings.UseLightsAsCandle)
            {
                data.AddRange(new List<byte> { 0xff, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00 });
            }
            else
            {
                data.AddRange(new List<byte> { 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00 });
            }

            await SendCommand(data, address);
        }
    }
}
