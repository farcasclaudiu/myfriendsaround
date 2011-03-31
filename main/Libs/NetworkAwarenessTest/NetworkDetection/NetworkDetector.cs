using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;



/**********************************************/
/*      Copyright of Gabor Dolhai @ 2010      */
/*            under MS-Pl license             */
/*       can be used freely for anybody       */
/*   If you use this code please send me an   */
/*email about your project to dolhaig at gmail*/
/*******************Thanks*********************/
namespace NetworkDetection
{
    #region Custom Enums and EventArgs
    public enum NetworkTypeRequestStatus
    {
        Default = 0,
        Started,//represents BackgroundWorker started
        Ended//BackgroundWorker Finished
    }

    public class NetworkAvailableEventArgs : System.EventArgs
    {

        public NetworkAvailableEventArgs(bool isOnline)
        {
            IsOnline = isOnline;
        }

        public bool IsOnline { get; private set; }
    }

    public class NetworkDetectorEventArgs : System.EventArgs
    {

        public NetworkDetectorEventArgs(bool isOnline, Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType netType)
        {
            IsOnline = isOnline;
            NetType = netType;
        }

        public bool IsOnline { get; private set; }
        public Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType NetType { get; private set; }
    }
    #endregion

    public class NetworkDetector
    {
        private static readonly NetworkDetector _instance = new NetworkDetector();

        #region Events
        public event EventHandler<NetworkAvailableEventArgs> OnNetworkON;
        public event EventHandler<NetworkAvailableEventArgs> OnNetworkOFF;
        public event EventHandler<NetworkAvailableEventArgs> OnNetworkChanged;

        public event EventHandler<NetworkDetectorEventArgs> OnZuneConnected;
        public event EventHandler<NetworkDetectorEventArgs> OnZuneDisconnected;
        public event EventHandler<NetworkDetectorEventArgs> OnConnectedEthernet;
        public event EventHandler<NetworkDetectorEventArgs> OnConnectedWifi;
        public event EventHandler<NetworkDetectorEventArgs> OnConnectedNone;
        public event EventHandler<NetworkDetectorEventArgs> OnConnectedBroadbandGsm;
        public event EventHandler<NetworkDetectorEventArgs> OnConnectedBroadbandCdma;
        public event EventHandler<NetworkDetectorEventArgs> OnConnectedOther;
        public event EventHandler<NetworkDetectorEventArgs> OnLostNetworkType;

        public event EventHandler<NetworkDetectorEventArgs> OnAsyncGetNetworkTypeCompleted;
        #endregion

        private System.Windows.Threading.DispatcherTimer updateTimer, pollTimer;
        private Queue<long> requestQueue;  //queue to store the requests timestemps
        private BackgroundWorker networkWorker;
        private bool online = false;
        private Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType net;
        private NetworkTypeRequestStatus requestStatus;  //current status of the BackgroundWorker
        private bool IsInstantRequestPresent;
        private bool detailedMode;
        private bool isZuneConnected;

        private NetworkDetector()
        {
            requestQueue = new Queue<long>();
            requestQueue.Clear();
            requestStatus = NetworkTypeRequestStatus.Default;
            updateTimer = new System.Windows.Threading.DispatcherTimer();
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);//there is no need to restart the BGWorker sooner then 300 millisec because the ~3request/sec requestlimit
            //updateTimer.Start();
            pollTimer = new System.Windows.Threading.DispatcherTimer();
            pollTimer.Tick += new EventHandler(pollTimer_Tick);
            networkWorker = new BackgroundWorker();
            networkWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(networkWorker_RunWorkerCompleted);
            networkWorker.DoWork += new DoWorkEventHandler(networkWorker_DoWork);
            IsInstantRequestPresent = false;
            detailedMode = false; //by default I hide the framework events for better Developer experience
            isZuneConnected = false;

            SetupNetworkChange();  //signing on the framework event
        }

        public static NetworkDetector Instance
        {
            get
            {
                return _instance;
            }
        }

