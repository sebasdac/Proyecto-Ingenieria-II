using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class Order
    {
        public Order()
        {
            Invoices = new HashSet<Invoice>();
        }

        public long Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string CarModel { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public DateTime OrderDate { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
