using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using OnvifEvents.OnvifEventServiceReference;
using System.Xml;


namespace OnvifEvents
{
    public partial class Form1 : Form
    {
        DateTime? SubTermTime;
        Timer SubRenewTimer = new Timer();
        string SubRenewUri;
        SubscriptionManagerClient SubscriptionManagerClient;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string ip = "172.16.5.12";
            int port = 80;

            EventPortTypeClient eptc = OnvifServices.GetEventClient(ip, port);
            
            // 1. Get Event Properties
            bool fixedTopicSet;
            TopicSetType tst = new TopicSetType();
            string[] topicExpDialect;
            string[] msgContentFilter;
            string[] producerProperties;
            string[] msgContentSchema;
            XmlElement[] any;

            string[] result = eptc.GetEventProperties(out fixedTopicSet, out tst, out topicExpDialect, out msgContentFilter, out producerProperties, out msgContentSchema, out any);

            listBox1.Items.Add(string.Format("Result: {0}", result));
            listBox1.Items.Add(string.Format("Message Content Filter Dialect(s)"));
            foreach (string msg in msgContentFilter)
                listBox1.Items.Add(string.Format("  {0}", msg));

            listBox1.Items.Add("");
            listBox1.Items.Add(string.Format("Message Content Schema Location(s)"));
            foreach (string msg in msgContentSchema)
                listBox1.Items.Add(string.Format("  {0}", msg));

            listBox1.Items.Add("");
            listBox1.Items.Add(string.Format("Producer Properties Filter Dialect(s)"));
            foreach (string msg in producerProperties)
                listBox1.Items.Add(string.Format("  {0}", msg));

            listBox1.Items.Add("");
            listBox1.Items.Add(string.Format("Topic Expression Dialect(s)"));
            foreach (string msg in topicExpDialect)
                listBox1.Items.Add(string.Format("  {0}", msg));

            listBox1.Items.Add("");
            listBox1.Items.Add(string.Format("Topic Set item(s)"));
            foreach (XmlElement x in tst.Any)
                listBox1.Items.Add("  " + x.Name);

            Capabilities c = eptc.GetServiceCapabilities();

            listBox1.Items.Add("");
            listBox1.Items.Add("Capabilites");
            listBox1.Items.Add(string.Format("  MaxNotificationProducers: {0}", c.MaxNotificationProducers));
            listBox1.Items.Add(string.Format("  MaxNotificationProducersSpecified: {0}", c.MaxNotificationProducersSpecified));
            listBox1.Items.Add(string.Format("  MaxPullPoints: {0}", c.MaxPullPoints));
            listBox1.Items.Add(string.Format("  MaxPullPointsSpecified: {0}", c.MaxPullPointsSpecified));
            listBox1.Items.Add(string.Format("  PersistentNotificationStorage: {0}", c.PersistentNotificationStorage));
            listBox1.Items.Add(string.Format("  PersistentNotificationStorageSpecified: {0}", c.PersistentNotificationStorageSpecified));
            listBox1.Items.Add(string.Format("  WSPausableSubscriptionManagerInterfaceSupport: {0}", c.WSPausableSubscriptionManagerInterfaceSupport));
            listBox1.Items.Add(string.Format("  WSPausableSubscriptionManagerInterfaceSupportSpecified: {0}", c.WSPausableSubscriptionManagerInterfaceSupportSpecified));
            listBox1.Items.Add(string.Format("  WSPullPointSupport: {0}", c.WSPullPointSupport));
            listBox1.Items.Add(string.Format("  WSPullPointSupportSpecified: {0}", c.WSPullPointSupportSpecified));
            listBox1.Items.Add(string.Format("  WSSubscriptionPolicySupport: {0}", c.WSSubscriptionPolicySupport));
            listBox1.Items.Add(string.Format("  WSSubscriptionPolicySupportSpecified: {0}", c.WSSubscriptionPolicySupportSpecified));

