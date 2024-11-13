using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class Invoice
    {
        public int Id { get; set; }
        public long? OrderId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;

        public virtual Order? Order { get; set; }
    }
}
