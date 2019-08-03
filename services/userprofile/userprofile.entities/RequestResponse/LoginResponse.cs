using System;
using System.Collections.Generic;
using System.Text;

namespace userprofile.entities.RequestResponse
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string EmailAddress { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsActived { get; set; }
    }

}
