using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace EyeLens
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.NavigationService.GoBack();
            }
            catch { };
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                this.NavigationService.GoBack();
            }
            catch { };
        }
    }
}