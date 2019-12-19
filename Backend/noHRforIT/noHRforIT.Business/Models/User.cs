using System;
using System.Collections.Generic;
using System.Text;

namespace noHRforIT.Business.Models
{
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public long TokenExpirationTime { get; set; }
    }
}
