﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Construct.Sensors.DragonTranscriptionSensor.AggregatorService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="SqlGeography", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Types")]
    [System.SerializableAttribute()]
    public partial class SqlGeography : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private Construct.Sensors.DragonTranscriptionSensor.AggregatorService.GeoData _geometryField;
        
        private bool _isNullField;
        
        private int _sridField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public Construct.Sensors.DragonTranscriptionSensor.AggregatorService.GeoData _geometry {
            get {
                return this._geometryField;
            }
            set {
                if ((this._geometryField.Equals(value) != true)) {
                    this._geometryField = value;
                    this.RaisePropertyChanged("_geometry");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public bool _isNull {
            get {
                return this._isNullField;
            }
            set {
                if ((this._isNullField.Equals(value) != true)) {
                    this._isNullField = value;
                    this.RaisePropertyChanged("_isNull");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int _srid {
            get {
                return this._sridField;
            }
            set {
                if ((this._sridField.Equals(value) != true)) {
                    this._sridField = value;
                    this.RaisePropertyChanged("_srid");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GeoData", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Types")]
    [System.SerializableAttribute()]
    public partial struct GeoData : System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private Construct.Sensors.DragonTranscriptionSensor.AggregatorService.ExtendedGeometryProperties m_extendedUserPropertiesField;
        
        private bool m_fValidField;
        
        private Construct.Sensors.DragonTranscriptionSensor.AggregatorService.Figure[] m_figuresField;
        
        private double[] m_mValuesField;
        
        private Construct.Sensors.DragonTranscriptionSensor.AggregatorService.Point[] m_pointsField;
        
        private Construct.Sensors.DragonTranscriptionSensor.AggregatorService.Shape[] m_shapesField;
        
        private double[] m_zValuesField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public Construct.Sensors.DragonTranscriptionSensor.AggregatorService.ExtendedGeometryProperties m_extendedUserProperties {
            get {
                return this.m_extendedUserPropertiesField;
            }
            set {
                if ((this.m_extendedUserPropertiesField.Equals(value) != true)) {
                    this.m_extendedUserPropertiesField = value;
                    this.RaisePropertyChanged("m_extendedUserProperties");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public bool m_fValid {
            get {
                return this.m_fValidField;
            }
            set {
                if ((this.m_fValidField.Equals(value) != true)) {
                    this.m_fValidField = value;
                    this.RaisePropertyChanged("m_fValid");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public Construct.Sensors.DragonTranscriptionSensor.AggregatorService.Figure[] m_figures {
            get {
                return this.m_figuresField;
            }
            set {
                if ((object.ReferenceEquals(this.m_figuresField, value) != true)) {
                    this.m_figuresField = value;
                    this.RaisePropertyChanged("m_figures");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public double[] m_mValues {
            get {
                return this.m_mValuesField;
            }
            set {
                if ((object.ReferenceEquals(this.m_mValuesField, value) != true)) {
                    this.m_mValuesField = value;
                    this.RaisePropertyChanged("m_mValues");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public Construct.Sensors.DragonTranscriptionSensor.AggregatorService.Point[] m_points {
            get {
                return this.m_pointsField;
            }
            set {
                if ((object.ReferenceEquals(this.m_pointsField, value) != true)) {
                    this.m_pointsField = value;
                    this.RaisePropertyChanged("m_points");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public Construct.Sensors.DragonTranscriptionSensor.AggregatorService.Shape[] m_shapes {
            get {
                return this.m_shapesField;
            }
            set {
                if ((object.ReferenceEquals(this.m_shapesField, value) != true)) {
                    this.m_shapesField = value;
                    this.RaisePropertyChanged("m_shapes");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public double[] m_zValues {
            get {
                return this.m_zValuesField;
            }
            set {
                if ((object.ReferenceEquals(this.m_zValuesField, value) != true)) {
                    this.m_zValuesField = value;
                    this.RaisePropertyChanged("m_zValues");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.FlagsAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="ExtendedGeometryProperties", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Types")]
    public enum ExtendedGeometryProperties : byte {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        IsWholeGlobeProp = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        None = 0,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Figure", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Types")]
    [System.SerializableAttribute()]
    public partial struct Figure : System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private Construct.Sensors.DragonTranscriptionSensor.AggregatorService.FigureAttributes figureAttributeField;
        
        private int pointOffsetField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public Construct.Sensors.DragonTranscriptionSensor.AggregatorService.FigureAttributes figureAttribute {
            get {
                return this.figureAttributeField;
            }
            set {
                if ((this.figureAttributeField.Equals(value) != true)) {
                    this.figureAttributeField = value;
                    this.RaisePropertyChanged("figureAttribute");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int pointOffset {
            get {
                return this.pointOffsetField;
            }
            set {
                if ((this.pointOffsetField.Equals(value) != true)) {
                    this.pointOffsetField = value;
                    this.RaisePropertyChanged("pointOffset");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Point", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Types")]
    [System.SerializableAttribute()]
    public partial struct Point : System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private double xField;
        
        private double yField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public double x {
            get {
                return this.xField;
            }
            set {
                if ((this.xField.Equals(value) != true)) {
                    this.xField = value;
                    this.RaisePropertyChanged("x");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public double y {
            get {
                return this.yField;
            }
            set {
                if ((this.yField.Equals(value) != true)) {
                    this.yField = value;
                    this.RaisePropertyChanged("y");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Shape", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Types")]
    [System.SerializableAttribute()]
    public partial struct Shape : System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int figureOffsetField;
        
        private int parentOffsetField;
        
        private Construct.Sensors.DragonTranscriptionSensor.AggregatorService.OpenGisType typeField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int figureOffset {
            get {
                return this.figureOffsetField;
            }
            set {
                if ((this.figureOffsetField.Equals(value) != true)) {
                    this.figureOffsetField = value;
                    this.RaisePropertyChanged("figureOffset");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int parentOffset {
            get {
                return this.parentOffsetField;
            }
            set {
                if ((this.parentOffsetField.Equals(value) != true)) {
                    this.parentOffsetField = value;
                    this.RaisePropertyChanged("parentOffset");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public Construct.Sensors.DragonTranscriptionSensor.AggregatorService.OpenGisType type {
            get {
                return this.typeField;
            }
            set {
                if ((this.typeField.Equals(value) != true)) {
                    this.typeField = value;
                    this.RaisePropertyChanged("type");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FigureAttributes", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Types")]
    public enum FigureAttributes : byte {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ExteriorRing = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Stroke = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        InteriorRing = 0,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OpenGisType", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Types")]
    public enum OpenGisType : byte {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        GeometryCollection = 7,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        MultiPolygon = 6,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        MultiLineString = 5,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        MultiPoint = 4,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Polygon = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        LineString = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Point = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Unknown = 0,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Construct.Aggregation", ConfigurationName="AggregatorService.IAggregator")]
    public interface IAggregator {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Construct.Aggregation/IAggregator/GetID", ReplyAction="http://Construct.Aggregation/IAggregator/GetIDResponse")]
        System.Guid GetID();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Construct.Aggregation/IAggregator/AddItem", ReplyAction="http://Construct.Aggregation/IAggregator/AddItemResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Construct.Sensors.DragonTranscriptionSensor.AggregatorService.SqlGeography))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Construct.Sensors.DragonTranscriptionSensor.AggregatorService.GeoData))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Construct.Sensors.DragonTranscriptionSensor.AggregatorService.ExtendedGeometryProperties))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Construct.Sensors.DragonTranscriptionSensor.AggregatorService.Figure[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Construct.Sensors.DragonTranscriptionSensor.AggregatorService.Figure))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Construct.Sensors.DragonTranscriptionSensor.AggregatorService.FigureAttributes))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Construct.Sensors.DragonTranscriptionSensor.AggregatorService.Point[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Construct.Sensors.DragonTranscriptionSensor.AggregatorService.Point))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Construct.Sensors.DragonTranscriptionSensor.AggregatorService.Shape[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Construct.Sensors.DragonTranscriptionSensor.AggregatorService.Shape))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Construct.Sensors.DragonTranscriptionSensor.AggregatorService.OpenGisType))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(double[]))]
        void AddItem(System.Guid sourceID, System.DateTime createdTime, Construct.Sensors.DragonTranscriptionSensor.AggregatorService.SqlGeography createdLocation, object blob);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Construct.Aggregation/IAggregator/AddTelemetry", ReplyAction="http://Construct.Aggregation/IAggregator/AddTelemetryResponse")]
        void AddTelemetry(System.Guid sourceID, string theTelemetry);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Construct.Aggregation/IAggregator/AddStream", ReplyAction="http://Construct.Aggregation/IAggregator/AddStreamResponse")]
        void AddStream(System.Guid sourceID);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAggregatorChannel : Construct.Sensors.DragonTranscriptionSensor.AggregatorService.IAggregator, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AggregatorClient : System.ServiceModel.ClientBase<Construct.Sensors.DragonTranscriptionSensor.AggregatorService.IAggregator>, Construct.Sensors.DragonTranscriptionSensor.AggregatorService.IAggregator {
        
        public AggregatorClient() {
        }
        
        public AggregatorClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AggregatorClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AggregatorClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AggregatorClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Guid GetID() {
            return base.Channel.GetID();
        }
        
        public void AddItem(System.Guid sourceID, System.DateTime createdTime, Construct.Sensors.DragonTranscriptionSensor.AggregatorService.SqlGeography createdLocation, object blob) {
            base.Channel.AddItem(sourceID, createdTime, createdLocation, blob);
        }
        
        public void AddTelemetry(System.Guid sourceID, string theTelemetry) {
            base.Channel.AddTelemetry(sourceID, theTelemetry);
        }
        
        public void AddStream(System.Guid sourceID) {
            base.Channel.AddStream(sourceID);
        }
    }
}
