using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class User
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public virtual ICollection<EndPoint> EndPoint { get; set; }
    }

}
