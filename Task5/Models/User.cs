using System;
using System.Collections.Generic;

namespace Task5.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string Status { get; set; }

        //IEnumerable<User> UsersList { get; set; }
    }
}
