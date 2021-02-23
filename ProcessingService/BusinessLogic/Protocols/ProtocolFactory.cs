
using Microsoft.Extensions.Logging;
using ProcessingService.Models;
using ProcessingService.Services;
using System.Collections.Generic;
using System.Net.Http;


namespace ProcessingService.BusinessLogic.Protocols
{
    public class ProtocolFactory
    {
        private readonly Dictionary<Data.Models.Protocol, IProtocolService> map;
        private readonly IHttpService http;
        private readonly IFtpService ftp;
        private readonly ISSHService ssh;
        private readonly ITelnetService telnet;

        public ProtocolFactory(
            IFtpService ftpService,
           ITelnetService telnetService,
            ISSHService sshService,
            IHttpService httpService)
        {
            ftp = ftpService;
            http = httpService;
            telnet = telnetService;
            ssh = sshService;

            map = new Dictionary<Data.Models.Protocol, IProtocolService>() {
                { Data.Models.Protocol.http , http },
                { Data.Models.Protocol.https, http },
                { Data.Models.Protocol.ftp, ftp },
                { Data.Models.Protocol.ftps, ftp },
                { Data.Models.Protocol.sftp, ftp },
                { Data.Models.Protocol.telnet, telnet },
                { Data.Models.Protocol.ssh, ssh }
            };
        }

        public List<IProtocol> MapToProtocols(List<EndPointExtended> eps)
        {
            List<IProtocol> tasks = new List<IProtocol>();
            foreach (var ep in eps)
            {
                if (ep is null)
                    continue;
                IProtocol protocol = MapToProtocol(ep);
                tasks.Add(protocol);
            }
            return tasks;
        }

        private IProtocol MapToProtocol(EndPointExtended ep)
        {
            if (ep is null)
                return null;
            IProtocolService service = map[ep.Protocol];
            IProtocol protocol = new Protocol(service, ep);

            return protocol;
        }
    }
}
