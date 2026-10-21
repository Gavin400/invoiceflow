using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoiceFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceFlow.Infrastructure.Persistence.Configurations
{
    public class SupplierConfiguration :  IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.VatNumber).HasMaxLength(30).IsRequired();
        builder.Property(s => s.Email).HasMaxLength(254).IsRequired();

        builder.HasIndex(s => s.VatNumber).IsUnique();
    }
    }
}