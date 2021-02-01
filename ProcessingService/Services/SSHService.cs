using ProcessingService.BusinessLogic;
using ProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace ProcessingService.Services
{
    public interface ISSHService : IProtocolService { }

    //test site: zeus.darkshell.eu

    public class SSHService : ISSHService
    {
        public ResponseResult CheckConnection(EndPointExtended endpoint)
        {
            ResponseResult result = new ResponseResult()
            {
                Id = endpoint.Id,
                Protocol = endpoint.Protocol,
                TimeStamp = DateTime.UtcNow,
                IsReachable = true,
            };

            int port = (int)DefaultPort.SSH;
            string server = endpoint.Ip;
            string responseData = String.Empty;
            Stopwatch watch = new Stopwatch();
            try
            {
                //tcp connection
                using TcpClient client = new TcpClient(server, port);
                using NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[2048];
                watch.Start();
                Int32 bytesRead = stream.Read(buffer, 0, buffer.Length);
                responseData = Encoding.ASCII.GetString(buffer, 0, bytesRead);                 //read response
                watch.Stop();
                //send any data 
            }
            catch (Exception e)
            {
                result.IsReachable = false;
                result.StatusMessage = e.Message;
            }
            finally
            {
                if (result.IsReachable)
                    result.Latency = watch.Elapsed.Milliseconds / 10;
            }

            if (String.IsNullOrEmpty(responseData))
                result.IsReachable = false;

            return result;
        }
    }
}
