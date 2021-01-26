using Data.Models;
using ProcessingService.Models;
using System;
using System.Diagnostics;
using System.Net;

namespace ProcessingService.Services
{
    public class HttpService : IProcessor
    {
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
            string host = ep.Ip;
            ResponseResult result = new ResponseResult
            {
                IsReachable = true,
                Id = ep.Id
            };
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
                Stopwatch watch = new Stopwatch();
                watch.Start();
                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                watch.Stop();
                TimeSpan span = watch.Elapsed;
                result.Latency = span.Milliseconds / 10;
                result.StatusMessage = response.StatusCode.ToString();

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

