using System;
using System.Collections.Generic;

namespace Oracle.DataAccess.Models
{
    public partial class VacationRequest
    {
        public long Id { get; set; }
        public long? EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = null!;
        public string? Comments { get; set; }

        public virtual Employee? Employee { get; set; }
    }
}
