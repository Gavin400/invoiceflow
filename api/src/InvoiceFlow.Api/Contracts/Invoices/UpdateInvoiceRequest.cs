using System.ComponentModel.DataAnnotations;

namespace InvoiceFlow.Api.Contracts.Invoices;

public record UpdateInvoiceRequest : IValidatableObject
{
    [Required, MaxLength(50)]
    public string InvoiceNumber { get; init; } = string.Empty;

    [Required, Range(0.01, 1_000_000_000)]
    public decimal? Amount { get; init; }

    [Required, StringLength(3, MinimumLength = 3)]
    public string Currency { get; init; } = "EUR";

    [Required]
    public DateOnly? IssueDate { get; init; }

    [Required]
    public DateOnly? DueDate { get; init; }

    [Required, MaxLength(20)]
    public string CostCenter { get; init; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DueDate < IssueDate)
        {
            yield return new ValidationResult(
                "Due date cannot be before the issue date.",
                [nameof(DueDate)]);
        }
    }
}