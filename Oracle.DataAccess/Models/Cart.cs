using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class Cart
    {
        public Cart()
        {
            Cartitems = new HashSet<Cartitem>();
            Users = new HashSet<User>();
        }

        public decimal Cartid { get; set; }
        public decimal Userid { get; set; }
        public DateTime? Createddate { get; set; }

        public virtual ICollection<Cartitem> Cartitems { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
