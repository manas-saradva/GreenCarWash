using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to,string subject,string body);
    }
}