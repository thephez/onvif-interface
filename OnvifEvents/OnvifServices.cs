using OnvifEvents.OnvifEventServiceReference;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace OnvifEvents
{
    public static class OnvifServices
    {

        public static EventPortTypeClient GetEventClient(string ip, int port)
        {
            EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/event_service", ip, port));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            EventPortTypeClient eptc = new EventPortTypeClient(bind, serviceAddress);

            //// Handles parsing SOAP messages received
            //OnvifEventBehavior behavior = new OnvifEventBehavior();
            //eptc.Endpoint.Behaviors.Add(behavior);

            return eptc;
        }


        public static PullPointSubscriptionClient GetPullPointSubClient(string ip, int port)
        {
            EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/event_service", ip, port));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            PullPointSubscriptionClient ppsc = new PullPointSubscriptionClient(bind, serviceAddress);
            return ppsc;
        }


        public static PullPointSubscriptionClient GetPullPointSubClient(string ip, int port, string toHeaderAddr,  List<MessageHeader> headers)
        {
            //EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/event_service", ip, port));
            AddressHeader[] addrHeader; // = AddressHeader.CreateAddressHeader("Action", "http://www.w3.org/2005/08/addressing", "http://docs.oasis-open.org/wsn/bw-2/NotificationProducer/SubscribeRequest");
            addrHeader = new AddressHeader[2];
            addrHeader[0] = AddressHeader.CreateAddressHeader("Action", "http://www.w3.org/2005/08/addressing", "http://www.onvif.org/ver10/events/wsdl/PullPointSubscription/PullMessagesRequest");
            addrHeader[1] = AddressHeader.CreateAddressHeader("To", "http://www.w3.org/2005/08/addressing", toHeaderAddr);
            //addrHeader[1] = AddressHeader.CreateAddressHeader("MessageID", "http://www.w3.org/2005/08/addressing", "urn:uuid:e47b9746-c5c1-455e-839f-0db5bed7d8c7");
            //addrHeader[1] = AddressHeader.CreateAddressHeader("ReplyTo", "http://www.w3.org/2005/08/addressing", "http://www.w3.org/2005/08/addressing/anonymous");


            //MessageHeader[] msgHeader;
            //msgHeader = new MessageHeader[1];
            //msgHeader[0] = MessageHeader.CreateHeader("Action", "http://www.w3.org/2005/08/addressing", "http://docs.oasis-open.org/wsn/bw-2/NotificationProducer/SubscribeRequest", true);

            EndpointAddress serviceAddress = new EndpointAddress(
                new System.Uri(string.Format("http://{0}:{1}/onvif/event_service", ip, port)),
                addrHeader);

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            PullPointSubscriptionClient ppsc = new PullPointSubscriptionClient(bind, serviceAddress);
            return ppsc;
        }

        public static PullPointClient GetPullPointClient(string ip, int port)
        {
            EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/event_service", ip, port));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);
            
            PullPointClient client = new PullPointClient(bind, serviceAddress);
            return client;
        }

        public static NotificationProducerClient GetNotificationProducerClient(string ip, int port)
        {
            EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/event_service", ip, port));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            NotificationProducerClient client = new NotificationProducerClient(bind, serviceAddress);
            return client;
        }

        public static NotificationConsumerClient GetNotificationConsumerClient(string ip, int port, string subscription)
        {

            EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/{2}", ip, port, subscription));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.WSAddressing10);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            NotificationConsumerClient client = new NotificationConsumerClient(bind, serviceAddress);
            return client;
        }


        //public static SubscriptionManagerClient GetSubscriptionManagerClient(string ip, int port, List<MessageHeader> headers)
        public static SubscriptionManagerClient GetSubscriptionManagerClient(string uri) // string ip, int port, List<MessageHeader> headers)
        {
            EndpointAddress serviceAddress = new EndpointAddress(uri); // string.Format("http://{0}:{1}/onvif/event_service", ip, port));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            SubscriptionManagerClient client = new SubscriptionManagerClient(bind, serviceAddress);
            return client;
        }
    }
}
