using System;
using System.Windows.Forms;

using Onvif_Interface.OnvifDeviceManagementServiceReference;
using System.ServiceModel.Discovery;
using System.IO;
using Onvif_Interface.OnvifPtzServiceReference;
using System.ServiceModel.Description;
using SDS.Video.Onvif;
using System.Text;
using OnvifEvents;
using Onvif_Interface.OnvifEventServiceReference;
using System.Net;
using System.ServiceModel;

namespace Onvif_Interface
{
    public partial class Form1 : Form
    {
        private string IP;
        private int Port;
        OnvifHttpListener HttpListener = new OnvifHttpListener();

        System.DateTime? SubTermTime;
        Timer SubRenewTimer = new Timer();
        string SubRenewUri;
        SubscriptionManagerClient SubscriptionManagerClient;
        
        public Form1()
        {
            InitializeComponent();
            btnPanLeft.MouseDown += BtnPanLeft_MouseDown;
            btnPanRight.MouseDown += BtnPanRight_MouseDown;
            btnTiltDown.MouseDown += BtnTiltDown_MouseDown;
            btnTiltUp.MouseDown += BtnTiltUp_MouseDown;
            btnZoomIn.MouseDown += BtnZoomIn_MouseDown;
            btnZoomOut.MouseDown += BtnZoomOut_MouseDown;

            btnPanLeft.MouseUp += BtnPan_MouseUp;
            btnPanRight.MouseUp += BtnPan_MouseUp;
            btnTiltDown.MouseUp += BtnTilt_MouseUp;
            btnTiltUp.MouseUp += BtnTilt_MouseUp;
            btnZoomIn.MouseUp += BtnZoom_MouseUp;
            btnZoomOut.MouseUp += BtnZoom_MouseUp;

            btnPreset1.Click += BtnPreset_Click;
            btnPreset2.Click += BtnPreset_Click;
            btnPreset3.Click += BtnPreset_Click;
            btnPreset4.Click += BtnPreset_Click;
            btnPreset5.Click += BtnPreset_Click;

            chkShowPwd.CheckedChanged += ChkShowPwd_CheckedChanged;

            //// If this is not set to false, the HTTP header includes "Expect: 100-Continue"
            //// This causes Samsung Onvif cameras to respond with error "417 - Expectation Failed"
            System.Net.ServicePointManager.Expect100Continue = false;

            // Start http listener to receive events
            HttpListener.StartHttpServer(8080);
            HttpListener.Notification += HttpListener_Notification;
        }

        private void HttpListener_Notification(object sender, EventArgs e)
        {
            lbxEvents.Items.Add("Notification(s) received");
            NotificationEventArgs n = (NotificationEventArgs)e;

            foreach (string notification in n.Notifications)
            {
                lbxEvents.Items.Add(string.Format("  {0}", notification));
                Console.WriteLine(string.Format("  {0}", notification));
            }

            lbxEvents.SelectedIndex = lbxEvents.Items.Count - 1;
        }

        public void Subscribe(string ip, int port)
        {
            EventPortTypeClient eptc = OnvifServices.GetEventClient(ip, port, txtUser.Text, txtPassword.Text);

            string localIP = GetLocalIp(); // "172.16.5.111";

            // Producer client
            NotificationProducerClient npc = OnvifServices.GetNotificationProducerClient(ip, port, txtUser.Text, txtPassword.Text);
            npc.Endpoint.Address = eptc.Endpoint.Address;

            Subscribe s = new Subscribe();
            // Consumer reference tells the device where to Post messages back to (the client)
            EndpointReferenceType clientEndpoint = new EndpointReferenceType() { Address = new AttributedURIType() { Value = string.Format("http://{0}:8080/subscription-1", localIP) } };
            s.ConsumerReference = clientEndpoint;
            s.InitialTerminationTime = "PT60S";

            try
            {
                SubscribeResponse sr = npc.Subscribe(s);

                // Store the subscription URI for use in Renew
                SubRenewUri = sr.SubscriptionReference.Address.Value;

                // Start timer to periodically check if a Renew request needs to be issued
                // Use PC time for timer in case camera time doesn't match PC time
                // This works fine because the renew command issues a relative time (i.e. PT60S) so PC/Camera mismatch doesn't matter
                SubTermTime = System.DateTime.UtcNow.AddSeconds(50); // sr.TerminationTime;
                SubRenewTimer.Start();
                SubRenewTimer.Interval = 1000;
                SubRenewTimer.Tick += SubRenewTimer_Tick;

                lbxEvents.Items.Add(string.Format("Initial Termination Time: {0} (Current Time: {1})", SubTermTime, System.DateTime.UtcNow));

                SubscriptionManagerClient = OnvifServices.GetSubscriptionManagerClient(SubRenewUri, txtUser.Text, txtPassword.Text);
            }
            catch (Exception e)
            {
                lbxEvents.Items.Add(string.Format("{0} Unable to subscribe to events on device [{1}]", System.DateTime.UtcNow, ip));
            }
        }

