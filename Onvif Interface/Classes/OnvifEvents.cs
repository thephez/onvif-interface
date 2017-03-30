using Onvif_Interface.OnvifEventServiceReference;
using System;
using System.Net;
using System.ServiceModel;
using System.Timers;

namespace SDS.Video.Onvif
{
    class OnvifEvents
    {
        private System.DateTime? SubTermTime;
        private Timer SubRenewTimer = new Timer();
        private string SubRenewUri;
        private SubscriptionManagerClient SubscriptionManagerClient;

        public event EventHandler Notification;
        private void OnNotification(string notification)
        {
            OnvifEventsStatusArgs e = new OnvifEventsStatusArgs(notification);
            Notification?.Invoke(this, e);
        }

        /// <summary>
        /// Subscribes to all events provided by the device and directs to :8080/subscription-1 Http listener
        /// </summary>
        /// <param name="ip">IP Address of camera</param>
        /// <param name="port">TCP port of camera</param>
        /// <param name="username">User</param>
        /// <param name="password">Password</param>
        public void Subscribe(string ip, int port, string username, string password)
        {
            EventPortTypeClient eptc = OnvifServices.GetEventClient(ip, port, username, password);

            string localIP = GetLocalIp();

            // Producer client
            NotificationProducerClient npc = OnvifServices.GetNotificationProducerClient(ip, port, username, password);
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
                SubRenewTimer.Elapsed += SubRenewTimer_Elapsed;

                OnNotification(string.Format("Initial Termination Time: {0} (Current Time: {1})", SubTermTime, System.DateTime.UtcNow));

                SubscriptionManagerClient = OnvifServices.GetSubscriptionManagerClient(SubRenewUri, username, password);
            }
            catch (Exception e)
            {
                OnNotification(string.Format("{0} Unable to subscribe to events on device [{1}]", System.DateTime.UtcNow, ip));
            }
        }

        /// <summary>
        /// Issues a subscription renew message ~10 seconds before the subscription's scheduled termination time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubRenewTimer_Elapsed(object sender, ElapsedEventArgs e)
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
        private void Renew()
        {
            Console.WriteLine(System.DateTime.Now + "\tIssue subscription renew");
            RenewResponse oRenewResult = SubscriptionManagerClient.Renew(new Renew() { TerminationTime = "PT60S" });
            SubTermTime = oRenewResult.TerminationTime;
            Console.WriteLine(string.Format("Current Time: {0}\tTermination Time: {1}", oRenewResult.CurrentTime.ToString(), oRenewResult.TerminationTime.Value.ToString()));
            //OnNotification(string.Format("Subscription renewed - Current Time: {0}\tTermination Time: {1}", oRenewResult.CurrentTime.ToString(), oRenewResult.TerminationTime.Value.ToString()));
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

                OnNotification(string.Format("Subscription canceled - Current Time: {0}", System.DateTime.UtcNow));
            }
            else
            {
                OnNotification(string.Format("No subscription to cancel - Current Time: {0}", System.DateTime.UtcNow));
            }
        }

        private string GetLocalIp()
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
    }

    /// <summary>
    /// EventArgs based class for returning status messages
    /// </summary>
    public class OnvifEventsStatusArgs : EventArgs
    {
        public string Message { get; }

        public OnvifEventsStatusArgs(string notifications)
        {
            Message = notifications;
        }
    }
}
