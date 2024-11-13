using System;
using System.Collections.Generic;

namespace Proyecto_Ingenieria.Models
{
    public partial class Car
    {
        public Car()
        {
            Inventories = new HashSet<Inventory>();
        }

        public long Id { get; set; }
        public string Model { get; set; } = null!;
        public byte Year { get; set; }
        public string Transmission { get; set; } = null!;
        public string Color { get; set; } = null!;

        public virtual ICollection<Inventory> Inventories { get; set; }
    }
}
