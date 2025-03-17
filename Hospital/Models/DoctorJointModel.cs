using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class DoctorJointModel
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public float Rating { get; set; }
        public string LicenseNumber { get; set; }

        public DoctorJointModel(int doctorId, int userId, int departmentId, float rating, string licenseNumber)
        {
            DoctorId = doctorId;
            UserId = userId;
            DepartmentId = departmentId;
            Rating = rating;
            LicenseNumber = licenseNumber;
        }
    }
}
