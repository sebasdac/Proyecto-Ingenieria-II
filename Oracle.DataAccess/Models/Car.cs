using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class Car
    {
        public Car()
        {
            CarDetails = new HashSet<CarDetail>();
        }

        public decimal Id { get; set; }
        public string? Model { get; set; }
        public decimal? Year { get; set; }

        public virtual ICollection<CarDetail> CarDetails { get; set; }
    }
}
