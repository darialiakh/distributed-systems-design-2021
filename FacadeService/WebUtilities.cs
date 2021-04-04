using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using static System.Int32;

namespace FacadeService
{
    public static class WebUtilities
    {
        public static string Post(string url, string body, Dictionary<string, string> headers = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Method = "POST";
            request.Timeout = MaxValue;
            request.ProtocolVersion = HttpVersion.Version10;
            request.ContentType = "application/json";

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(body);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = (HttpWebResponse)request.GetResponse();

            return GetResponse(response);
        }

        public static string Post(string url, string body)
        {
            return SendData(url, body, "POST");
        }

        public static string Post(string url)
        {
            return SendData(url, null, "POST");
        }

        public static string Get(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Method = "GET";
            request.ServicePoint.Expect100Continue = false;

            var response = (HttpWebResponse)request.GetResponse();

            return GetResponse(response);
        }

        public static string GetResponse(HttpWebResponse webResponse)
        {
            string response;
            var receiveStream = webResponse.GetResponseStream();

            using (var readStream = new StreamReader(receiveStream ?? throw new InvalidOperationException(), Encoding.UTF8))
            {
                response = readStream.ReadToEnd();
            }

            return response;
        }

        private static string SendData(string url, string body, string method)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.Method = method;
            request.Timeout = MaxValue;
            request.ProtocolVersion = HttpVersion.Version10;
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(body);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = (HttpWebResponse)request.GetResponse();

            return GetResponse(response);
        }
    }
}