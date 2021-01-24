using Data.Models;
using ProcessingService.Models;
using System;
using System.Diagnostics;
using System.Net;

namespace ProcessingService.Services
{
    public class HttpService : IProcessor
    {

        private string SetHostName(string hostname)
        {
            string host = hostname;
            if (!hostname.Trim().StartsWith("http://") || !hostname.Trim().StartsWith("https://"))
                host = String.Concat("http://", hostname);
            return host;
        } 

        private bool IsHostNameValid(string hostname)
        {
            if (Object.Equals(hostname, null)) return false;
            return true;
        }

        public ResponseResult CheckConnection(Data.Models.EndPoint ep)
        {
            if (!IsHostNameValid(ep.Ip))
            {
                return new ResponseResult()
                {
                    Id = ep.Id,
                    TimeStamp = DateTime.UtcNow,
                    IsReachable = false,
                    Latency = 0,
                    StatusMessage = "Invalid hostname: " + ep.Ip,
                };
            }
            string host = SetHostName(ep.Ip);
            ResponseResult result = new ResponseResult();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            try
            {
                result.Id = ep.Id;
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

