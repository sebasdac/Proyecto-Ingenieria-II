using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class CarTransmission
    {
        public decimal Id { get; set; }
        public decimal CarId { get; set; }
        public string Transmission { get; set; } = null!;

        public virtual Car? Car { get; set; } = null!;
    }
}
