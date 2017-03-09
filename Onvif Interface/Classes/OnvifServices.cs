using Onvif_Interface.OnvifDeviceManagementServiceReference;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using Onvif_Interface.OnvifPtzServiceReference;
using Microsoft.Web.Services3.Security.Tokens;
using System.Xml;
using Onvif_Interface.WsSecurity;

namespace SDS.Video.Onvif
{
    static class OnvifServices
    {

        public static DeviceClient GetOnvifDeviceClient(string ip, int port, string username = "", string password = "")
        {
            EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/device_service", ip, port));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            DeviceClient deviceClient = new DeviceClient(bind, serviceAddress);

            if (username != string.Empty)
            {
                // Handles adding of SOAP Security header containing User Token (user, nonce, pwd digest)
                PasswordDigestBehavior behavior = new PasswordDigestBehavior(username, password);
                deviceClient.Endpoint.Behaviors.Add(behavior);
            }

            return deviceClient;
        }

        public static Onvif_Interface.OnvifMediaServiceReference.MediaClient GetOnvifMediaClient(string ip, int port, string username = "", string password = "")
        {
            EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/media_service", ip, port));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            Onvif_Interface.OnvifMediaServiceReference.MediaClient mediaClient = new Onvif_Interface.OnvifMediaServiceReference.MediaClient(bind, serviceAddress);

            if (username != string.Empty)
            {
                // Handles adding of SOAP Security header containing User Token (user, nonce, pwd digest)
                PasswordDigestBehavior behavior = new PasswordDigestBehavior(username, password);
                mediaClient.Endpoint.Behaviors.Add(behavior);
            }
            
            return mediaClient;
        }

        public static PTZClient GetOnvifPTZClient(string ip, int port, string username = "", string password = "")
        {
            EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/ptz_service", ip, port));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            PTZClient ptzClient = new PTZClient(bind, serviceAddress);

            if (username != string.Empty)
            {
                // Handles adding of SOAP Security header containing User Token (user, nonce, pwd digest)
                PasswordDigestBehavior behavior = new PasswordDigestBehavior(username, password);
                ptzClient.Endpoint.Behaviors.Add(behavior);
            }

            return ptzClient;
        }

        //public static DeviceClient GetOnvifDeviceClient2(string ip, int port, string user, string password)
        //{
        //    EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/device_service", ip, port));

        //    UsernameToken token = new UsernameToken(user, password, PasswordOption.SendHashed);
        //    XmlElement securityToken = token.GetXml(new XmlDocument());
        //    MessageHeader securityHeader = MessageHeader.CreateHeader("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", securityToken, false);

        //    HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
        //    httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

        //    var messageElement = new TextMessageEncodingBindingElement();
        //    messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
        //    CustomBinding bind = new CustomBinding(messageElement, httpBinding);

        //    DeviceClient deviceClient = new DeviceClient(bind, serviceAddress);

        //    //deviceClient.ClientCredentials.UserName.UserName = user;
        //    //deviceClient.ClientCredentials.UserName.Password = password;
            
        //    return deviceClient;
        //}
    }
}
