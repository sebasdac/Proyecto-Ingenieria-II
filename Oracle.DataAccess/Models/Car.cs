using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class Car
    {
        public Car()
        {
            CarColors = new HashSet<CarColor>();
            CarTransmissions = new HashSet<CarTransmission>();
        }

        public decimal Id { get; set; }
        public string Model { get; set; } = null!;
        public int Year { get; set; }

        public virtual ICollection<CarColor> CarColors { get; set; }
        public virtual ICollection<CarTransmission> CarTransmissions { get; set; }
    }
}
