using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace OnvifEvents
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            HttpServer();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static async void HttpServer()
        {
            HttpListener http = GetHttpListener(8080);
            http.Start();

            while (true)
            {
                HttpListenerContext httpRequest = await http.GetContextAsync();
                HttpListenerRequest request = httpRequest.Request;

                Console.WriteLine(httpRequest.Request.InputStream.ToString());



                if (httpRequest.Request.InputStream.CanRead)
                {
                    //byte[] input = new byte[] { };
                    //httpRequest.Request.InputStream.Read(input, 0, Convert.ToInt32(httpRequest.Request.InputStream.Length));

                    using (System.IO.Stream body = request.InputStream)
                    {

                        //XmlDocument doc = new XmlDocument();
                        //doc.Load(body);


                        using (System.IO.StreamReader reader = new StreamReader(body, request.ContentEncoding))
                        {
                            string xml = reader.ReadToEnd();
                            //Console.WriteLine(xml);
                            
                            XDocument xDoc = XDocument.Load(new StringReader(xml));
                            XNamespace onvifEvent = XNamespace.Get("http://docs.oasis-open.org/wsn/b-2");

                            //var unwrappedResponse = xDoc.Descendants((XNamespace)"http://www.w3.org/2003/05/soap-envelope" + "Body")
                            //    .First()
                            //    .FirstNode;

                            var unwrappedResponse = xDoc.Descendants((XNamespace)"http://docs.oasis-open.org/wsn/b-2" + "Message")
                                .First()
                                .FirstNode;
                            Console.WriteLine(unwrappedResponse + "\n");

                            var items = xDoc.Descendants(onvifEvent + "Message").Elements();
                            foreach (var item in items)
                            {
                                Console.WriteLine(item.FirstAttribute.Name + " " + item.FirstAttribute.Value);
                                Console.WriteLine(item.LastAttribute.Name + " " + item.LastAttribute.Value);
                            }


                            onvifEvent = XNamespace.Get("http://www.onvif.org/ver10/schema");
                            items = xDoc.Descendants(onvifEvent + "Source").Elements();
                            foreach (var item in items)
                            {
                                Console.WriteLine(item.FirstAttribute.Value + " = " + item.LastAttribute.Value);
                            }

                            items = xDoc.Descendants(onvifEvent + "Data").Elements();
                            foreach (var item in items)
                            {
                                Console.WriteLine(item.FirstAttribute.Value + " = " + item.LastAttribute.Value);
                            }

                        }
                    }
                }

                // Send response
                HttpListenerResponse response = httpRequest.Response;
                System.IO.Stream output = response.OutputStream;
                const string responseString = "Ack"; //<html><body>Hello world</body></html>";
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                output.Write(buffer, 0, buffer.Length);
                //Console.WriteLine(output);
                output.Close();
            }
        }

        public static HttpListener GetHttpListener(int port)
        {
            HttpListener http = new HttpListener();
            http.Prefixes.Add(string.Format("http://*:{0}/subscription-1/", port));

            return http;
        }
    }
}
