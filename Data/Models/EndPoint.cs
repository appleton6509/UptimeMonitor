using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
   public enum Protocol
    {
        Http,
        Https,
        Ftp
    }
    public class EndPoint : BaseModel
    {
        public Guid UserId { get; set; }

        [StringLength(30, ErrorMessage = "{0} must be a maximum of {1} characters")]
        public string Description { get; set; }

        [Display(Name = "Site")]
        [RegularExpression(@"?[a-z0-9]+([-.]{1}[a-z0-9]+)*.[a-z]{2,5}(:[0-9]{1,5})?(/.*)?$",
    ErrorMessage = "{0} must be a valid web address")]
        [StringLength(60, ErrorMessage = "{0} must be a minimum of {2} characters", MinimumLength = 5)]
        public string Ip { get; set; }

        public Protocol Protocol { get; set; }

        public bool? NotifyOnFailure { get; set; }

        public virtual ICollection<ResultData> HttpResult { get; set; }

    }
}