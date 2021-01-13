using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class WebUser
    {
        public WebUser(string userName, string identityId)
        {
            this.UserName = userName;
            this.IdentityId = identityId;
        }
        public Guid Id { get; set; }
        public string IdentityId { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<EndPoint> EndPoint { get; set; }
    }
}
