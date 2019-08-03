using System;
using System.Collections.Generic;
using System.Text;

namespace userprofile.entities.DTOs
{
    public class UserActivationDto
    {
        public long UserId { get; set; }
        public string ActivationCode { get; set; }
    }
}
