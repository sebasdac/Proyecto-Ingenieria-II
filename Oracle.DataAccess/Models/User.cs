using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class User
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public long? CustomerId { get; set; }

        public virtual Customer? Customer { get; set; }
    }
}
