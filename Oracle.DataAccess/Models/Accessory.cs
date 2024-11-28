using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class Accessory
    {
        public decimal Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Cost { get; set; }
        public decimal Quantity { get; set; }
        public string? Description { get; set; }
    }
}
