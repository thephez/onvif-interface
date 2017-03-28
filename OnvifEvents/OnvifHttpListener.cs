using Microsoft.Web.Services3.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OnvifEvents
{
    public class OnvifHttpListener
    {
        public event EventHandler Notification;

        public void OnNotification(List<string> notifyMessages)
        {
            NotificationEventArgs e = new NotificationEventArgs(notifyMessages);
            Notification?.Invoke(this, e);
        }

        public async void StartHttpServer(int port)
        {
            HttpListener http = GetHttpListener(port);
            Console.WriteLine(string.Format("\nStarting Http Listener on port {0}\n", port));
            http.Start();

            while (true)
            {
                // Check if still listening
                if (!http.IsListening)
                {
                    Console.WriteLine("\nHttp Listener no longer listening.  Restarting...\n");
                    http.Start();
                    System.Threading.Thread.Sleep(500);
                }

                HttpListenerContext httpRequest = await http.GetContextAsync();
                Console.WriteLine(string.Format("{0} Received request", DateTime.Now.ToString("hh.mm.ss.ffffff")));
                ProcessRequest(httpRequest);
            }
        }

        private async void ProcessRequest(HttpListenerContext httpRequest)
        {
            Console.WriteLine(string.Format("{0} Processing request", DateTime.Now.ToString("hh.mm.ss.ffffff")));
            HttpListenerRequest request = httpRequest.Request;

            Console.WriteLine(DateTime.Now.ToString("hh.mm.ss.ffffff") + " " + httpRequest.Request.InputStream.ToString());

            if (httpRequest.Request.InputStream.CanRead)
            {
                using (Stream body = request.InputStream)
                {
                    //XmlDocument doc = new XmlDocument();
                    //doc.Load(body);

                    using (StreamReader reader = new StreamReader(body, request.ContentEncoding))
                    {
                        Console.WriteLine(string.Format("{0} Reading XML", DateTime.Now.ToString("hh.mm.ss.ffffff")));

                        // Use task to get xml to avoid locking up
                        Task<string> task = Task.Run(() => GetXml(reader));
                        string xml = await task;
                        //Console.WriteLine(xml);
                        Console.WriteLine(string.Format("{0} XML read complete", DateTime.Now.ToString("hh.mm.ss.ffffff")));

                        XDocument xDoc = XDocument.Load(new StringReader(xml));
                        Console.WriteLine(string.Format("{0} xDoc loaded", DateTime.Now.ToString("hh.mm.ss.ffffff")));

                        XNamespace onvifEvent = XNamespace.Get("http://docs.oasis-open.org/wsn/b-2");

                        ScanElements(xDoc.Elements());
                        

                        //var unwrappedResponse = xDoc.Descendants((XNamespace)"http://www.w3.org/2003/05/soap-envelope" + "Body")
                        //    .First()
                        //    .FirstNode;

                        //var notificationMsg = xDoc.Descendants((XNamespace)"http://docs.oasis-open.org/wsn/b-2" + "Notify")
                        //    .First()
                        //    .FirstNode;
                        //Console.WriteLine(notificationMsg + "\n");

                        var notifications = xDoc.Descendants(onvifEvent + "Notify").Elements();
                        ParseNotifications(notifications);

                        XmlDocument x = new XmlDocument();
                        x.LoadXml(xml);

                        //foreach (var item in notifications)
                        //{
                        //    Console.WriteLine(item.FirstAttribute.Name + " " + item.FirstAttribute.Value);
                        //    Console.WriteLine(item.LastAttribute.Name + " " + item.LastAttribute.Value);
                        //}

                        //var items = xDoc.Descendants(onvifEvent + "Message").Elements();
                        //foreach (var item in items)
                        //{
                        //    Console.WriteLine(item.FirstAttribute.Name + " " + item.FirstAttribute.Value);
                        //    Console.WriteLine(item.LastAttribute.Name + " " + item.LastAttribute.Value);
                        //}

                        //onvifEvent = XNamespace.Get("http://www.onvif.org/ver10/schema");
                        //items = xDoc.Descendants(onvifEvent + "Source").Elements();
                        //foreach (var item in items)
                        //{
                        //    Console.WriteLine(item.FirstAttribute.Value + " = " + item.LastAttribute.Value);
                        //}
                    }
                }
            }

            Console.WriteLine(string.Format("{0} Sending response", DateTime.Now.ToString("hh.mm.ss.ffffff")));

            // Send response
            HttpListenerResponse response = httpRequest.Response;
            response.StatusCode = 202;  // 202 - Accepted
            Stream output = response.OutputStream;
            const string responseString = ""; //<html><body>Hello world</body></html>";
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            output.Write(buffer, 0, buffer.Length);
            //Console.WriteLine(output);
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
                notifyMessages.Add(string.Format("Event Info - Time: {1}, Topic: {0}, Operation: {2}, {3} = {4}, {5} = {6}", topic, time, operation, srcName, srcValue, dataName, dataValue));
            }

            OnNotification(notifyMessages);
        }

        private static string GetXml(StreamReader reader)
        {
            string xml = reader.ReadToEnd();  // Slow operation
            return xml;
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

    public class NotificationEventArgs : EventArgs
    {
        public List<string> Notifications;

        public NotificationEventArgs(List<string> notifications)
        {
            Notifications = notifications;
        }
    }
}
