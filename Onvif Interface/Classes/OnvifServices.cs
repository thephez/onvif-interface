using Onvif_Interface.OnvifDeviceManagementServiceReference;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using Onvif_Interface.OnvifPtzServiceReference;

namespace SDS.Video.Onvif
{
    static class OnvifServices
    {

        public static DeviceClient GetOnvifDeviceClient(string ip, int port)
        {
            EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/device_service", ip, port));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            DeviceClient deviceClient = new DeviceClient(bind, serviceAddress);

            return deviceClient;
        }

        public static Onvif_Interface.OnvifMediaServiceReference.MediaClient GetOnvifMediaClient(string ip, int port)
        {
            EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/media_service", ip, port));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            Onvif_Interface.OnvifMediaServiceReference.MediaClient mediaClient = new Onvif_Interface.OnvifMediaServiceReference.MediaClient(bind, serviceAddress);

            return mediaClient;
        }

        public static PTZClient GetOnvifPTZClient(string ip, int port)
        {
            EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/ptz_service", ip, port));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            PTZClient ptzClient = new PTZClient(bind, serviceAddress);

            return ptzClient;
        }
    }
}
