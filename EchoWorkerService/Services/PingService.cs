 using EchoWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;


namespace EchoWorkerService.Services
{
    static class PingStatus
    {
        /// <summary>
        /// List of status codes and definitions returns from a ping reply
        /// </summary>
        public static Dictionary<int, string> Message { get; } = new Dictionary<int, string>()
        {
            { -1,    "Unknown Error" },
            { 0,     "SUCCESS" },
            { 11001, "The reply buffer was too small." },
            { 11002, "The destination network was unreachable" },
            { 11003, "the destination host was unreachable" },
            { 11004, "The destination protocol was unreachable" },
            { 11005, "The destination port was unreachable" },
            { 11006, "Insufficient IP resources were available" },
            { 11007, "A bad IP Option was specified" },
            { 11008, "A hardware error occured" },
            { 11009, "The packet was too big" },
            { 11010, "Packet has timed out" },
            { 11011, "A bad request" },
            { 11012, "A bad route" },
            { 11013, "The TTL expired in transit" },
            { 11014, "The TTL expired during fragment reassembly" },
            { 11015, "a parameter problem" },
            { 11016, "Datagrams are arriving too fast to be processed and have been discarded" },
            { 11017, "An IP option was too big" },
            { 11018, "a bad destination" },
            { 11050, "A general failure. This error can be returned for some malformed ICMP packets" },
        };
    }

    public class PingService
    {
        /// <summary>
        /// DNS or IP of host to ping
        /// </summary>
        public string AddressOrIp { get; set; }

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressOrIp">DNS or IP address of host to ping</param>
        /// <param name="intervalBetweenPings">Interval in milliseconds between pings</param>
        public PingService(string addressOrIp)
        {
            this.AddressOrIp = addressOrIp;

        }
        public PingService() {

        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// Starts an asynchronous ping request and returns the result
        /// </summary>
        /// <returns></returns>
        public async Task<PingResult> StartPingAsync()
        {

            Ping sender = new Ping();

            PingResult result = new PingResult()
            {
                TimeStamp = DateTime.Now,
                AddressOrIp = this.AddressOrIp
            };
            PingReply reply;

            try
            {
                reply = await sender.SendPingAsync(this.AddressOrIp);

                result.AddReplyData(reply);
            }
            catch (Exception b)
            {
                result.AddReplyException(b);
            }
            finally
            {
                //release resource
                sender.Dispose();
            }
            //return the ping results
            return result;
        }

        #endregion Public Methods


    }
}
