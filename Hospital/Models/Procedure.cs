using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class Procedure
    {
        public int ProcedureId { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        //Duration in minutes
        public TimeSpan Duration { get; set; }

        public Procedure(int procedureId, int departmentId, string name, TimeSpan duration)
        {
            this.ProcedureId = procedureId;
            this.DepartmentId = departmentId;
            this.Name = name;
            this.Duration = duration;
        }
    }
}
