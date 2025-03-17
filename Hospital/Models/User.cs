using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Cnp { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegistrationDate { get; set; }

        public User(int userId, string username, string password, string mail, string role, string name, DateOnly birthDate, string cnp, string address, string phoneNumber, DateTime registrationDate)
        {
            UserId = userId;
            Username = username;
            Password = password;
            Mail = mail;
            Role = role;
            Name = name;
            BirthDate = birthDate;
            Cnp = cnp;
            Address = address;
            PhoneNumber = phoneNumber;
            RegistrationDate = registrationDate;
        }
    }
}
