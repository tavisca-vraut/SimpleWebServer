using System;
using System.IO;
using System.Net;

namespace WebServer
{
    internal class Utility
    {
        internal static void BuildResponseAndSend(ref HttpListenerContext context, byte[] buffer, int statusCode, string mimeType)
        {
            context.Response.ContentType = mimeType;
            context.Response.StatusCode = statusCode;
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        internal static string GetLocationOfFileRequested(string folderToServe, string url)
        {
            string relativeFileLocation = url.Substring($"http://localhost:{WebServer.PortNumber}/".Length);
            relativeFileLocation = relativeFileLocation.Replace('/', '\\');
            string filePath = Path.Combine(folderToServe, relativeFileLocation);

            return filePath;
        }
    }
}
