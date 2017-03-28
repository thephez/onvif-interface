using OnvifEvents.OnvifEventServiceReference;
using SimpleWebServer;
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
            //WebServer ws = new WebServer(SendResponse, GetOnvifHttpPrefix(8080)); // "http://localhost:8080/test/");
            //ws.Run();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        //public static string SendResponse(HttpListenerRequest request)
        //{
        //    return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
        //}

        public static string GetOnvifHttpPrefix(int port)
        {
            return string.Format("http://*:{0}/subscription-1/", port);
        }
    }
}
