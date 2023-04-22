using DigiSeatShared.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DigiSeatShared.Implementations.Lights
{

    public class PhilipsHue : ILightIntegration
    {
        private static HttpClient _client;
        private readonly string _address = "http://10.0.0.112";
        private readonly string _key = "dG9FU898bylhq1puHY91tgu-e2VRTEo8wmlYxhRB";

        public PhilipsHue()
        {
            _client = new HttpClient();
        }

        public async Task TurnOn(string address, string color)
        {
            try
            {
                var url = $"{_address}/api/{_key}/lights/{address}/state";
                var request = new HttpRequestMessage(HttpMethod.Put, url);
                var details = new PhilipsHueRequest
                {
                    Bri = 200,
                    Hue = GetColorHue(color),
                    On = true,
                    Sat = 190
                };

                var json = JsonConvert.SerializeObject(details);
                request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                await _client.SendAsync(request);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task TurnOff(string address)
        {
            try
            {
                var url = $"{_address}/api/{_key}/lights/{address}/state";
                var request = new HttpRequestMessage(HttpMethod.Put, url);
                var details = new PhilipsHueRequest
                {
                    Bri = 200,
                    On = false,
                    Sat = 190
                };

                var json = JsonConvert.SerializeObject(details);
                request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                await _client.SendAsync(request);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private int GetColorHue(string color)
        {
            var hue = Enum.Parse(typeof(ColorHue), color);
            return (int)hue;
        }

        internal enum ColorHue
        {
            Green = 16000,
            Red = 0,
            Yellow = 8000,
            Pink = 53000,
            Teal = 29000,
            Orange = 3800,
            Blue = 40000,
            Purple = 48000,
            LightBlue = 40000,
            LightGreen = 16000
        }

        private class PhilipsHueRequest
        {
            [JsonProperty("on")]
            public bool On { get; set; }
            [JsonProperty("sat")]
            public int Sat { get; set; }
            [JsonProperty("bri")]
            public int Bri { get; set; }
            [JsonProperty("hue")]
            public long Hue { get; set; }
        }
    }
}
