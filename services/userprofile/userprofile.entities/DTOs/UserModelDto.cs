using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using userprofile.entities.Models;

namespace userprofile.entities.DTOs
{
    public class UserModelDto
    {

        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string EmailAddress { get; set; }

        public string PhoneNo { get; set; }
        public string ReferredBy { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActivated { get; set; }
        public bool IsActive { get; set; }

        public static Expression<Func<UserModel, UserModelDto>> User
        {
            get
            {
                return model => new UserModelDto
                {
                    Id = model.Id,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    EmailAddress = model.EmailAddress,
                    DateCreated = model.DateCreated,
                    PhoneNo = model.PhoneNo,
                    IsActivated = model.IsActivated,
                    IsActive = model.IsActive,
                    ReferredBy = model.ReferredBy
                };
            }
        }
    }
}
