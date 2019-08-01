using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace WebServer
{
    public class WebServer
    {
        private readonly HttpListener _httpListener = new HttpListener();
        private readonly string FolderToServe = @"C:\TestServer";
        public const int PortNumber = 8000;

        public WebServer(string prefix)
        {
            _httpListener.Prefixes.Add(prefix);
            try
            {
                _httpListener.Start();
                Console.WriteLine($"Now serving at port {PortNumber}...");
            }
            catch(HttpListenerException httpListenerException)
            {
                Console.WriteLine(httpListenerException.ToString());
                Console.WriteLine(Environment.NewLine + "Server failed to start..");
            }
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem(KeepServerListeningAsynchrounously);
        }

        private void KeepServerListeningAsynchrounously(object _)
        {
            while (_httpListener.IsListening)
            {
                ThreadPool.QueueUserWorkItem(ProcessClientRequests, _httpListener.GetContext());
            }
        }

        private void ProcessClientRequests(object ctx)
        {
            var context = (HttpListenerContext)ctx;

            string filePath = Utility.GetLocationOfFileRequested(FolderToServe, context.Request.Url.ToString());

            if (filePath.Contains("favicon.ico")) return;

            byte[] buffer;
            string mimeType;

            try
            {
                buffer = File.ReadAllBytes(filePath);

                var extension = Path.GetExtension(filePath);

                mimeType = MimeTypes.GetMimeType(extension);
            }
            catch(Exception exception)
            {
                if (exception is FileNotFoundException || 
                    exception is DirectoryNotFoundException)
                {
                    Utility.BuildResponseAndSend(ref context,
                                         Encoding.ASCII.GetBytes("404: File not found. Kya kar raha hai?"),
                                         404,
                                         MimeTypes.GetMimeType(".txt"));
                    return;
                }
                
                if (exception is KeyNotFoundException)
                {
                    Utility.BuildResponseAndSend(ref context,
                                    Encoding.ASCII.GetBytes("510: Mime-Type not supported."),
                                    510,
                                    MimeTypes.GetMimeType(".txt"));
                    return;
                }

                if (exception is UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.ToString());
                    return;
                }

                throw;
            }

            Utility.BuildResponseAndSend(ref context, buffer, 200, mimeType);
        }

        public void StopListening()
        {
            _httpListener.Stop();
            _httpListener.Close();
        }
    }
}
