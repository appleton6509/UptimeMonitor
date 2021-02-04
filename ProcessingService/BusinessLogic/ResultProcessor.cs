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
    public class ResultProcessor : IObserver<TaskResultDTO>
    {
        private readonly IDatabaseService db;
        private readonly IEmailService email;
        private readonly ILogger<ResultProcessor> log;
        private IDisposable unsubscriber;
        public ResultProcessor(IDatabaseService db, IEmailService email, ILogger<ResultProcessor> log)
        {
            this.db = db;
            this.email = email;
            this.log = log;
        }

        public void ProcessEmail(TaskResultDTO ep)
        {
            if (!ep.EndPoint.NotifyOnFailure || ep.Response.IsReachable)
                return;
            email.SendFailureEmail(ep.EndPoint);
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
        /// <summary>
        /// not implemented
        /// </summary>
        public void OnCompleted()
        {
           //do nothing, wait for more requests
        }
       /// <summary>
       /// not implemented
       /// </summary>
       /// <param name="error"></param>
        public void OnError(Exception error)
        {
          // do nothing
        }

        public void OnNext(TaskResultDTO value)
        {
            this.ProcessDb(value.Response);
            this.ProcessEmail(value);
        }

        public void Subscribe(IObservable<TaskResultDTO> observerable)
        {
            unsubscriber = observerable.Subscribe(this);
        }
        public void Unsubscribe()
        {
            if (unsubscriber != null)
                unsubscriber.Dispose();
        }
    }
}
