using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceFlow.Domain.Entities
{
    public class Supplier
    {
        public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string VatNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}