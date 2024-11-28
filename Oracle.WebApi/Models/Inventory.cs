using System;
using System.Collections.Generic;

namespace Oracle.WebApi.Models
{
    public partial class Inventory
    {
        public long Id { get; set; }
        public long? CarId { get; set; }
        public int Quantity { get; set; }

        public virtual Car? Car { get; set; }
    }
}
