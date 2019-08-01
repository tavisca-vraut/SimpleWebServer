using System;
using System.Net;

namespace WebServer
{
    class Program
    {
        private static void Main(string[] _)
        {
            var webServer = new WebServer($"http://localhost:{WebServer.PortNumber}/");
            webServer.Run();

            Console.ReadKey();
            webServer.StopListening();
        }
    }
}
