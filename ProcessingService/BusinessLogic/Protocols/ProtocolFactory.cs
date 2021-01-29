﻿
using Microsoft.Extensions.Logging;
using ProcessingService.Models;
using ProcessingService.Services;
using System.Collections.Generic;
using System.Net.Http;


namespace ProcessingService.BusinessLogic.Protocols
{
    public class ProtocolFactory
    {
        private readonly Dictionary<Data.Models.Protocol, IProtocolService> Map;
        private readonly IHttpService http;
        private readonly IFtpService ftp;
        public ProtocolFactory(
            IFtpService ftpService,
            IHttpService httpService)
        {
            ftp = ftpService;
            http = httpService;

            Map = new Dictionary<Data.Models.Protocol, IProtocolService>() {
                { Data.Models.Protocol.http , http },
                { Data.Models.Protocol.https, http },
                { Data.Models.Protocol.ftp, ftp },
                { Data.Models.Protocol.ftps, ftp },
                { Data.Models.Protocol.sftp, ftp }
            };
        }
        public List<IProtocol> GetProtocols(List<EndPointExtended> eps)
        {
            List<IProtocol> tasks = new List<IProtocol>();
            foreach (var ep in eps)
            {
                IProtocol protocol = GetProtocol(ep);
                if (protocol != null)
                    tasks.Add(protocol);
            }
            return tasks;
        }

        private IProtocol GetProtocol(EndPointExtended ep)
        {
            IProtocolService service =  Map[ep.Protocol];
            IProtocol protocol = new Protocol(service, ep);

            return protocol;
        }
    }
}