            //pull(ip, port);
            subscribe(ip, port);
            //test(ip, port);
        }

        public void subscribe(string ip, int port)
        {
            EventPortTypeClient eptc = OnvifServices.GetEventClient(ip, port);

            string localIP = GetLocalIp(); // "172.16.5.111";

            //// Consumer client (for receiving notifications?) - not working
            //NotificationConsumerClient ncc = OnvifServices.GetNotificationConsumerClient(localIP, 8080, "subscription-1");
            //Console.WriteLine(string.Format("Consumer Client state: {0}", ncc.State));
            //ncc.Open();
            //Console.WriteLine(string.Format("Consumer Client state: {0}", ncc.State));
            //Notify n = new Notify();
            //n.PropertyChanged += N_PropertyChanged;
            //ncc.Notify(n);
            
            // Producer client
            NotificationProducerClient npc = OnvifServices.GetNotificationProducerClient(ip, port);
            npc.Endpoint.Address = eptc.Endpoint.Address;

            Subscribe s = new Subscribe();
            // Consumer reference tells the device where to Post messages back to (the client)
            EndpointReferenceType clientEndpoint = new EndpointReferenceType() { Address = new AttributedURIType() { Value = string.Format("http://{0}:8080/subscription-1", localIP) } };
            s.ConsumerReference = clientEndpoint;
            s.InitialTerminationTime = "PT60S";

            SubscribeResponse sr = npc.Subscribe(s);

            // Store the subscription URI for use in Renew
            SubRenewUri = sr.SubscriptionReference.Address.Value;

            // Start timer to periodically check if a Renew request needs to be issued
            SubTermTime = sr.TerminationTime;
            SubRenewTimer.Start();
            SubRenewTimer.Interval = 1000;
            SubRenewTimer.Tick += SubRenewTimer_Tick;

            SubscriptionManagerClient = OnvifServices.GetSubscriptionManagerClient(SubRenewUri); // oAux1.Address.Value);
            //Renew();
        }

        /// <summary>
        /// Issues a subscription renew message ~10 seconds before the subscription's scheduled termination time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubRenewTimer_Tick(object sender, EventArgs e)
        {
            var timeDiff = SubTermTime - DateTime.UtcNow;
            var x = timeDiff.Value.Ticks / TimeSpan.TicksPerSecond;
            if (x < 10)
            {
                // Send renew
                Renew();
            }
        }

        public void Renew()
        {
            Console.WriteLine(DateTime.Now + "\tIssue subscription renew");
            RenewResponse oRenewResult = SubscriptionManagerClient.Renew(new Renew() { TerminationTime = "PT60S" });
            SubTermTime = oRenewResult.TerminationTime;
            Console.WriteLine(string.Format("Current Time: {0}\tTermination Time: {1}", oRenewResult.CurrentTime.ToString(), oRenewResult.TerminationTime.Value.ToString()));
        }

        public void pull(string ip, int port)
        {
            EventPortTypeClient eptc = OnvifServices.GetEventClient(ip, port);

            // 2. CreatePullPointSubscription
            FilterType filter = new FilterType();
            DateTime currentTime;
            XmlElement[] xml = null;
            DateTime? termTime;
            List<MessageHeader> lstHeaders = new List<MessageHeader>() { };

            EndpointReferenceType ert = eptc.CreatePullPointSubscription(
                filter, 
                "PT15S", 
                new CreatePullPointSubscriptionSubscriptionPolicy(), 
                ref xml, 
                out currentTime, 
                out termTime
                );
            listBox1.Items.Add(ert.Address.Value);

            if ((ert.ReferenceParameters != null) && (ert.ReferenceParameters.Any != null))
            {
                foreach (System.Xml.XmlElement oXml in ert.ReferenceParameters.Any)
                {
                    string strName = oXml.LocalName;
                    string strNS = oXml.NamespaceURI;
                    string strValue = oXml.InnerXml;

                    lstHeaders.Add(MessageHeader.CreateHeader(strName, strNS, strValue, true));
                }
            }

            //PullPointSubscriptionClient ppsc = OnvifServices.GetPullPointSubClient(ip, port);
            //NotificationMessageHolderType[] nmht;
            //DateTime terminationTime;
            //XmlDocument doc = new XmlDocument();
            //doc.CreateElement("test");
            //XmlElement[] pullMsgXml = { doc.CreateElement("test") };

            //ppsc.PullMessages("PT5S", 5, any, out terminationTime, out nmht);

        }

