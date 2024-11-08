using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Oracle.DataAccess.Models
{
    public partial class Inventory
    {
        public long Id { get; set; }
        public long? CarId { get; set; }
        public int Quantity { get; set; }

   
        public virtual Car? Car { get; set; }
    }
}
