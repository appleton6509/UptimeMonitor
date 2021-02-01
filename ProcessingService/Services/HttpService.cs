using Data.Models;
using Microsoft.Extensions.Logging;
using ProcessingService.BusinessLogic;
using ProcessingService.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProcessingService.Services
{
    public interface IHttpService : IProtocolService { }

    public class HttpService : IHttpService
    {
        private readonly HttpClient http;
        private readonly ILogger log;
        public HttpService(HttpClient client, ILogger<HttpService> logger)
        {
            http = client;
            log = logger;
        }

        public ResponseResult CheckConnection(EndPointExtended ep)
        {
            if (ep.Ip is null)
                return new ResponseResult()
                {
                    Id = ep.Id,
                    TimeStamp = DateTime.UtcNow,
                    IsReachable = false,
                    Latency = 0,
                    StatusMessage = "Invalid hostname: " + ep.Ip,
                    Protocol = Data.Models.Protocol.http
                };

            string host = $"{ep.Protocol}://{ep.Ip}";
            ResponseResult result = new ResponseResult
            {
                IsReachable = true,
                Id = ep.Id,
                Protocol = ep.Protocol
            };

            try
            {
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, host);
                Stopwatch watch = new Stopwatch();
                watch.Start();
                HttpResponseMessage res = http.SendAsync(message).Result;
                watch.Stop();
                if (!res.IsSuccessStatusCode)
                    result.IsReachable = false;
                result.Latency = watch.Elapsed.Milliseconds / 10;
                result.StatusMessage = res.ReasonPhrase;
            }
            catch (HttpRequestException e) //failed to reach endpoint
            {
                result.IsReachable = false;
                result.StatusMessage = e.Message;

            }
            catch (Exception e) //system error
            {
                result.IsReachable = false;
                result.StatusMessage = "Internal error: " + e.Message;
                log.LogError($"Error occured in request to {host} with message: " + e.Message);
            }
            return result;
        }
    }
}

