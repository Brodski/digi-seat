﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DigiSeatShared.Models
{
    public class LayoutTable
    {
        public TranslateTransform Transform { get; set; }
        public Border Rectangle { get; set; }
        public Table Table { get; set; }
    }
}