        /// <summary>
        /// Issues a subscription renew message ~10 seconds before the subscription's scheduled termination time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubRenewTimer_Tick(object sender, EventArgs e)
        {
            var timeDiff = SubTermTime - System.DateTime.UtcNow;
            var x = timeDiff.Value.Ticks / TimeSpan.TicksPerSecond;
            if (x < 10)
            {
                // Send renew
                Renew();
            }
        }

        /// <summary>
        /// Renews the SubscriptionManagerClient's subscription
        /// </summary>
        public void Renew()
        {
            Console.WriteLine(System.DateTime.Now + "\tIssue subscription renew");
            RenewResponse oRenewResult = SubscriptionManagerClient.Renew(new Renew() { TerminationTime = "PT60S" });
            SubTermTime = oRenewResult.TerminationTime;
            Console.WriteLine(string.Format("Current Time: {0}\tTermination Time: {1}", oRenewResult.CurrentTime.ToString(), oRenewResult.TerminationTime.Value.ToString()));
            lbxEvents.Items.Add(string.Format("Subscription renewed - Current Time: {0}\tTermination Time: {1}", oRenewResult.CurrentTime.ToString(), oRenewResult.TerminationTime.Value.ToString()));
            lbxEvents.SelectedIndex = lbxEvents.Items.Count - 1;
        }

        /// <summary>
        /// Unsubscribes from the subscription in the SubscriptionManagerClient
        /// </summary>
        public void Unsubscribe()
        {
            if ((SubscriptionManagerClient != null) && ((SubscriptionManagerClient.State == CommunicationState.Opened) | (SubscriptionManagerClient.State == CommunicationState.Created)))
            {
                Unsubscribe u = new Unsubscribe();
                UnsubscribeResponse oUnSubResult = SubscriptionManagerClient.Unsubscribe(u);
                SubscriptionManagerClient.Close();
                SubRenewTimer.Stop();

                lbxEvents.Items.Add(string.Format("Subscription canceled - Current Time: {0}", System.DateTime.UtcNow));
                lbxEvents.SelectedIndex = lbxEvents.Items.Count - 1;
            }
            else
            {
                lbxEvents.Items.Add(string.Format("No subscription to cancel - Current Time: {0}", System.DateTime.UtcNow));
                lbxEvents.SelectedIndex = lbxEvents.Items.Count - 1;
            }
        }

        public string GetLocalIp()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (System.Net.IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    return localIP;
                }
            }

