using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace DigiSeatShared
{
    public static class ColorHelper
    {
        public static SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            return myBrush;
        }

        public static Brush GetTableColor(string state)
        {
            switch (state)
            {
                case "open":
                    return ColorHelper.GetSolidColorBrush("FF212121"); 
                case "holding":
                    return ColorHelper.GetSolidColorBrush("FF212121"); 
                case "seated":
                    return ColorHelper.GetSolidColorBrush("FF424242");
                default:
                    return new SolidColorBrush(Windows.UI.Colors.Red);
            }
        }


        public static Brush GetBorderColor(int index)
        {
            var colorLookup = new Dictionary<int, string>
            {
                {0, "FF0033cc" }, // blue
                {1, "FFff33cc" }, // pinkish
                {2, "FFe60000" }, // red
                {3, "FF009900" },  // green
                {4, "FF4d2600" }, // brown 
                {5, "FFff911e" }, // burnt orange
                {6, "FF00cc99" }, // teal
                {7, "FF009900" }, //light pink
                {8, "FFFFFFFF" }, // white
                {9, "FFaaffc3" }, //light green

            };

            return ColorHelper.GetSolidColorBrush(colorLookup[index]);
        }


        /*        public static Brush GetBorderColor(string state)
                {
                    switch (state)
                    {
                        case "open":
                            return ColorHelper.GetSolidColorBrush("FF424242");
                        case "holding":
                            return new SolidColorBrush(Windows.UI.Colors.Blue);
                        case "seated":
                            return ColorHelper.GetSolidColorBrush("FF424242");
                        default:
                            return new SolidColorBrush(Windows.UI.Colors.Red);
                    }
                }
                */
    }
}
