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

namespace NetworkAwarenessTest
{
    public partial class MainPage : PhoneApplicationPage
    {
        string NetType = "";
        bool online = false;



        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
            Unloaded += new RoutedEventHandler(MainPage_Unloaded);
        }

        void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged -= new System.Net.NetworkInformation.NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Insert(0, ">>> Refresh Started " + System.DateTime.Now.ToString("T") + " >>>>>>>>");
            Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType net = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;
            NetType = net.ToString();
            listBox1.Items.Insert(1,"         Refresh Ended " + System.DateTime.Now.ToString("T"));
            listBox1.Items.Insert(2, "         stored at Changed event: " + NetType);
            listBox1.Items.Insert(3, "         Current status: " + net.ToString());
            if (net == Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ethernet) listBox1.Items.Insert(4, "!!!!! Zune is Connected");
            SetupNetworkChange();
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            listBox1.Items.Insert(0, "+++++ Changed started " + System.DateTime.Now.ToString("T") + " +++++");
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                if (!online)
                {
                    online = true;
                    // do what is needed to GoOnline();
                }
            }
            else
            {
                if (online)
                {
                    online = false;
                    listBox1.Items.Insert(0, "-----  No network available.   -----");
                    // do what is needed to GoOffline();
                }
            }
            listBox1.Items.Insert(1, "              Changed GetNetType " + System.DateTime.Now.ToString("T"));
            Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType net = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;
            NetType = net.ToString();
            listBox1.Items.Insert(2, "              Changed Ended " + System.DateTime.Now.ToString("T"));
            listBox1.Items.Insert(3, "              IsOnline : " + online.ToString());
            listBox1.Items.Insert(4, "              Current status: " + net.ToString());
            if (net == Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ethernet) listBox1.Items.Insert(5,"!!!!! Zune is Connected");
        }

        private void SetupNetworkChange()
        {
            // Get current network availalability and store the 
            // initial value of the online variable
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                online = true;
                // do what is needed to GoOnline();
            }
            else
            {
                online = false;
                // do what is needed to GoOffline();
            }

            // Now add a network change event handler to indicate
            // network availability 
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += new System.Net.NetworkInformation.NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Insert(0, ">>> Refresh Started " + System.DateTime.Now.ToString("T") + " >>>>>>>>");
            Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType net = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;
            //MessageBox.Show("Current status: "+net.ToString(), "stored nettype:" + App.NetType, MessageBoxButton.OK);
            listBox1.Items.Insert(1, "        Refresh Ended " + System.DateTime.Now.ToString("T"));
            listBox1.Items.Insert(2,"         stored at Changed event: " + NetType);
            listBox1.Items.Insert(3, "         Current status: " + net.ToString());
        }

        private void buttonTime_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Insert(0, "<<<<< Current Time " + System.DateTime.Now.ToString("T") + " <<<<<<<<<");
        }
    }
}