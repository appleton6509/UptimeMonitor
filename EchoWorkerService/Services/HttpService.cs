using ProcessingService.Models;
using System;
using System.Diagnostics;
using System.Net;

namespace ProcessingService.Services
{
    public class HttpService : IProcessor
    {
        private string _host;
        public string Host { 
            get
            {
                return this._host;
            }
        }

        public bool SetHostName(string hostname)
        {
            if (Object.Equals(hostname, null)) return false;

            string host = hostname;
            if (!hostname.Trim().StartsWith("http://"))
                host = String.Concat("http://", hostname);
            this._host = host;
            return true;
        } 

        public ResponseResult CheckConnection()
        {
            ResponseResult result = new ResponseResult();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Host);
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                watch.Stop();
                TimeSpan span = watch.Elapsed;
                result.Latency = span.Milliseconds / 10;
                result.StatusMessage = response.StatusCode.ToString();
                result.IsReachable = true;
            }
            catch (Exception e)
            {
                result.IsReachable = false;
                result.StatusMessage = e.Message;
            }
            return result;
        }
    }
}

