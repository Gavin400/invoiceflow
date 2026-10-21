using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoiceFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceFlow.Infrastructure.Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
         public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber).HasMaxLength(50).IsRequired();
        builder.Property(i => i.Amount).HasPrecision(18, 2);
        builder.Property(i => i.Currency).HasMaxLength(3).IsFixedLength().IsRequired();
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(i => i.CostCenter).HasMaxLength(20).IsRequired();

        // The same supplier can't send the same invoice number twice (duplicate invoice protection)
        builder.HasIndex(i => new { i.SupplierId, i.InvoiceNumber }).IsUnique();

        builder.HasOne(i => i.Supplier)
               .WithMany(s => s.Invoices)
               .HasForeignKey(i => i.SupplierId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.AuditTrail)
               .WithOne()
               .HasForeignKey(a => a.InvoiceId)
               .OnDelete(DeleteBehavior.Restrict);
    }
        
    }
}