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

namespace NetworkDetection
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        NetworkDetector nd = NetworkDetector.Instance;
        int interval = 500;

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            nd.OnAsyncGetNetworkTypeCompleted += new EventHandler<NetworkDetectorEventArgs>(nd_OnAsyncGetNetworkTypeCompleted);
            nd.OnConnectedBroadbandCdma += new EventHandler<NetworkDetectorEventArgs>(nd_OnConnectedBroadbandCdma);
            nd.OnConnectedBroadbandGsm += new EventHandler<NetworkDetectorEventArgs>(nd_OnConnectedBroadbandGsm);
            nd.OnConnectedEthernet += new EventHandler<NetworkDetectorEventArgs>(nd_OnConnectedEthernet);
            nd.OnConnectedNone += new EventHandler<NetworkDetectorEventArgs>(nd_OnConnectedNone);
            nd.OnConnectedOther += new EventHandler<NetworkDetectorEventArgs>(nd_OnConnectedOther);
            nd.OnConnectedWifi += new EventHandler<NetworkDetectorEventArgs>(nd_OnConnectedWifi);
            nd.OnNetworkChanged += new EventHandler<NetworkAvailableEventArgs>(nd_OnNetworkChanged);
            nd.OnNetworkOFF += new EventHandler<NetworkAvailableEventArgs>(nd_OnNetworkOFF);
            nd.OnNetworkON += new EventHandler<NetworkAvailableEventArgs>(nd_OnNetworkON);
            nd.OnZuneConnected += new EventHandler<NetworkDetectorEventArgs>(nd_OnZuneConnected);
            nd.OnZuneDisconnected += new EventHandler<NetworkDetectorEventArgs>(nd_OnZuneDisconnected);
            nd.OnLostNetworkType += new EventHandler<NetworkDetectorEventArgs>(nd_OnLostNetworkType);

            nd.SetNetworkPolling(0,0,interval);
        }

        void nd_OnLostNetworkType(object sender, NetworkDetectorEventArgs e)
        {
            listBox1.Items.Insert(0, "OnLostNetworkType ->previous Nettype:" + e.NetType.ToString());
        }

        void nd_OnZuneDisconnected(object sender, NetworkDetectorEventArgs e)
        {
            listBox1.Items.Insert(0, "OnZuneDisconnected ->Nettype:" + e.NetType.ToString());
        }

        void nd_OnZuneConnected(object sender, NetworkDetectorEventArgs e)
        {
            listBox1.Items.Insert(0, "OnZuneConnected ->Nettype:" + e.NetType.ToString());
        }

        void nd_OnNetworkON(object sender, NetworkAvailableEventArgs e)
        {
            listBox1.Items.Insert(0, "OnNetworkON ->Online:" + e.IsOnline.ToString());
        }

        void nd_OnNetworkOFF(object sender, NetworkAvailableEventArgs e)
        {
            listBox1.Items.Insert(0, "OnNetworkOFF ->Online:" + e.IsOnline.ToString());
        }

        void nd_OnNetworkChanged(object sender, NetworkAvailableEventArgs e)
        {
            listBox1.Items.Insert(0, "OnNetworkChanged ->Online:" + e.IsOnline.ToString());
        }

        void nd_OnConnectedWifi(object sender, NetworkDetectorEventArgs e)
        {
            listBox1.Items.Insert(0, "OnConnectedWifi " + System.DateTime.Now.ToString("T") + " >>>>>>>>");
        }

        void nd_OnConnectedOther(object sender, NetworkDetectorEventArgs e)
        {
            listBox1.Items.Insert(0, "OnConnectedOther " + System.DateTime.Now.ToString("T") + " >>>>>>>>");
        }

        void nd_OnConnectedNone(object sender, NetworkDetectorEventArgs e)
        {
            listBox1.Items.Insert(0, "OnConnectedNone " + System.DateTime.Now.ToString("T") + " >>>>>>>>");
        }

        void nd_OnConnectedEthernet(object sender, NetworkDetectorEventArgs e)
        {
            listBox1.Items.Insert(0, "OnConnectedEthernet " + System.DateTime.Now.ToString("T") + " >>>>>>>>");
        }

        void nd_OnConnectedBroadbandGsm(object sender, NetworkDetectorEventArgs e)
        {
            listBox1.Items.Insert(0, "OnConnectedBroadbandGsm " + System.DateTime.Now.ToString("T") + " >>>>>>>>");
        }

        void nd_OnConnectedBroadbandCdma(object sender, NetworkDetectorEventArgs e)
        {
            listBox1.Items.Insert(0, "OnConnectedBroadbandCdma " + System.DateTime.Now.ToString("T") + " >>>>>>>>");
        }

        void nd_OnAsyncGetNetworkTypeCompleted(object sender, NetworkDetectorEventArgs e)
        {
            listBox1.Items.Insert(0, "OnAsyncGetNetworkTypeCompleted " + System.DateTime.Now.ToString("T") + " >>>>>>>>");
        }

        private void buttonMode_Click(object sender, RoutedEventArgs e)
        {
            nd.DetailedMode = !nd.DetailedMode;
            if (nd.DetailedMode) buttonMode.Content = "DT";
            else buttonMode.Content = "DF";
            listBox1.Items.Insert(0, " DetailedMode : " + nd.DetailedMode.ToString() + " <<<<<<<<<");
        }

        private void buttonTime_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Insert(0, "<<<<< Current Time " + System.DateTime.Now.ToString("T") + " <<<<<<<<<");
        }

        private void buttonStoppPoll_Click(object sender, RoutedEventArgs e)
        {
            nd.DisableNetworkPolling();
        }

        private void buttonStartPoll_Click(object sender, RoutedEventArgs e)
        {
            nd.SetNetworkPolling(0, 0, interval);
        }

        private void buttonIncrease_Click(object sender, RoutedEventArgs e)
        {
            interval += 100;
            //if (interval > 1000) interval = 1000;
            nd.SetNetworkPolling(0, 0, interval);
            listBox1.Items.Insert(0, " Current polling Time " + interval.ToString() + " <<<<<<<<<");
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void buttonDecrease_Click(object sender, RoutedEventArgs e)
        {
            interval -= 100;
            if (interval < 100) interval = 100;
            nd.SetNetworkPolling(0, 0, interval);
            listBox1.Items.Insert(0, " Current polling Time " + interval.ToString() + " <<<<<<<<<");
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Insert(0, ">>> Refresh Started " + System.DateTime.Now.ToString("T") + " >>>>>>>>");
            listBox1.Items.Insert(1, "         Current status: " + nd.GetCurrentNetworkType().ToString());
            nd.AsyncGetNetworkType();
            listBox1.Items.Insert(2, "        Refresh Ended " + System.DateTime.Now.ToString("T"));
        }
    
    }
}