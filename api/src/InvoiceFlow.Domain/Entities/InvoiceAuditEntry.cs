using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoiceFlow.Domain.Enums;

namespace InvoiceFlow.Domain.Entities
{
    public class InvoiceAuditEntry
    {
         public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public InvoiceStatus? FromStatus { get; set; }
    public InvoiceStatus ToStatus { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAtUtc { get; set; }
    public string? Comment { get; set; }
    }
}