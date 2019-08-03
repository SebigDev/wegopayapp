using System;

namespace userprofile.entities.Models
{
    public class UserModel
    {
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string EmailAddress { get; set; }

        public byte[] PaswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Password { get; set; }

        public string PhoneNo { get; set; }
        public string ReferredBy { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActivated { get; set; }
        public bool IsActive { get; set; }
    }
}
