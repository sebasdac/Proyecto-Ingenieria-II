using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class CarColor
    {
        public decimal Id { get; set; }
        public decimal CarId { get; set; }
        public string Color { get; set; } = null!;

        public virtual Car? Car { get; set; } = null!;
    }
}
