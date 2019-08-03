using System;
using System.Collections.Generic;
using System.Text;

namespace userprofile.entities.RequestResponse
{
    public class RegistrationResponse
    {
        public bool IsEmailTaken { get; set; }
        public bool IsSucceeded { get; set; }
    }
}
