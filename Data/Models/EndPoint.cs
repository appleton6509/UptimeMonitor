using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class EndPoint
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [StringLength(30, ErrorMessage = "{0} must be a maximum of {1} characters")]
        public string Description { get; set; }

        [RegularExpression(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?|^((http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$",
    ErrorMessage = "{0} must be a valid site or ip address")]
        [StringLength(60, ErrorMessage = "{0} must be a minimum of {2} characters", MinimumLength = 5)]
        public string Ip { get; set; }

        public virtual ICollection<HttpResult> HttpResult { get; set; }

    }
}