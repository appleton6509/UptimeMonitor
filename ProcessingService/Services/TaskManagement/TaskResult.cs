using ProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessingService.Services
{
    public class TaskResult
    {
        public ResponseResult Response { get; set; }
        public bool? NotifyOnFailure { get; set; }
        public string Email { get; set; }
    }
}
