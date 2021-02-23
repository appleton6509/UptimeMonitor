using Microsoft.Extensions.Logging;
using ProcessingService.BusinessLogic;
using ProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingService.Services
{
    public interface IFtpService : IProtocolService { }
    public class FtpService : IFtpService
    {
        private readonly ILogger<FtpService> log;
        public FtpService(ILogger<FtpService> logger)
        {
            log = logger;
        }
        public ResponseResult CheckConnection(EndPointExtended endpoint)
        {
            return CheckFTP(endpoint, endpoint.Protocol);
        }
        private ResponseResult CheckFTP(EndPointExtended endpoint, Data.Models.Protocol protocol)
        {
            FtpWebRequest request = CreateFtpRequest(endpoint, protocol);
            ResponseResult result;
            Stopwatch watch = new Stopwatch();
            try
            {
                watch.Start();
                var response = (FtpWebResponse)request.GetResponse();
                watch.Stop();
                result = CreateResponseResult(endpoint, watch.Elapsed.Milliseconds / 10);
            }
            catch (Exception e)
            {
                result = CreateResponseResult(endpoint,0,e);
            }
            return result;
        }
        private ResponseResult CreateResponseResult(EndPointExtended endpoint,int latency = 0,Exception e = null)
        {
            ResponseResult result = ResponseResult.Create(endpoint);
            if (e.GetType() == typeof(WebException))
            {
                WebException exception = (e as WebException);
                switch (exception.Status)
                {
                    case WebExceptionStatus.Success: //means connection was successful
                        result.UpdateAsSuccessful(latency);
                        break;
                    case WebExceptionStatus.ProtocolError: // username/password error
                        result.UpdateAsSuccessful(latency);
                        break;
                    default:
                        result.UpdateAsFailed($"{exception.Status} - {exception.Message}");
                        break;
                }
            } else if (e.GetType() == typeof(Exception))
            {
                result.UpdateAsFailed($"{e.Message}");
                log.LogError($"IP: {endpoint.Ip} - {e.Message}");
            } else
                result.UpdateAsSuccessful(latency);
            return result;
        }

        private static FtpWebRequest CreateFtpRequest(EndPointExtended endpoint, Data.Models.Protocol protocol)
        {
            var request = (FtpWebRequest)WebRequest.Create($"ftp://{endpoint.Ip}");
            request.Credentials = new NetworkCredential("username", "password");
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            if (protocol == Data.Models.Protocol.ftps)
                request.EnableSsl = true;

            return request;
        }

        private ResponseResult CheckSFTP(EndPointExtended endpoint)
        {
            throw new NotImplementedException();
        }
    }
}
