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

        public WebUser(int id, string userName, string identityId, ICollection<EndPoint> endPoint)
        {
            Id = id;
            UserName = userName;
            EndPoint = endPoint;
            IdentityId = identityId;
        }

        public int Id { get; set; }
        public string IdentityId { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<EndPoint> EndPoint { get; set; }
    }
}
