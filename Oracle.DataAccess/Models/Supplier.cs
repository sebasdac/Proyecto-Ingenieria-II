using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class Supplier
    {
        public long Id { get; set; }
        public string SupplierName { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