        #region BackgroundWorker
        void networkWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Debug.WriteLine(">>>>> GetNetType started " + System.DateTime.Now.ToString("T") + " >>>>>");
            e.Result = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;
        }
        //no need to lock the variables if We do everithing in the completed event handler

        void networkWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DetectOnlineStatus();
            if ((detailedMode) || (net != (Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType)e.Result))
            {
                //there is no need to get events all the time, just when really changing something or the DetailedMode is true
                if (net != (Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType)e.Result)
                    RaiseNotify(OnLostNetworkType, new NetworkDetectorEventArgs(online, net));
                net = (Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType)e.Result;
                Debug.WriteLine("      New NetType: " + net.ToString());
                if (net == Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ethernet) Debug.WriteLine("!!!!! Zune is Connected");
                switch (net)
                {
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ethernet:
                        if (!isZuneConnected)
                        {
                            isZuneConnected = true;
                            RaiseNotify(OnZuneConnected, new NetworkDetectorEventArgs(online, net));
                        }
                        RaiseNotify(OnConnectedEthernet, new NetworkDetectorEventArgs(online, net));
                        break;
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Wireless80211:
                        if (isZuneConnected)
                        {
                            isZuneConnected = false;
                            RaiseNotify(OnZuneDisconnected, new NetworkDetectorEventArgs(online, net));
                        }
                        RaiseNotify(OnConnectedWifi, new NetworkDetectorEventArgs(online, net));
                        break;
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandCdma:
                        if (isZuneConnected)
                        {
                            isZuneConnected = false;
                            RaiseNotify(OnZuneDisconnected, new NetworkDetectorEventArgs(online, net));
                        }
                        RaiseNotify(OnConnectedBroadbandCdma, new NetworkDetectorEventArgs(online, net));
                        break;
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandGsm:
                        if (isZuneConnected)
                        {
                            isZuneConnected = false;
                            RaiseNotify(OnZuneDisconnected, new NetworkDetectorEventArgs(online, net));
                        }
                        RaiseNotify(OnConnectedBroadbandGsm, new NetworkDetectorEventArgs(online, net));
                        break;
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None:
                        if (!online)
                        {
                            if (isZuneConnected)
                            {
                                /*if we lost all network connection and the Zune was present before, 
                                 then we lost the Zune too. Normally when Zune is present and then we got
                                 a None NetType, the PC just lost the internet connection but not the Zune sync*/
                                isZuneConnected = false;
                                RaiseNotify(OnZuneDisconnected, new NetworkDetectorEventArgs(online, net));
                            }
                        }
                        RaiseNotify(OnConnectedNone, new NetworkDetectorEventArgs(online, net));
                        break;
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.AsymmetricDsl:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Atm:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.BasicIsdn:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ethernet3Megabit:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.FastEthernetFx:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.FastEthernetT:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Fddi:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.GenericModem:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.GigabitEthernet:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.HighPerformanceSerialBus:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.IPOverAtm:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Isdn:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Loopback:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MultiRateSymmetricDsl:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ppp:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.PrimaryIsdn:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.RateAdaptDsl:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Slip:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.SymmetricDsl:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.TokenRing:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Tunnel:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Unknown:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.VeryHighSpeedDsl:
                    default://theoretically we can't get here but better be prepared
                        RaiseNotify(OnConnectedOther, new NetworkDetectorEventArgs(online, net));
                        break;
                }
            }
            for (int i = 0; i < requestQueue.Count; i++)
            {
                if (requestQueue.Peek() < System.DateTime.Now.Ticks) requestQueue.Dequeue();
                //the requests before this moment just got answered
                //if other requests are coming right after this, they will be served inside the next networkWorker_RunWorkerCompleted
            }
            if (requestQueue.Count == 0) updateTimer.Stop();
            if (IsInstantRequestPresent)  //the user requested a single poll
            {
                RaiseNotify(OnAsyncGetNetworkTypeCompleted, new NetworkDetectorEventArgs(online, net));
                IsInstantRequestPresent = false;
            }
            requestStatus = NetworkTypeRequestStatus.Ended;
            Debug.WriteLine("<<<<< GetNetType ended " + System.DateTime.Now.ToString("T") + " <<<<<");
        }
        #endregion

        #region Private Functions
        private void EnqueueRequest()
        {
            requestQueue.Enqueue(System.DateTime.Now.Ticks);
            if (!updateTimer.IsEnabled) updateTimer.Start();
        }

        private void DetectOnlineStatus() //are we connected to any network or not
        {
            if (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                if (!online)
                {
                    online = true; //the network just came back
                    RaiseNotify(OnNetworkON, new NetworkAvailableEventArgs(online));
                    // do what is needed to GoOnline();
                }
            }
            else
            {
                if (online)
                {
                    online = false;  //we just lost all network connectivity
                    RaiseNotify(OnNetworkOFF, new NetworkAvailableEventArgs(online));
                    Debug.WriteLine("-----  No network available.   -----");
                    // do what is needed to GoOffline();
                }
            }
        }

        void pollTimer_Tick(object sender, EventArgs e)
        {
            EnqueueRequest();
        }

        void updateTimer_Tick(object sender, EventArgs e)
        {
            if (requestQueue.Count > 0)
            {
                if (!networkWorker.IsBusy)
                {
                    requestStatus = NetworkTypeRequestStatus.Started;
                    networkWorker.RunWorkerAsync();
                }
            }
        }

        #region Event Handling
        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("+++++ Changed started " + System.DateTime.Now.ToString("T") + " +++++");
            DetectOnlineStatus();
            if (detailedMode) RaiseNotify(OnNetworkChanged, new NetworkAvailableEventArgs(online));
            Debug.WriteLine("      IsOnline : " + online.ToString());
            Debug.WriteLine("      Current NetType: " + net.ToString());
            Debug.WriteLine("+++++ Changed Ended " + System.DateTime.Now.ToString("T"));
            Debug.WriteLine("      Changed GetNetType Launch" + System.DateTime.Now.ToString("T"));
            EnqueueRequest();
        }

        private void SetupNetworkChange() 
        {
            // Get current network availalability and store the 
            // initial value of the online variable
            if (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                online = true;
                // do what is needed to GoOnline();
            }
            else
            {
                online = false;
                // do what is needed to GoOffline();
            }
            // Now add a network change event handler to indicate network availability 
            EnqueueRequest();
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += new System.Net.NetworkInformation.NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
        }

        protected void RaiseNotify(EventHandler<NetworkDetectorEventArgs> handler, NetworkDetectorEventArgs e)
        {
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseNotify(EventHandler<NetworkAvailableEventArgs> handler, NetworkAvailableEventArgs e)
        {
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #endregion

        #region Public Functions and Properties
        public void AsyncGetNetworkType()
        {
            //requestQueue.Enqueue(System.DateTime.Now.Ticks);
            IsInstantRequestPresent = true;
            if (!networkWorker.IsBusy)
            {
                requestStatus = NetworkTypeRequestStatus.Started;
                networkWorker.RunWorkerAsync();
            }
            if (!updateTimer.IsEnabled) updateTimer.Start();
        }

        public void SetNetworkPolling(int Minutes, int Seconds, int Milliseconds)
        {
            pollTimer.Interval = new TimeSpan(0, 0, Minutes, Seconds, Milliseconds);
            if (!pollTimer.IsEnabled) pollTimer.Start();
        }

        public void DisableNetworkPolling()
        {
            if (pollTimer.IsEnabled) pollTimer.Stop();
        }



        public Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType GetCurrentNetworkType()
        {
            return net;
        }

        public NetworkTypeRequestStatus GetRequestStatus()
        {
            //NetworkTypeRequestStatus temp = requestStatus;
            //if (requestStatus == NetworkTypeRequestStatus.Ended) requestStatus = NetworkTypeRequestStatus.Default;
            //return temp;
            return requestStatus;
        }

        public bool DetailedMode
        {
            get
            {
                return detailedMode;
            }
            set
            {
                detailedMode = value;
            }
        }

        public bool GetZuneStatus()
        {
            return isZuneConnected;
        }
        #endregion
    }
}
