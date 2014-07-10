﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17379
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Construct.MessageBrokering.TransponderService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="TransponderService.ITransponder")]
    public interface ITransponder {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITransponder/AddObject", ReplyAction="http://tempuri.org/ITransponder/AddObjectResponse")]
        bool AddObject(string jsonObject);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/ITransponder/AddObject", ReplyAction="http://tempuri.org/ITransponder/AddObjectResponse")]
        System.IAsyncResult BeginAddObject(string jsonObject, System.AsyncCallback callback, object asyncState);
        
        bool EndAddObject(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ITransponderChannel : Construct.MessageBrokering.TransponderService.ITransponder, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AddObjectCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public AddObjectCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TransponderClient : System.ServiceModel.ClientBase<Construct.MessageBrokering.TransponderService.ITransponder>, Construct.MessageBrokering.TransponderService.ITransponder {
        
        private BeginOperationDelegate onBeginAddObjectDelegate;
        
        private EndOperationDelegate onEndAddObjectDelegate;
        
        private System.Threading.SendOrPostCallback onAddObjectCompletedDelegate;
        
        public TransponderClient() {
        }
        
        public TransponderClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TransponderClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TransponderClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TransponderClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<AddObjectCompletedEventArgs> AddObjectCompleted;
        
        public bool AddObject(string jsonObject) {
            return base.Channel.AddObject(jsonObject);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginAddObject(string jsonObject, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginAddObject(jsonObject, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public bool EndAddObject(System.IAsyncResult result) {
            return base.Channel.EndAddObject(result);
        }
        
        private System.IAsyncResult OnBeginAddObject(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string jsonObject = ((string)(inValues[0]));
            return this.BeginAddObject(jsonObject, callback, asyncState);
        }
        
        private object[] OnEndAddObject(System.IAsyncResult result) {
            bool retVal = this.EndAddObject(result);
            return new object[] {
                    retVal};
        }
        
        private void OnAddObjectCompleted(object state) {
            if ((this.AddObjectCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.AddObjectCompleted(this, new AddObjectCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void AddObjectAsync(string jsonObject) {
            this.AddObjectAsync(jsonObject, null);
        }
        
        public void AddObjectAsync(string jsonObject, object userState) {
            if ((this.onBeginAddObjectDelegate == null)) {
                this.onBeginAddObjectDelegate = new BeginOperationDelegate(this.OnBeginAddObject);
            }
            if ((this.onEndAddObjectDelegate == null)) {
                this.onEndAddObjectDelegate = new EndOperationDelegate(this.OnEndAddObject);
            }
            if ((this.onAddObjectCompletedDelegate == null)) {
                this.onAddObjectCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnAddObjectCompleted);
            }
            base.InvokeAsync(this.onBeginAddObjectDelegate, new object[] {
                        jsonObject}, this.onEndAddObjectDelegate, this.onAddObjectCompletedDelegate, userState);
        }
    }
}
