using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class CarSale
    {
        public long Id { get; set; }
        public string CarModel { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime SaleDate { get; set; }
        public string CustomerName { get; set; } = null!;
    }
}
