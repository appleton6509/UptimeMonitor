using ProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessingService.Services
{
    public class TaskResult
    {
        public ResponseResult Response { get; set; }
        public bool EmailOnCompletion { get; set; }
    }
}
