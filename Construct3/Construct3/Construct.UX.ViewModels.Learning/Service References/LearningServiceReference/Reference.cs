﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17379
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Construct.UX.ViewModels.Learning.LearningServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="LearningServiceReference.IModel")]
    public interface IModel {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModel/GeneratedLabelAttributeVectors", ReplyAction="http://tempuri.org/IModel/GeneratedLabelAttributeVectorsResponse")]
        void GeneratedLabelAttributeVectors(System.Guid sessionID);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IModel/GeneratedLabelAttributeVectors", ReplyAction="http://tempuri.org/IModel/GeneratedLabelAttributeVectorsResponse")]
        System.IAsyncResult BeginGeneratedLabelAttributeVectors(System.Guid sessionID, System.AsyncCallback callback, object asyncState);
        
        void EndGeneratedLabelAttributeVectors(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IModelChannel : Construct.UX.ViewModels.Learning.LearningServiceReference.IModel, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ModelClient : System.ServiceModel.ClientBase<Construct.UX.ViewModels.Learning.LearningServiceReference.IModel>, Construct.UX.ViewModels.Learning.LearningServiceReference.IModel {
        
        private BeginOperationDelegate onBeginGeneratedLabelAttributeVectorsDelegate;
        
        private EndOperationDelegate onEndGeneratedLabelAttributeVectorsDelegate;
        
        private System.Threading.SendOrPostCallback onGeneratedLabelAttributeVectorsCompletedDelegate;
        
        public ModelClient() {
        }
        
        public ModelClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ModelClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ModelClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ModelClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> GeneratedLabelAttributeVectorsCompleted;
        
        public void GeneratedLabelAttributeVectors(System.Guid sessionID) {
            base.Channel.GeneratedLabelAttributeVectors(sessionID);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGeneratedLabelAttributeVectors(System.Guid sessionID, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGeneratedLabelAttributeVectors(sessionID, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndGeneratedLabelAttributeVectors(System.IAsyncResult result) {
            base.Channel.EndGeneratedLabelAttributeVectors(result);
        }
        
        private System.IAsyncResult OnBeginGeneratedLabelAttributeVectors(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Guid sessionID = ((System.Guid)(inValues[0]));
            return this.BeginGeneratedLabelAttributeVectors(sessionID, callback, asyncState);
        }
        
        private object[] OnEndGeneratedLabelAttributeVectors(System.IAsyncResult result) {
            this.EndGeneratedLabelAttributeVectors(result);
            return null;
        }
        
        private void OnGeneratedLabelAttributeVectorsCompleted(object state) {
            if ((this.GeneratedLabelAttributeVectorsCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GeneratedLabelAttributeVectorsCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GeneratedLabelAttributeVectorsAsync(System.Guid sessionID) {
            this.GeneratedLabelAttributeVectorsAsync(sessionID, null);
        }
        
        public void GeneratedLabelAttributeVectorsAsync(System.Guid sessionID, object userState) {
            if ((this.onBeginGeneratedLabelAttributeVectorsDelegate == null)) {
                this.onBeginGeneratedLabelAttributeVectorsDelegate = new BeginOperationDelegate(this.OnBeginGeneratedLabelAttributeVectors);
            }
            if ((this.onEndGeneratedLabelAttributeVectorsDelegate == null)) {
                this.onEndGeneratedLabelAttributeVectorsDelegate = new EndOperationDelegate(this.OnEndGeneratedLabelAttributeVectors);
            }
            if ((this.onGeneratedLabelAttributeVectorsCompletedDelegate == null)) {
                this.onGeneratedLabelAttributeVectorsCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGeneratedLabelAttributeVectorsCompleted);
            }
            base.InvokeAsync(this.onBeginGeneratedLabelAttributeVectorsDelegate, new object[] {
                        sessionID}, this.onEndGeneratedLabelAttributeVectorsDelegate, this.onGeneratedLabelAttributeVectorsCompletedDelegate, userState);
        }
    }
}
