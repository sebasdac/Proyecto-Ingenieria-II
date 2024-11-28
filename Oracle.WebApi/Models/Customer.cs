using System;
using System.Collections.Generic;

namespace Oracle.WebApi.Models
{
    public partial class Customer
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Cedula { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
