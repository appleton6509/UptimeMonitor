using Microsoft.Extensions.Logging;
using ProcessingService.BusinessLogic;
using ProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace ProcessingService.Services
{
    public interface ITelnetService : IProtocolService { }

    public class TelnetService :ITelnetService
    {
        private ILogger<TelnetService> log;
        public TelnetService(ILogger<TelnetService> logger)
        {
            log = logger;
        }
        public ResponseResult CheckConnection(EndPointExtended endpoint)
        {
            ResponseResult result = new ResponseResult()
            {
                Id = endpoint.Id,
                Protocol = endpoint.Protocol,
                TimeStamp = DateTime.UtcNow,
                IsReachable = true,
            };

            int port = (int)DefaultPort.Telnet;
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
                byte[] writedata = Encoding.ASCII.GetBytes("testdata");
                stream.Write(writedata, 0, writedata.Length);
                //store response
                buffer = new byte[2048];
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                responseData += Encoding.ASCII.GetString(buffer, 0, bytesRead);                //read response

            }
            catch (Exception e)
            {
                result.IsReachable = false;
                result.StatusMessage = e.Message;
            } finally
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
