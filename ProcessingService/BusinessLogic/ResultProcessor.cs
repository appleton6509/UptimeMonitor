using Microsoft.Extensions.Logging;
using ProcessingService.DTO;
using ProcessingService.Models;
using ProcessingService.Services;
using ProcessingService.Services.Email;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessingService.BusinessLogic
{
    public class ResultProcessor
    {
        private readonly IDatabaseService db;
        private readonly IEmailService email;
        private readonly ILogger<ResultProcessor> log;

        public ResultProcessor(IDatabaseService db, IEmailService email, ILogger<ResultProcessor> log)
        {
            this.db = db;
            this.email = email;
            this.log = log;
        }
        public void Process(Queue<TaskResultDTO> tasks)
        {
            if (tasks.Count == 0)
                return;
           log.LogInformation($"{DateTime.UtcNow} - adding {tasks.Count} records to db");

            Queue<TaskResultDTO> temp = new Queue<TaskResultDTO>(tasks);
            while(temp.Count > 0)
            {
                TaskResultDTO result = temp.Dequeue();
                ProcessDb(result.Response);
                ProcessEmail(result.EndPoint);
            }
        }

        public void ProcessEmail(EndPointExtended ep)
        {
            if (!ep.NotifyOnFailure)
                return;
            email.SendFailureEmail("appleton6509@gmail.com", ep);
        }

        public void ProcessDb(ResponseResult result)
        {
            try
            {
                db.Create(new ResultData(result));
            }
            catch (Exception e)
            {
                log.LogError("Error adding to DB: " + e.Message);
            }
        }

    }
}
