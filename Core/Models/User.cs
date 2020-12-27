using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class User
    {
        public User(string userName)
        {
            this.UserName = userName;
        }

        public User(int iD, string userName, ICollection<EndPoint> endPoint)
        {
            ID = iD;
            UserName = userName;
            EndPoint = endPoint;
        }

        public int ID { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<EndPoint> EndPoint { get; set; }
    }
}