            throw new Exception("Local IP address not found");
        }


        private void btnGetOnvifInfo_Click(object sender, EventArgs e)
        {
            Unsubscribe();
            IP = txtIP.Text;
            Port = (int)numPort.Value;

            tssLbl.Text = "Scanning device";
            btnGetOnvifInfo.Enabled = false;
            UseWaitCursor = true;

            ClearData();
            try
            {
                Subscribe(IP, Port);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Exception: {0}", ex.Message), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            try
            {
                GetOnvifInfo(IP, Port);
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
            DeviceClient client = OnvifServices.GetOnvifDeviceClient(ip, port); //, "service", "Sierra123")) // new DeviceClient(bind, serviceAddress))
            client.Endpoint.Behaviors.Add(new EndpointDiscoveryBehavior());

            gbxPtzControl.Visible = true;

            // We can now ask for information
            // ONVIF application programmer guide (5.1.3) suggests checking time first 
            // (no auth required) so time offset can be determined (needed for auth if applicable)
            client = OnvifServices.GetOnvifDeviceClient(IP.ToString(), Port);
            GetDeviceTime(client);

            // Switch to an authenticated client if the username field contains something
            if (txtUser.Text != string.Empty)
                client = OnvifServices.GetOnvifDeviceClient(IP.ToString(), Port, txtUser.Text, txtPassword.Text);

            GetDeviceInfo(client);
            GetServices(client);
            
            if (lbxCapabilities.Items.Contains("http://www.onvif.org/ver20/ptz/wsdl"))
            {
                gbxPtzControl.Enabled = true;
                GetPtzServices(ip, port);
                //PTZTest(client, ip, port);
            }
            else
            {
                gbxPtzControl.Enabled = false;
            }

            GetMediaInfo();
        }

        private void GetDeviceTime(DeviceClient client)
        {
            // Should compare recieved timestamp with local machine.  If out of sync, authentication may fail
            SystemDateTime dt = client.GetSystemDateAndTime();
            string date = string.Format("{0:0000}-{1:00}-{2:00}", dt.UTCDateTime.Date.Year, dt.UTCDateTime.Date.Month, dt.UTCDateTime.Date.Day);
            string time = string.Format("{0:00}:{1:00}:{2:00}", dt.UTCDateTime.Time.Hour, dt.UTCDateTime.Time.Minute, dt.UTCDateTime.Time.Second);
            lblDeviceTime.Text = string.Format("Device Time: {0} {1}", date, time);
            File.AppendAllText("info.txt", string.Format("\n\nDate and Time from: {0}:{1} [UTC Date: {2}, UTC Time: {3}]", IP.ToString(), Port, date, time));
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
                File.AppendAllText("info.txt", string.Format("\nDevice: {0} ({4}:{5}) [Firmware: {1}, Serial #: {2}, Hardware ID: {3}]\n", model, fwversion, serialno, hwid, IP.ToString(), Port));
            }
            catch (Exception ex) when (ex.InnerException.Message == "The remote server returned an error: (401) Unauthorized.")
            {
                // Authentication required
                tssLbl.Text = string.Format("{0} Authentication failure. Unable to log into device.", System.DateTime.Now); // - {0}", ex.Message);
                throw new Exception(string.Format("Authentication failure - {0}", ex.Message), ex);
            }
        }

        private void GetServices(DeviceClient client)
        {
            // GetCapabilities is now deprecated (as of v2.1) - replaced by GetServices (Older devices may still use)
            OnvifDeviceManagementServiceReference.Capabilities capabilities = client.GetCapabilities(new CapabilityCategory[] { CapabilityCategory.All });

            if (capabilities.Analytics != null) { lbxCapabilities.Items.Add("Analytics"); }
            if (capabilities.Events != null) { lbxCapabilities.Items.Add("Events"); }
            if (capabilities.Extension != null) { lbxCapabilities.Items.Add("Extension"); }
            if (capabilities.Imaging != null) { lbxCapabilities.Items.Add("Imaging"); }
            if (capabilities.Media != null) { lbxCapabilities.Items.Add("Media"); }
            if (capabilities.PTZ != null) { lbxCapabilities.Items.Add("PTZ"); }

            lbxCapabilities.Items.Add("");

            Service[] svc = client.GetServices(IncludeCapability: true);
            foreach (Service s in svc)
            {
                Console.WriteLine(s.XAddr + " " + " " + s.Namespace);  // Not present on Axis + s.Capabilities.NamespaceURI);
                lbxCapabilities.Items.Add(string.Format("{0}", s.Namespace));
                if (s.Capabilities != null)
                {
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
            }

            //DeviceServiceCapabilities dsc = client.GetServiceCapabilities();
        }

        private void GetMediaInfo()
        {
            lbxCapabilities.Items.Add("");
            lbxCapabilities.Items.Add("Media Info");
            OnvifMediaServiceReference.MediaClient mclient = OnvifServices.GetOnvifMediaClient(IP.ToString(), Port, txtUser.Text, txtPassword.Text);
            OnvifMediaServiceReference.VideoSource[] videoSources = mclient.GetVideoSources();
            foreach (OnvifMediaServiceReference.VideoSource v in videoSources)
            {
                string vsInfo = string.Format("  Video Source {0}: Framerate={1}, Resolution={2}x{3}", v.token, v.Framerate, v.Resolution.Width, v.Resolution.Height);
                lbxCapabilities.Items.Add(string.Format("{0}", vsInfo));
            }
            OnvifMediaServiceReference.Profile[] mProfiles = mclient.GetProfiles();
            foreach (OnvifMediaServiceReference.Profile p in mProfiles)
            {
                string pInfo = string.Format("  Profile {0}: Token={1}", p.Name, p.token);
                lbxCapabilities.Items.Add(string.Format("{0}", pInfo));
            }
        }

        private void GetPtzServices(string ip, int port)
        {
            PTZClient ptzService;
            OnvifMediaServiceReference.MediaClient mediaService;

            // Create PTZ and Media object
            if (txtUser.Text != string.Empty)
            {
                ptzService = OnvifServices.GetOnvifPTZClient(ip, port, txtUser.Text, txtPassword.Text);
                mediaService = OnvifServices.GetOnvifMediaClient(ip, port, txtUser.Text, txtPassword.Text);
            }
            else
            {
                ptzService = OnvifServices.GetOnvifPTZClient(ip, port);
                mediaService = OnvifServices.GetOnvifMediaClient(ip, port);
            }

            lbxPtzInfo.Items.Add("Supported Operations");
            foreach (OperationDescription odc in ptzService.Endpoint.Contract.Operations)
            {
                lbxPtzInfo.Items.Add("  " + odc.Name);
            }
            Console.WriteLine(ptzService);
        }

        private void PTZTest(DeviceClient client, string ip, int port)
        {
            // Create Media object
            OnvifMediaServiceReference.MediaClient mediaService = OnvifServices.GetOnvifMediaClient(ip, port);

            // Create PTZ object
            PTZClient ptzService = OnvifServices.GetOnvifPTZClient(ip, port); // new PTZClient(client.Endpoint.Binding, client.Endpoint.Address);

            // Get target profile
            OnvifMediaServiceReference.Profile[] mediaProfiles = mediaService.GetProfiles();
            string profileToken = mediaProfiles[0].token;
            OnvifMediaServiceReference.Profile mediaProfile = mediaService.GetProfile(profileToken);

            // Get Presets
            try
            {
                PTZPreset[] presets = ptzService.GetPresets(profileToken);
                lbxPtzInfo.Items.Add("");
                lbxPtzInfo.Items.Add("Presets");
                foreach (PTZPreset p in presets)
                {
                    lbxPtzInfo.Items.Add(string.Format("  Preset {0} ({1}) @ {2}:{3} {4}", p.Name, p.token, p.PTZPosition.PanTilt.x, p.PTZPosition.PanTilt.y, p.PTZPosition.Zoom.x));
                }

                UpdatePtzLocation(ptzService, profileToken);
            }
            catch (Exception ex)
            {
                tssLbl.Text = "Unable to get presets and update location: " + ex.Message;
                throw;
            }

            // Fails if not a PTZ
            OnvifPtzServiceReference.PTZNode node = ptzService.GetNode("1"); // nodes[0].token);

            PTZConfiguration[] ptzConfigs = ptzService.GetConfigurations();
            File.AppendAllText("ptz.txt", string.Format("\nPTZ configs found: {0}", ptzConfigs.Length));
            File.AppendAllText("ptz.txt", string.Format("\nPTZ config - Name: {0}", ptzConfigs[0].Name));
            File.AppendAllText("ptz.txt", string.Format("\nPTZ config - Token: {0}", ptzConfigs[0].token));

            try
            {
                // Add PTZ config to Media profile
                //mediaClient.AddPTZConfiguration("Profile1", ptzConfigs[0].token);

                try
                {
                    PTZSpeed velocity = new PTZSpeed();
                    File.AppendAllText("ptz.txt", string.Format("\nSetting velocity"));

                    //velocity.Zoom = new Vector1D() { x = ptzOptions.Spaces.ContinuousPanTiltVelocitySpace[0].XRange.Max, space = ptzOptions.Spaces.ContinuousZoomVelocitySpace[0].URI }; ;
                    velocity.PanTilt = new Vector2D() { x = (float)-0.5, y = 0 }; ;
                    //ptzService.GotoPreset(profileToken, presets[presets.Length - 1].token, velocity);

                    //ptz.GotoHomePosition(profileToken, velocity);
                    //ptzService.Stop(profileToken, true, false);
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

        private void UpdatePtzLocation(PTZClient ptzClient, string profileToken)
        {
            // Get Status
            PTZStatus status = ptzClient.GetStatus(profileToken);
            lblPtzLocationX.Text = "x: " + status.Position.PanTilt.x.ToString();
            lblPtzLocationY.Text = "y: " + status.Position.PanTilt.y.ToString();
            lblPtzLocationZoom.Text = "zoom: " + status.Position.Zoom.x.ToString();
        }

        private void UpdatePtzLocation(PTZStatus status)
        {
            lblPtzLocationX.Text = "x: " + status.Position.PanTilt.x.ToString();
            lblPtzLocationY.Text = "y: " + status.Position.PanTilt.y.ToString();
            lblPtzLocationZoom.Text = "zoom: " + status.Position.Zoom.x.ToString();
        }

        private void PtzStop()
        {
            OnvifPtz ptz = new OnvifPtz(IP, Port, txtUser.Text, txtPassword.Text);
            ptz.Stop();
        }

        private void btnSetConnectInfo_Click(object sender, EventArgs e)
        {
            IP = txtIP.Text;
            Port = (int)numPort.Value;

            // ODM Axis example
            //string password = "Sierra123";
            //string nonce = "h3dfca1Z/E+Wm15KYE78mgUAAAAAAA==";
            //string date = "2017-03-08T17:11:48.000Z";
            //string digest = "kkj/3C2oLKU57bzYCMKLAKjbheo=";
            string password = "userpassword";
            string nonce = "LKqI6G/AikKCQrN0zqZFlg==";
            string date = "2010-09-16T07:50:45Z";
            string digest = "tuOSpGlFlIXsozq4HFNeeGeFLEI=";

            //GetWsPasswordDigest("admin", password, nonce, date, digest);
        }

        public void GetWsPasswordDigest(string user, string password, string nonce, string timestamp, string digest = "")
        {
            var nonceDecodeBinary = Convert.FromBase64String(nonce);
            byte[] dateBinary = Encoding.UTF8.GetBytes(timestamp);
            byte[] passwordBinary = Encoding.UTF8.GetBytes(password);
            Console.WriteLine(string.Format("Nonce decoded from B64 -> Hex: {0} ", BitConverter.ToString(nonceDecodeBinary)));

            byte[] concatData = new byte[nonceDecodeBinary.Length + dateBinary.Length + passwordBinary.Length];
            Buffer.BlockCopy(nonceDecodeBinary, 0, concatData, 0, nonceDecodeBinary.Length);
            Buffer.BlockCopy(dateBinary, 0, concatData, nonceDecodeBinary.Length, dateBinary.Length);
            Buffer.BlockCopy(passwordBinary, 0, concatData, nonceDecodeBinary.Length + dateBinary.Length, passwordBinary.Length);

            System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
            string computedDigest = Convert.ToBase64String(sha1.ComputeHash(concatData));
            Console.WriteLine(string.Format("Current Hash:\t{0}\nOriginal Hash:\t{1}", computedDigest, digest));

            if (digest != "")
            {
                if (computedDigest == digest)
                    MessageBox.Show("Hash match" + txtPassword.Text);
                else
                    MessageBox.Show(string.Format("Hash mismatch\nActual\t{0}\nCalc\t{1}", digest, computedDigest));
            }
        }

        private void ClearData()
        {
            lbxCapabilities.Items.Clear();
            lbxPtzInfo.Items.Clear();

            lblFirmware.Text = "Firmware:";
            lblModel.Text = "Model:";
            lblSerial.Text = "Serial #:";
            lblHwID.Text = "Hardware ID:";
            lblDeviceTime.Text = "Time:";

            lblPtzLocationX.Text = "Location (x):";
            lblPtzLocationY.Text = "Location (y):";
            lblPtzLocationZoom.Text = "Location (zoom):";
        }

        //PTZ Move commands
        private void BtnPanLeft_MouseDown(object sender, MouseEventArgs e)
        {
            float speed = (float)numPtzCmdSpeed.Value / 100;
            OnvifPtz ptz = new OnvifPtz(IP, Port, txtUser.Text, txtPassword.Text);
            ptz.Pan(-speed);
            UpdatePtzLocation(ptz.GetPtzLocation());
        }

        private void BtnPanRight_MouseDown(object sender, MouseEventArgs e)
        {
            float speed = (float)numPtzCmdSpeed.Value / 100;
            OnvifPtz ptz = new OnvifPtz(IP, Port, txtUser.Text, txtPassword.Text);
            ptz.Pan(speed);
            UpdatePtzLocation(ptz.GetPtzLocation());
        }

        private void BtnTiltUp_MouseDown(object sender, MouseEventArgs e)
        {
            float speed = (float)numPtzCmdSpeed.Value / 100;
            OnvifPtz ptz = new OnvifPtz(IP, Port, txtUser.Text, txtPassword.Text);
            ptz.Tilt(speed);
            UpdatePtzLocation(ptz.GetPtzLocation());
        }

        private void BtnTiltDown_MouseDown(object sender, MouseEventArgs e)
        {
            float speed = (float)numPtzCmdSpeed.Value / 100;
            OnvifPtz ptz = new OnvifPtz(IP, Port, txtUser.Text, txtPassword.Text);
            ptz.Tilt(-speed);
            UpdatePtzLocation(ptz.GetPtzLocation());
        }

        private void BtnZoomOut_MouseDown(object sender, MouseEventArgs e)
        {
            float speed = (float)numPtzCmdSpeed.Value / 100;
            OnvifPtz ptz = new OnvifPtz(IP, Port, txtUser.Text, txtPassword.Text);
            ptz.Zoom(-speed);
            UpdatePtzLocation(ptz.GetPtzLocation());
        }

        private void BtnZoomIn_MouseDown(object sender, MouseEventArgs e)
        {
            float speed = (float)numPtzCmdSpeed.Value / 100;
            OnvifPtz ptz = new OnvifPtz(IP, Port, txtUser.Text, txtPassword.Text);
            ptz.Zoom(speed);
            UpdatePtzLocation(ptz.GetPtzLocation());
        }

        private void BtnPreset_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            OnvifPtz ptz = new OnvifPtz(IP, Port, txtUser.Text, txtPassword.Text);
            try
            {
                ptz.ShowPreset(Convert.ToInt32(btn.Text));
                Console.WriteLine(string.Format("Moving to preset {0}", btn.Text));
                UpdatePtzLocation(ptz.GetPtzLocation());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        // Ptz stop commands
        private void BtnTilt_MouseUp(object sender, MouseEventArgs e)
        {
            PtzStop();
        }

        private void BtnPan_MouseUp(object sender, MouseEventArgs e)
        {
            PtzStop();
        }

        private void BtnZoom_MouseUp(object sender, MouseEventArgs e)
        {
            PtzStop();
        }

        private void ChkShowPwd_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            txtPassword.PasswordChar = chk.Checked ? '\0' : '*';
        }

        private void Form1_Closing(object sender, FormClosedEventArgs e)
        {
            Unsubscribe();
        }
    }
}
