using System;
using System.Collections.Generic;
using System.Text;

namespace noHRforIT.Business.Models.AuthorizationModels
{
    public class UpdateModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
