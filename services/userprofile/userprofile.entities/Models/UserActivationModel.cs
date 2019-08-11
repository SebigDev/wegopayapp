using System;
using System.Collections.Generic;
using System.Text;

namespace userprofile.entities.Models
{
    public class UserActivationModel
    {
        public long Id { get; set; }
        public long UserModelId { get; set; }
        public string ActivationCode { get; set; }
        public DateTime SentOn { get; set; }
        public DateTime? ActivatedOn { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool ComfirmedActivation { get; set; }
    }
}
