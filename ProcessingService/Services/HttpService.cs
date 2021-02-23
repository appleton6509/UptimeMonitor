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

        public ResponseResult CheckConnection(EndPointExtended endpoint)
        {
            if (endpoint.Ip is null)
            {
                ResponseResult failedResult = ResponseResult.Create(endpoint);
                failedResult.StatusMessage = "Invalid hostname: " + endpoint.Ip;
                return failedResult;
            }
            ResponseResult result = ResponseResult.Create(endpoint);
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                string host = $"{endpoint.Protocol}://{endpoint.Ip}";
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, host);
                HttpResponseMessage res = http.SendAsync(message).Result;
                watch.Stop();
                CreateResponseResult(endpoint, res, watch.Elapsed.Milliseconds / 10);
            }
            catch (Exception e)
            {
                result = CreateResponseResult(endpoint, null, 0, e);
            }
            return result;
        }
        private ResponseResult CreateResponseResult(EndPointExtended endpoint, HttpResponseMessage response = null, int latency = 0,Exception exception = null)
        {
            ResponseResult result = ResponseResult.Create(endpoint);
            if (response.IsSuccessStatusCode)
            {
                result.IsReachable = true;
                result.StatusMessage = response.ReasonPhrase;
                result.Latency = latency;
            }  else
            {
                result.IsReachable = false;
                result.StatusMessage = (exception is null) ? response.ReasonPhrase : exception.Message;
            }
            return result;

        }
    }
}

