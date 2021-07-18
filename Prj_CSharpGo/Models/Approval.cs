using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class Approval
    {
        public int EmployeeId { get; set; }
        public string ApprovalSheet { get; set; }
        public string ApprovalPk { get; set; }
        public string ApprovalColumn { get; set; }
        public string ApprovalAfter { get; set; }
        public DateTime? ApprovalDate { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
