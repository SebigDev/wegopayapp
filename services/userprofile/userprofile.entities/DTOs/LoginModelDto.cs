using System;
using System.Collections.Generic;
using System.Text;

namespace userprofile.entities.DTOs
{
    public class LoginModelDto
    {
        public string EmailAddress { get; set; }
        public string PhoneNo { get; set; }
        public string Password { get; set; }
        public bool IsRememberMe { get; set; }
    }
}
