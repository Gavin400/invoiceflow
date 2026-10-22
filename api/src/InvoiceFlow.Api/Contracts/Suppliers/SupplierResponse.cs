namespace InvoiceFlow.Api.Contracts.Suppliers;

public record SupplierResponse(
    Guid Id,
    string Name,
    string VatNumber,
    string Email,
    DateTime CreatedAtUtc);