using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class Inventory
    {
        public long? Id { get; set; }
        public long? CarId { get; set; }
        public int? Quantity { get; set; }
    }
}
