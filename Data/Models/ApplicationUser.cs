using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>, IBaseModel
    {
        public ApplicationUser() { }
        public ApplicationUser(string userName)
        {
            this.UserName = this.Email = userName;
        }
        public virtual ICollection<EndPoint> EndPoint { get; set; }
    }
}
