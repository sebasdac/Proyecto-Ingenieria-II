using System;
using System.Collections.Generic;

namespace Proyecto_Ingenieria.Models
{
    public partial class Employee
    {
        public Employee()
        {
            VacationRequests = new HashSet<VacationRequest>();
        }

        public long Id { get; set; }
        public string EmployeeName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public string LastName { get; set; } = null!;
        public string Cedula { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;

        public virtual ICollection<VacationRequest> VacationRequests { get; set; }
    }
}
