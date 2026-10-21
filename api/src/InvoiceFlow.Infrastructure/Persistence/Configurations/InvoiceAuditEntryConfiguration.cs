using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoiceFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceFlow.Infrastructure.Persistence.Configurations
{
    public class InvoiceAuditEntryConfiguration : IEntityTypeConfiguration<InvoiceAuditEntry>
    {
        public void Configure(EntityTypeBuilder<InvoiceAuditEntry> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FromStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.ToStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.ChangedBy).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Comment).HasMaxLength(500);

        builder.HasIndex(a => a.InvoiceId);
    }
    }
}