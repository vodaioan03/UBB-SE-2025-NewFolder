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
        public double Rating { get; set; }
        public string LicenseNumber { get; set; }
        public string DoctorName { get; set; }


        public DoctorJointModel(int doctorId, int userId, string doctorName, int departmentId, double rating, string licenseNumber)
        {
            DoctorId = doctorId;
            UserId = userId;
            DepartmentId = departmentId;
            Rating = rating;
            LicenseNumber = licenseNumber;
            DoctorName = doctorName;
        }

    }
}
