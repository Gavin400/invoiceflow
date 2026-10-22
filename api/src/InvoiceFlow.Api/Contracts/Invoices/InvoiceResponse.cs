using InvoiceFlow.Domain.Enums;

namespace InvoiceFlow.Api.Contracts.Invoices;

public record InvoiceResponse(
    Guid Id,
    string InvoiceNumber,
    Guid SupplierId,
    string SupplierName,
    decimal Amount,
    string Currency,
    DateOnly IssueDate,
    DateOnly DueDate,
    string CostCenter,
    InvoiceStatus Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);