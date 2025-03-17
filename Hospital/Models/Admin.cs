using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        public int UserId { get; set; }

        public Admin(int adminId, int userId)
        {
            AdminId = adminId;
            UserId = userId;
        }
    }
}
