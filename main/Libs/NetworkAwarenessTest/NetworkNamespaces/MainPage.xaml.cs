using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace NetworkNamespaces
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += new System.Net.NetworkInformation.NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            listBoxPhone.Items.Insert(0,Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable().ToString());
            listBoxSilverlight.Items.Insert(0,System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable().ToString());
            if (listBoxSilverlight.Items.Count > 21) listBoxSilverlight.Items.RemoveAt(listBoxSilverlight.Items.Count-1);
            if (listBoxPhone.Items.Count > 21) listBoxPhone.Items.RemoveAt(listBoxPhone.Items.Count - 1);
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            listBoxPhone.Items.Insert(0,Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable().ToString());
            listBoxSilverlight.Items.Insert(0,System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable().ToString());
            if (listBoxSilverlight.Items.Count > 21) listBoxSilverlight.Items.RemoveAt(listBoxSilverlight.Items.Count - 1);
            if (listBoxPhone.Items.Count > 21) listBoxPhone.Items.RemoveAt(listBoxPhone.Items.Count - 1);
        }
    }
}