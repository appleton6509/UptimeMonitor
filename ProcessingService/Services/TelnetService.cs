using Microsoft.Extensions.Logging;
using ProcessingService.BusinessLogic;
using ProcessingService.Models;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace ProcessingService.Services
{
    public interface ITelnetService : IProtocolService { }

    public class TelnetService : CommandService, ITelnetService { }
}
