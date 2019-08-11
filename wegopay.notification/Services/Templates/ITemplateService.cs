using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace wegopay.notification.Services.Templates
{
    public interface ITemplateService
    {
        Task<string> RetrieveTemplate(int notification);
    }
}
