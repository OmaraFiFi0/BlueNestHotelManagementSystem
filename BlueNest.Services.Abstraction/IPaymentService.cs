using BlueNest.Shared.Reponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Services.Abstraction
{
    public interface IPaymentService
    {

        Task<GenericResponse<string>> CreatePaymentUrlAsync(Guid bookingId);
    }
}
