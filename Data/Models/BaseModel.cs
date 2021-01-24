using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class BaseModel : IBaseModel
    {
        public Guid Id { get; set; }
    }
}
