using ProcessingService.BusinessLogic;
using ProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace ProcessingService.Services
{
    public class CommandService : IProtocolService
    {
        public virtual ResponseResult CheckConnection(EndPointExtended endpoint)
        {
            int port = (int)DefaultPort.Telnet;
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
                WriteToStream(stream, "RandomDataAnythingGoes");
                responseData += ReadStreamResponse(stream);
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            return CreateResponseResult(endpoint, responseData,watch, error);
        }

        internal static ResponseResult CreateResponseResult(EndPointExtended endpoint, string responseData, Stopwatch watch, string errorMessage = "")
        {
            ResponseResult result = ResponseResult.Create(endpoint);
            if (String.IsNullOrEmpty(responseData) || !String.IsNullOrEmpty(errorMessage))
                    result.UpdateAsFailed(errorMessage);
            result.UpdateAsSuccessful(watch.Elapsed.Milliseconds / 10);
            return result;
        }

        internal static void WriteToStream(NetworkStream stream, string writeData)
        {
            byte[] writedata = Encoding.ASCII.GetBytes(writeData);
            stream.Write(writedata, 0, writedata.Length);
        }

        internal static string ReadStreamResponse(NetworkStream stream)
        {
            byte[] buffer = new byte[2048];
            Int32 bytesRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

    }
}
