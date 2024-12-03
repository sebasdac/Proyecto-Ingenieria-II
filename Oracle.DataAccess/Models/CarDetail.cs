using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class CarDetail
    {
        public decimal Id { get; set; }
        public decimal? CarId { get; set; }
        public string? TransmissionType { get; set; }
        public string? Color { get; set; }
        public decimal? Stock { get; set; }
        public decimal? Price { get; set; }

        public virtual Car? Car { get; set; }
    }
}
