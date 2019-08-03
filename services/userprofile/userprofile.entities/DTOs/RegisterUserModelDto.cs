using System;
using System.Collections.Generic;
using System.Text;

namespace userprofile.entities.DTOs
{
   public class RegisterUserModelDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string EmailAddress { get; set; }

        public string Pasword { get; set; }

        public string PhoneNo { get; set; }
        public string ReferredBy { get; set; }
    }
}
