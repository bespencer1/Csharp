﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoapServiceTest.DataIntegrationService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://dis.lashgroup.com", ConfigurationName="DataIntegrationService.IDataIntegrationService")]
    public interface IDataIntegrationService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://dis.lashgroup.com/Request", ReplyAction="http://dis.lashgroup.com/Response")]
        System.ServiceModel.Channels.Message Request(System.ServiceModel.Channels.Message request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://dis.lashgroup.com/Request", ReplyAction="http://dis.lashgroup.com/Response")]
        System.Threading.Tasks.Task<System.ServiceModel.Channels.Message> RequestAsync(System.ServiceModel.Channels.Message request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDataIntegrationServiceChannel : SoapServiceTest.DataIntegrationService.IDataIntegrationService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DataIntegrationServiceClient : System.ServiceModel.ClientBase<SoapServiceTest.DataIntegrationService.IDataIntegrationService>, SoapServiceTest.DataIntegrationService.IDataIntegrationService {
        
        public DataIntegrationServiceClient() {
        }
        
        public DataIntegrationServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DataIntegrationServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataIntegrationServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataIntegrationServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.ServiceModel.Channels.Message Request(System.ServiceModel.Channels.Message request) {
            return base.Channel.Request(request);
        }
        
        public System.Threading.Tasks.Task<System.ServiceModel.Channels.Message> RequestAsync(System.ServiceModel.Channels.Message request) {
            return base.Channel.RequestAsync(request);
        }
    }
}
