using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Core.Entities.BookingModule
{
    public enum BookingStatus
    {
        PaymentPending = 0,
        PaymentPaid = 1,
        PaymentCancelled = 2,
        PaymentFaild = 3,
    }
}
