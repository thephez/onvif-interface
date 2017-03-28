using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace OnvifEvents
{
    // These classes are used to parse received SOAP Messages

    class OnvifEventBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            Debug.Print(DateTime.Now + "\tOnvifEventBehavior AddBindingParameters");
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new OnvifEventMessageInspector());
            Debug.Print(DateTime.Now + "\tOnvifEventBehavior ApplyClientBehavior");
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            Debug.Print(DateTime.Now + "\tOnvifEventBehavior ApplyDispatchBehavior");
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            Debug.Print(DateTime.Now + "\tOnvifEventBehavior Validate");
        }
    }


    public class OnvifEventMessageInspector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            Debug.Print(string.Format("{1}\tOnvifEventMessageInspector AfterReceiveReply: {0}", reply.ToString(), DateTime.Now));
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            //Debug.Print(string.Format("{1}OnvifEventMessageInspector BeforeSendRequest: {0}", request.ToString(), DateTime.Now));
            return request;
        }
    }


}
