using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class Cartitem
    {
        public decimal Cartitemid { get; set; }
        public decimal Cartid { get; set; }
        public decimal Productid { get; set; }
        public string Producttype { get; set; } = null!;
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual Cart Cart { get; set; } = null!;
    }
}
