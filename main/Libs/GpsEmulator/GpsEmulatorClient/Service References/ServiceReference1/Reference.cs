﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.Phone.ServiceReference, version 3.7.0.0
// 
namespace GpsEmulatorClient.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IGpsEmulatorService")]
    public interface IGpsEmulatorService {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IGpsEmulatorService/GetCurrentPosition", ReplyAction="http://tempuri.org/IGpsEmulatorService/GetCurrentPositionResponse")]
        System.IAsyncResult BeginGetCurrentPosition(System.AsyncCallback callback, object asyncState);
        
        string EndGetCurrentPosition(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGpsEmulatorServiceChannel : GpsEmulatorClient.ServiceReference1.IGpsEmulatorService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetCurrentPositionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetCurrentPositionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public string Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GpsEmulatorServiceClient : System.ServiceModel.ClientBase<GpsEmulatorClient.ServiceReference1.IGpsEmulatorService>, GpsEmulatorClient.ServiceReference1.IGpsEmulatorService {
        
        private BeginOperationDelegate onBeginGetCurrentPositionDelegate;
        
        private EndOperationDelegate onEndGetCurrentPositionDelegate;
        
        private System.Threading.SendOrPostCallback onGetCurrentPositionCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public GpsEmulatorServiceClient() {
        }
        
        public GpsEmulatorServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GpsEmulatorServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GpsEmulatorServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GpsEmulatorServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<GetCurrentPositionCompletedEventArgs> GetCurrentPositionCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult GpsEmulatorClient.ServiceReference1.IGpsEmulatorService.BeginGetCurrentPosition(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetCurrentPosition(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        string GpsEmulatorClient.ServiceReference1.IGpsEmulatorService.EndGetCurrentPosition(System.IAsyncResult result) {
            return base.Channel.EndGetCurrentPosition(result);
        }
        
        private System.IAsyncResult OnBeginGetCurrentPosition(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((GpsEmulatorClient.ServiceReference1.IGpsEmulatorService)(this)).BeginGetCurrentPosition(callback, asyncState);
        }
        
        private object[] OnEndGetCurrentPosition(System.IAsyncResult result) {
            string retVal = ((GpsEmulatorClient.ServiceReference1.IGpsEmulatorService)(this)).EndGetCurrentPosition(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetCurrentPositionCompleted(object state) {
            if ((this.GetCurrentPositionCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetCurrentPositionCompleted(this, new GetCurrentPositionCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetCurrentPositionAsync() {
            this.GetCurrentPositionAsync(null);
        }
        
        public void GetCurrentPositionAsync(object userState) {
            if ((this.onBeginGetCurrentPositionDelegate == null)) {
                this.onBeginGetCurrentPositionDelegate = new BeginOperationDelegate(this.OnBeginGetCurrentPosition);
            }
            if ((this.onEndGetCurrentPositionDelegate == null)) {
                this.onEndGetCurrentPositionDelegate = new EndOperationDelegate(this.OnEndGetCurrentPosition);
            }
            if ((this.onGetCurrentPositionCompletedDelegate == null)) {
                this.onGetCurrentPositionCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetCurrentPositionCompleted);
            }
            base.InvokeAsync(this.onBeginGetCurrentPositionDelegate, null, this.onEndGetCurrentPositionDelegate, this.onGetCurrentPositionCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override GpsEmulatorClient.ServiceReference1.IGpsEmulatorService CreateChannel() {
            return new GpsEmulatorServiceClientChannel(this);
        }
        
        private class GpsEmulatorServiceClientChannel : ChannelBase<GpsEmulatorClient.ServiceReference1.IGpsEmulatorService>, GpsEmulatorClient.ServiceReference1.IGpsEmulatorService {
            
            public GpsEmulatorServiceClientChannel(System.ServiceModel.ClientBase<GpsEmulatorClient.ServiceReference1.IGpsEmulatorService> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginGetCurrentPosition(System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[0];
                System.IAsyncResult _result = base.BeginInvoke("GetCurrentPosition", _args, callback, asyncState);
                return _result;
            }
            
            public string EndGetCurrentPosition(System.IAsyncResult result) {
                object[] _args = new object[0];
                string _result = ((string)(base.EndInvoke("GetCurrentPosition", _args, result)));
                return _result;
            }
        }
    }
}
