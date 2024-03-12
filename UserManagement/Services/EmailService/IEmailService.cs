using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs;

namespace UserManagement.Services.EmailService
{
    public interface IEmailService
    {
        bool SendEmail(EmailDTO request);
    }
}