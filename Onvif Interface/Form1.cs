using System;
using System.Windows.Forms;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

using Onvif_Interface.OnvifDeviceManagementServiceReference;
using System.ServiceModel.Discovery;
using System.IO;
using Onvif_Interface.OnvifPtzServiceReference;

namespace Onvif_Interface
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetOnvifInfo_Click(object sender, EventArgs e)
        {
            tssLbl.Text = "Scanning device";
            btnGetOnvifInfo.Enabled = false;
            UseWaitCursor = true;
            try
            {
                GetOnvifInfo(txtIP.Text, (int)numPort.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Exception: {0}", ex.Message));
                MessageBox.Show(string.Format("Exception: {0}", ex.Message), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                btnGetOnvifInfo.Enabled = true;
                UseWaitCursor = false;
            }
        }

        private void GetOnvifInfo(string ip, int port = 80)
        {
            EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/device_service", ip, port));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            // Add our custom behavior - this require the Microsoft WSE 3.0 SDK
            //PasswordDigestBehavior behavior = new PasswordDigestBehavior(CameraASCIIStringLogin, CameraASCIIStringPassword);

            using (DeviceClient client = new DeviceClient(bind, serviceAddress))
            {
                client.Endpoint.Behaviors.Add(new EndpointDiscoveryBehavior());

                // We can now ask for information
                // ONVIF application programmer guide (5.1.3) suggests checking time first 
                // (no auth required) so time offset can be determined (needed for auth if applicable)
                GetDeviceTime(client);

                GetDeviceInfo(client);

                GetCapabilities(client);

                PTZTest(client, ip, port);
            }
        }

        private void GetDeviceTime(DeviceClient client)
        {
            try
            {
                SystemDateTime dt = client.GetSystemDateAndTime();
                string date = string.Format("{0:0000}-{1:00}-{2:00}", dt.UTCDateTime.Date.Year, dt.UTCDateTime.Date.Month, dt.UTCDateTime.Date.Day);
                string time = string.Format("{0:00}:{1:00}:{2:00}", dt.UTCDateTime.Time.Hour, dt.UTCDateTime.Time.Minute, dt.UTCDateTime.Time.Second);
                lblDeviceTime.Text = string.Format("Device Time: {0} {1}", date, time);
                //File.AppendAllText("info.txt", string.Format("\n\nDate and Time from: {0}:{1} [UTC Date: {2}, UTC Time: {3}]", ip, port, date, time));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void GetDeviceInfo(DeviceClient client)
        {
            string model;
            string fwversion;
            string serialno;
            string hwid;
            try
            {
                client.GetDeviceInformation(out model, out fwversion, out serialno, out hwid);
                string deviceInfo = string.Format("\nDevice: {0} [Firmware: {1}, Serial #: {2}, Hardware ID: {3}]\n", model, fwversion, serialno, hwid);
                lblModel.Text = string.Format("Model: {0}", model);
                lblFirmware.Text = string.Format("Firmware: {0}", fwversion);
                lblSerial.Text = string.Format("Serial #: {0}", serialno);
                lblHwID.Text = string.Format("Hardware ID: {0}", hwid);

                tssLbl.Text = string.Format("{0} - Device info retrieved", System.DateTime.Now);
                Console.WriteLine(deviceInfo);
                //File.AppendAllText("info.txt", string.Format("\nDevice: {0} ({4}:{5}) [Firmware: {1}, Serial #: {2}, Hardware ID: {3}]\n", model, fwversion, serialno, hwid, ip, port));
            }
            catch (Exception ex) when (ex.InnerException.Message == "The remote server returned an error: (401) Unauthorized.")
            {
                // Authentication required
                tssLbl.Text = string.Format("{0} Authentication failure. Unable to log into device.", System.DateTime.Now); // - {0}", ex.Message);
                throw new Exception(string.Format("Authentication failure - {0}", ex.Message), ex);
            }
        }

        private void GetCapabilities(DeviceClient client)
        {
            lbxCapabilities.Items.Clear();

            //capabilities = client.GetCapabilities(new CapabilityCategory[] { CapabilityCategory.All });

            //if (capabilities.Analytics != null) { lbxCapabilities.Items.Add("Analytics"); }
            //if (capabilities.Events != null) { lbxCapabilities.Items.Add("Events"); }
            //if (capabilities.Extension != null) { lbxCapabilities.Items.Add("Extension"); }
            //if (capabilities.Imaging != null) { lbxCapabilities.Items.Add("Imaging"); }
            //if (capabilities.Media != null) { lbxCapabilities.Items.Add("Media"); }
            //if (capabilities.PTZ != null) { lbxCapabilities.Items.Add("PTZ"); }

            //lbxCapabilities.Items.Add("");

            Service[] svc = client.GetServices(IncludeCapability: true);
            foreach (Service s in svc)
            {
                Console.WriteLine(s.XAddr + " " + s.Capabilities.NamespaceURI + " " + s.Namespace);
                lbxCapabilities.Items.Add(string.Format("{0}", s.Namespace));
                foreach (System.Xml.XmlNode x in s.Capabilities)
                {
                    Console.WriteLine(string.Format("\t{0}", x.LocalName));
                    lbxCapabilities.Items.Add(string.Format("    {0}", x.LocalName));
                    if (x.Attributes.Count > 0)
                    {
                        foreach (System.Xml.XmlNode a in x.Attributes)
                        {
                            Console.WriteLine(string.Format("\t\t{0} = {1}", a.Name, a.Value));
                            lbxCapabilities.Items.Add(string.Format("        {0} = {1}", a.Name, a.Value));
                        }
                    }
                }
            }

            DeviceServiceCapabilities dsc = client.GetServiceCapabilities();

        }

        private void PTZTest(DeviceClient client, string ip, int port)
        {
            if (lbxCapabilities.Items.Contains("http://www.onvif.org/ver20/ptz/wsdl"))
            {
                // Create Media object
                OnvifMediaServiceReference.MediaClient mediaService = GetOnvifMediaClient(ip, port);

                // Create PTZ object
                PTZClient ptzService = GetOnvifPTZClient(ip, port); // new PTZClient(client.Endpoint.Binding, client.Endpoint.Address);
                Console.WriteLine(ptzService);

                // Get target profile
                string profileToken = "0";
                OnvifMediaServiceReference.Profile mediaProfile = mediaService.GetProfile(profileToken);

                // Get Presets
                PTZPreset[] presets = ptzService.GetPresets(profileToken);

                // Get Status
                PTZStatus status = ptzService.GetStatus(profileToken);

                //// Get Nodes
                //OnvifPtzServiceReference.PTZNode[] nodes = ptz.GetNodes();
                //Console.WriteLine(nodes.Length);

                //foreach (OnvifPtzServiceReference.PTZNode n in nodes)
                //{
                //    lbxCapabilities.Items.Add("PTZ " + n.Name);
                //    File.AppendAllText("ptz.txt", string.Format("\nPTZ node - Name: {0}, Presets: {1}", n.Name, n.MaximumNumberOfPresets));
                //}

                // Fails if not a PTZ
                OnvifPtzServiceReference.PTZNode node = ptzService.GetNode("1"); // nodes[0].token);

                PTZConfiguration[] ptzConfigs = ptzService.GetConfigurations();
                File.AppendAllText("ptz.txt", string.Format("\nPTZ configs found: {0}", ptzConfigs.Length));
                File.AppendAllText("ptz.txt", string.Format("\nPTZ config - Name: {0}", ptzConfigs[0].Name));
                File.AppendAllText("ptz.txt", string.Format("\nPTZ config - Token: {0}", ptzConfigs[0].token));

                try
                {
                    //PTZConfigurationOptions ptzOptions = ptz.GetConfigurationOptions(ptzConfigs[0].token);
                    //File.AppendAllText("ptz.txt", string.Format("\nPTZ options - Direction: {0}, ContinuousPanTiltVelocitySpace: {1}", ptzOptions?.PTControlDirection?.Reverse, ptzOptions?.Spaces?.ContinuousPanTiltVelocitySpace[0].XRange.Max));
                    //File.AppendAllText("ptz.txt", string.Format("\nPTZ options - ContinuousZoomVelocitySpace.URI: {0}", ptzOptions.Spaces.ContinuousZoomVelocitySpace[0].URI));

                    // Add PTZ config to Media profile
                    //mediaClient.AddPTZConfiguration("Profile1", ptzConfigs[0].token);

                    try
                    {
                        PTZSpeed velocity = new PTZSpeed();
                        File.AppendAllText("ptz.txt", string.Format("\nSetting velocity"));

                        //velocity.Zoom = new Vector1D() { x = ptzOptions.Spaces.ContinuousPanTiltVelocitySpace[0].XRange.Max, space = ptzOptions.Spaces.ContinuousZoomVelocitySpace[0].URI }; ;
                        velocity.PanTilt = new Vector2D() { x = (float)-0.5, y = 0 }; ;
                        //File.AppendAllText("ptz.txt", string.Format("\nXRange.Max = {0}", velocity.Zoom.x));
                        //File.AppendAllText("ptz.txt", string.Format("\nContinuousZoomVelocitySpace[0].URI = {0}", velocity.Zoom.space));

                        ptzService.GotoPreset(profileToken, presets[presets.Length - 1].token, velocity);

                        //ptz.ContinuousMove(profileToken, velocity, "");
                        //ptz.GotoHomePosition(profileToken, velocity);

                        ptzService.Stop(profileToken, true, false);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText("ptz.txt", string.Format("\nException trying to PTZ:\n\t{0}\n\tStack Trace: {1}", ex.Message, ex.StackTrace));
                        if (ex.InnerException != null)
                            File.AppendAllText("ptz.txt", string.Format("\n\tInner Exception: {0}", ex.InnerException));
                        throw;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                    throw;
                }
            }
        }

        private OnvifMediaServiceReference.MediaClient GetOnvifMediaClient(string ip, int port)
        {
            EndpointAddress serviceAddress = new EndpointAddress(string.Format("http://{0}:{1}/onvif/media_service", ip, port));

            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;

            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);

            OnvifMediaServiceReference.MediaClient mediaClient = new OnvifMediaServiceReference.MediaClient(bind, serviceAddress);

            return mediaClient;
        }

        private PTZClient GetOnvifPTZClient(string ip, int port)
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
