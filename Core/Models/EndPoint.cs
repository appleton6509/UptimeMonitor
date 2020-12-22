using System.Collections.Generic;

namespace Core.Models
{
    public class EndPoint
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Echo> Echo { get; set; }

    }
}