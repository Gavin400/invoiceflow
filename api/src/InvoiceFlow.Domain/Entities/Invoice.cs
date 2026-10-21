using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoiceFlow.Domain.Enums;

namespace InvoiceFlow.Domain.Entities
{
    public class Invoice
    {
        public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateOnly IssueDate { get; set; }
    public DateOnly DueDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<InvoiceAuditEntry> AuditTrail { get; set; } = new List<InvoiceAuditEntry>();
    }
}