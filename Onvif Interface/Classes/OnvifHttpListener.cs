using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SDS.Video.Onvif
{
    public class OnvifHttpListener
    {
        private HttpListener http;
        private bool RunServer = false;
        public bool IsListening { get; private set; } =  false;

        public event EventHandler Notification;

        public void OnNotification(List<string> notifyMessages)
        {
            HttpNotificationEventArgs e = new HttpNotificationEventArgs(notifyMessages);
            Notification?.Invoke(this, e);
        }

        /// <summary>
        /// Start a persistent HttpListener on the provided port
        /// </summary>
        /// <param name="port">TCP Port to listen to</param>
        public async void StartHttpServer(int port)
        {
            http = GetHttpListener(port);
            Console.WriteLine(string.Format("\nStarting Http Listener on port {0}\n", port));
            http.Start();
            IsListening = http.IsListening;

            RunServer = true;
            while (RunServer)
            {
                // Check if still listening
                if (!http.IsListening)
                {
                    IsListening = http.IsListening;
                    Console.WriteLine("\nHttp Listener no longer listening.  Restarting...\n");
                    http.Start();
                    System.Threading.Thread.Sleep(500);
                    IsListening = http.IsListening;
                }

                try
                {
                    HttpListenerContext httpRequest = await http.GetContextAsync();
                    Console.WriteLine(string.Format("{0} Received request", DateTime.Now.ToString("hh.mm.ss.ffffff")));
                    ProcessRequest(httpRequest);
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("{0} Exception: {1}", DateTime.Now.ToString("hh.mm.ss.ffffff"), e.Message));
                }
            }
        }

        /// <summary>
        /// Shut down the HttpListener
        /// </summary>
        public void StopHttpServer()
        {
            Console.WriteLine("\nStopping Http Listener\n");
            RunServer = false;
            http.Close();
        }

        private async void ProcessRequest(HttpListenerContext httpRequest)
        {
            Console.WriteLine(string.Format("{0} Processing request", DateTime.Now.ToString("hh.mm.ss.ffffff")));
            HttpListenerRequest request = httpRequest.Request;

            if (httpRequest.Request.InputStream.CanRead)
            {
                using (Stream body = request.InputStream)
                {
                    using (StreamReader reader = new StreamReader(body, request.ContentEncoding))
                    {
                        Console.WriteLine(string.Format("{0} Reading XML", DateTime.Now.ToString("hh.mm.ss.ffffff")));

                        // Use task to get xml to avoid locking up
                        try
                        {
                            Task<string> task = Task.Run(() => GetXml(reader));
                            string xml = await task;
                            //Console.WriteLine(xml);
                            Console.WriteLine(string.Format("{0} XML read complete", DateTime.Now.ToString("hh.mm.ss.ffffff")));

                            if (xml != null && xml != string.Empty)
                            {
                                XDocument xDoc = XDocument.Load(new StringReader(xml));
                                Console.WriteLine(string.Format("{0} xDoc loaded", DateTime.Now.ToString("hh.mm.ss.ffffff")));

                                XNamespace onvifEvent = XNamespace.Get("http://docs.oasis-open.org/wsn/b-2");

                                //ScanElements(xDoc.Elements());

                                //var notificationMsg = xDoc.Descendants((XNamespace)"http://docs.oasis-open.org/wsn/b-2" + "Notify")
                                //    .First()
                                //    .FirstNode;
                                //Console.WriteLine(notificationMsg + "\n");

                                var notifications = xDoc.Descendants(onvifEvent + "Notify").Elements();
                                ParseNotifications(notifications);
                            }
                            else
                            {
                                Debug.Print("Null or empty Xml string received - don't process");
                                return;
                            }
                        }
                        catch (HttpListenerException e)
                        {
                            Debug.Print(e.Message);
                            return;
                        }
                    }
                }
            }

            Console.WriteLine(string.Format("{0} Sending response", DateTime.Now.ToString("hh.mm.ss.ffffff")));

            // Send response
            HttpListenerResponse response = httpRequest.Response;
            response.StatusCode = 202;  // 202 - Accepted
            Stream output = response.OutputStream;
            const string responseString = "";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            Console.WriteLine(string.Format("{0} Done processing", DateTime.Now.ToString("hh.mm.ss.ffffff")));
        }

        /// <summary>
        /// Parse out notifications and pass a list of them to an event
        /// </summary>
        /// <param name="notifications"></param>
        private void ParseNotifications(IEnumerable<XElement> notifications)
        {
            XNamespace onvifEvent = XNamespace.Get("http://docs.oasis-open.org/wsn/b-2");
            XNamespace onvifEventDetail = XNamespace.Get("http://www.onvif.org/ver10/schema");
            //XNamespace onvifEventTopic = XNamespace.Get("http://www.onvif.org/ver10/topics");
            List<string> notifyMessages = new List<string>();

            foreach (var n in notifications)
            {
                string notifyXml = n.ToString();
                XDocument xDoc = XDocument.Load(new StringReader(notifyXml));

                // Topic
                List<XElement> topicList = xDoc.Descendants(onvifEvent + "Topic").ToList();
                string topic = topicList.ElementAt(0).Value;

                // Message
                var message = xDoc.Descendants(onvifEvent + "Message").Elements();
                IEnumerable<XAttribute> msgAttr = message.Attributes();
                var time = msgAttr.ElementAt(0).Value;
                //var time = message.Attributes("UtcTime").ToString();
                var operation = msgAttr.ElementAt(1).Value;// message.Attributes("PropertyOperation");

                // Source
                var source = xDoc.Descendants(onvifEventDetail + "Source").Elements();
                IEnumerable<XAttribute> srcAttr = source.Attributes();
                var srcName = srcAttr.ElementAt(0).Value;
                var srcValue = srcAttr.ElementAt(1).Value;

                // Data
                var data = xDoc.Descendants(onvifEventDetail + "Data").Elements();
                IEnumerable<XAttribute> dataAttr = data.Attributes();
                var dataName = dataAttr.ElementAt(0).Value;
                var dataValue = dataAttr.ElementAt(1).Value;

                //Console.WriteLine(string.Format("Event Info - Time: {0}, Operation: {1}, {2} = {3}, {4} = {5}", time, operation, srcName, srcValue, dataName, dataValue));
                notifyMessages.Add(string.Format("Time: {1}, Topic: {0}, Operation: {2}, {3} = {4}, {5} = {6}", topic, time, operation, srcName, srcValue, dataName, dataValue));
            }

            OnNotification(notifyMessages);
        }

        private static string GetXml(StreamReader reader)
        {
            try
            {
                string xml = reader.ReadToEnd();  // Slow operation
                return xml;
            }
            catch (HttpListenerException e)
            {
                Debug.Print(e.Message);
                return string.Empty;
            }
        }

        public static HttpListener GetHttpListener(int port)
        {
            HttpListener http = new HttpListener();
            http.Prefixes.Add(string.Format("http://*:{0}/subscription-1/", port));

            return http;
        }

        private void ScanElements(IEnumerable<XElement> elements)
        {
            foreach (XElement x in elements.Elements())
            {
                Console.WriteLine(x.Name.LocalName + " " + x.Value);
                
                if (x.HasElements)
                {
                    ScanElements(x.Elements());
                }
            }
        }
    }

    /// <summary>
    /// EventArgs based class for returning Onvif notifications from HttpListener
    /// </summary>
    public class HttpNotificationEventArgs : EventArgs
    {
        public List<string> Notifications;

        public HttpNotificationEventArgs(List<string> notifications)
        {
            Notifications = notifications;
        }
    }
}
