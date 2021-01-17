using Data.Models;
using ProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessingService.Services
{
    public interface IProcessor
    {
        public ResponseResult CheckConnection(EndPoint endpoint);
    }
}
