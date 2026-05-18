using BlueNest.Shared.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Services.Abstraction
{
    public interface IEmailService
    {
        Task SendEmail(Email email);
    }
}
