using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.UI;

namespace EyeLens.Controls
{
    public partial class ColorInfoControl : UserControl
    {
        public ColorInfoControl()
        {
            InitializeComponent();
        }

        private string _colorName;

        public string CurrentColorName
        {
            get { return _colorName; }
            set { _colorName = value; }
        }

        private Color _currentColor;

        public Color CurrentColor
        {
            get { return _currentColor; }
            set { _currentColor = value; }
        }
        
    }
}
