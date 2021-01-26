using Data.Models;
using Microsoft.Extensions.Logging;
using ProcessingService.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProcessingService.Services
{
    public class HttpService : IHttpService
    {       
        private readonly HttpClient _client;
        private readonly ILogger<HttpService> _logger;
        public HttpService(HttpClient client, ILogger<HttpService> logger)
        {
            _client = client;
            _logger = logger;
        }
        private bool IsHostNameValid(string hostname)
        {
            if (Object.Equals(hostname, null)) return false;
            return true;
        }

        public async Task<ResponseResult> CheckConnection(Data.Models.EndPoint ep)
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
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, host);
                Stopwatch watch = new Stopwatch();
                watch.Start();
                HttpResponseMessage res = await _client.SendAsync(message);
               watch.Stop();
                TimeSpan span = watch.Elapsed;
                if (!res.IsSuccessStatusCode)
                    result.IsReachable = false;
                result.Latency = span.Milliseconds / 10;
                result.StatusMessage = res.ReasonPhrase;
            }
            catch (HttpRequestException e)
            {
                result.IsReachable = false;
                result.StatusMessage = e.Message;
                _logger.LogInformation($"Unable to reach {host} with message: " + e.Message);
            } catch (Exception e)
            {
                result.IsReachable = false;
                result.StatusMessage = "Internal error: " + e.Message;
                _logger.LogCritical($"Error occured in request to {host} with message: " + e.Message);
            }
            
            return result;
        }
    }
}

