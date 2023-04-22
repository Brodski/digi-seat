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

namespace DigiSeatFront
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AcceptedTable : Page
    {
        public string LightText = "";
        public string TableDirections = "";

        private static string _lightColor { get; set; }
        private static string _tableDirections { get; set; }

        public AcceptedTable()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var parameter = (SitResponse)e.Parameter;
            _lightColor = parameter.LightColor;
            _tableDirections = parameter.SectionLocation;
            LightText = $"Look for the {_lightColor} light";
            TableDirections = $"Please walk {_tableDirections}";
            Bindings.Update();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());
        }
    }
}
