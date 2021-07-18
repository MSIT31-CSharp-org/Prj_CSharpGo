using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeePassword { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public bool? EmployeeStatus { get; set; }
    }
}
