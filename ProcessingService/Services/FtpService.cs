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
    public interface IFtpService : IProtocolService
    {
    }
    public class FtpService : IFtpService
    {
        private readonly ILogger<FtpService> log;
        public FtpService(ILogger<FtpService> logger)
        {
            log = logger;
        }
        public ResponseResult CheckConnection(EndPointExtended endpoint)
        {
            ResponseResult res = null;
            switch (endpoint.Protocol)
            {
                case Data.Models.Protocol.ftp:
                    res = CheckFTP(endpoint);
                    break;
                case Data.Models.Protocol.ftps:
                    res = CheckFTP(endpoint, true);
                    break;
                case Data.Models.Protocol.sftp:
                    res = CheckSFTP(endpoint);
                    break;
            }
            return res;
        }
        private ResponseResult CheckFTP(EndPointExtended endpoint, bool useftps = false)
        {
            var request = (FtpWebRequest)WebRequest.Create($"ftp://{endpoint.Ip}");
            request.Credentials = new NetworkCredential("username", "password");
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            if (useftps)
                request.EnableSsl = true;

            ResponseResult result = new ResponseResult()
            {
                Id = endpoint.Id,
                IsReachable = false,
                Protocol = Data.Models.Protocol.ftp,
                TimeStamp = DateTime.UtcNow
            };
            Stopwatch watch = new Stopwatch();
            try
            {
                watch.Start();
                var response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException s)
            {
                switch (s.Status)
                {
                    case WebExceptionStatus.Success: //means connection was successful
                        result.IsReachable = true;
                        break;
                    case WebExceptionStatus.ProtocolError: // username/password error
                        result.IsReachable = true;
                        break;
                    default: 
                        result.IsReachable = false;
                        result.StatusMessage = $"{s.Status} - {s.Message}";
                        break;
                }
            }
            catch (Exception e)
            {
                result.IsReachable = false;
                result.StatusMessage = $"{e.Message}";
                log.LogError($"IP: {endpoint.Ip} - {e.Message}");

            }
            finally
            {
                watch.Stop();
                if (result.IsReachable)
                    result.Latency = watch.Elapsed.Milliseconds / 10;
            }
            return result;
        }

        private ResponseResult CheckSFTP(EndPointExtended endpoint)
        {
            throw new NotImplementedException();
        }
    }
}
