using System;
using System.Collections.Generic;

namespace Oracle.WebApi.Models
{
    public partial class Invoice
    {
        public long Id { get; set; }
        public long? OrderId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;

        public virtual Order? Order { get; set; }
    }
}
