using Microsoft.Extensions.Logging;
using ProcessingService.BusinessLogic;
using ProcessingService.Models;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace ProcessingService.Services
{
    public interface ISSHService { }

    //test site: zeus.darkshell.eu
    public class SshService : CommandService, ISSHService
    {
        public override ResponseResult CheckConnection(EndPointExtended endpoint)
        {
            int port = (int)DefaultPort.SSH;
            string server = endpoint.Ip;
            string responseData = String.Empty;
            string error = String.Empty;
            Stopwatch watch = new Stopwatch();
            try
            {
                watch.Start();
                using TcpClient client = new TcpClient(server, port);
                using NetworkStream stream = client.GetStream();
                responseData = ReadStreamResponse(stream);
                watch.Stop();
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            return CreateResponseResult(endpoint, responseData, watch, error);
        }
    }
}
