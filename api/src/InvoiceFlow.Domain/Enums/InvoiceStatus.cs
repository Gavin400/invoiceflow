using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceFlow.Domain.Enums
{
    public enum InvoiceStatus
    {
         Draft,
    Submitted,
    Approved,
    Rejected,
    Paid
    }
}