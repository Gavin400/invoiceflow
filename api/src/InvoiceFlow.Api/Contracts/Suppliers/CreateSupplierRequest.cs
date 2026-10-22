using System.ComponentModel.DataAnnotations;

namespace InvoiceFlow.Api.Contracts.Suppliers;

public record CreateSupplierRequest
{
    [Required, MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required, MaxLength(30)]
    public string VatNumber { get; init; } = string.Empty;

    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; init; } = string.Empty;
}