        public void test(string ip, int port)
        {
            XmlElement[] Any = new XmlElement[] { };
            DateTime CurrentTime;
            DateTime? TerminationTime;
            List<MessageHeader> lstHeaders = new List<MessageHeader>() { };
            var oEventClient = OnvifServices.GetEventClient(ip, port);

            var oAux1 = oEventClient.CreatePullPointSubscription(
                new FilterType(),
                "PT600S",
                new CreatePullPointSubscriptionSubscriptionPolicy(),
                ref Any,
                out CurrentTime,
                out TerminationTime);

            if ((oAux1.ReferenceParameters != null) && (oAux1.ReferenceParameters.Any != null))
            {
                foreach (XmlElement oXml in oAux1.ReferenceParameters.Any)
                {
                    string strName = oXml.LocalName;
                    string strNS = oXml.NamespaceURI;
                    string strValue = oXml.InnerXml;

                    lstHeaders.Add(MessageHeader.CreateHeader(strName, strNS, strValue, true));
                }
            }
            
            //HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
            //requestMessage.Headers.Add("test", "value");
            //lstHeaders.Add(MessageHeader.CreateHeader("test", "something", "value"));

            // oAux1.Address.Value -> the proxy endpoint address
            // lstHeaders -> headers to add to the SOAP message of the proxy request
            //var oPullPointSubscriptionClient = OnvifServices.GetPullPointSubClient(oAux1.Address.Value, 80, lstHeaders);
            var oPullPointSubscriptionClient = OnvifServices.GetPullPointSubClient(ip, 80, oAux1.Address.Value, lstHeaders);
            var oSubscriptionManagerClient = OnvifServices.GetSubscriptionManagerClient(oAux1.Address.Value); //, 80, lstHeaders);

            //do
            //{
                NotificationMessageHolderType[] NotificationMessages;
            try
            {
                oPullPointSubscriptionClient.PullMessages("PT60S", 1024, Any, out CurrentTime, out NotificationMessages);

                listBox1.Items.Add("");
                listBox1.Items.Add("Events");
                foreach (NotificationMessageHolderType message in NotificationMessages)
                {
                    foreach (XmlAttribute x in message.Message.Attributes)
                    {
                        Console.WriteLine(string.Format("{0} = {1}", x.Name, x.Value));
                    }
                    
                    string time = message.Message.Attributes[0].Value;
                    string topic = message.Topic.Any[0].Value;
                    string operation = message.Message.Attributes[1].Value;
                    string source = message.Message.ChildNodes[0].Value;
                    string key;
                    string data = message.Message.ChildNodes[1].Value;
                    listBox1.Items.Add(string.Format("  {0}\t{1}\t{2}\t{3}\t{4}", time, topic, operation, source, data));

                }

                //var oRenewResult = oSubscriptionManagerClient.Renew(new LIB.Cameras.ONVIF_WS.onvif10_events.Renew() { TerminationTime = "PT600S" });
                var oRenewResult = oSubscriptionManagerClient.Renew(new Renew() { TerminationTime = "PT60S" });
            }
            catch (Exception ex)
            {
                listBox1.Items.Add("------");
                listBox1.Items.Add(string.Format("Exception: {0}", ex.Message));
            }

            //} while (_session != null);

            //oSubscriptionManagerClient.Unsubscribe(new Unsubscribe());
        }

        public string GetLocalIp()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    return localIP;
                }
            }

            throw new Exception("Local IP address not found");
        }
    }
}
