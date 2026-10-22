using System.Linq.Expressions;
using InvoiceFlow.Api.Contracts.Invoices;
using InvoiceFlow.Domain.Entities;
using InvoiceFlow.Domain.Enums;
using InvoiceFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceFlow.Api.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoicesController(AppDbContext db) : ControllerBase
{
    private static readonly Expression<Func<Invoice, InvoiceResponse>> ToResponse = i =>
        new InvoiceResponse(
            i.Id, i.InvoiceNumber, i.SupplierId, i.Supplier.Name,
            i.Amount, i.Currency, i.IssueDate, i.DueDate,
            i.CostCenter, i.Status, i.CreatedAtUtc, i.UpdatedAtUtc);



    [HttpGet]
public async Task<ActionResult<IReadOnlyList<InvoiceResponse>>> GetAll(
    [FromQuery] InvoiceStatus? status,
    CancellationToken ct)
{
    var query = db.Invoices.AsNoTracking();

    if (status is not null)
    {
        query = query.Where(i => i.Status == status);
    }

    var invoices = await query
        .OrderByDescending(i => i.CreatedAtUtc)
        .Select(ToResponse)
        .ToListAsync(ct);

    return Ok(invoices);
}


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InvoiceResponse>> GetById(Guid id, CancellationToken ct)
    {
        var invoice = await FindResponseAsync(id, ct);
        return invoice is null ? NotFound() : Ok(invoice);
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceResponse>> Create(CreateInvoiceRequest request, CancellationToken ct)
    {
        var supplierId = request.SupplierId!.Value;
        var invoiceNumber = request.InvoiceNumber.Trim();

        if (!await db.Suppliers.AnyAsync(s => s.Id == supplierId, ct))
        {
            ModelState.AddModelError(nameof(request.SupplierId), "Supplier does not exist.");
            return ValidationProblem(ModelState);
        }

        if (await db.Invoices.AnyAsync(i => i.SupplierId == supplierId && i.InvoiceNumber == invoiceNumber, ct))
        {
            return DuplicateInvoice(invoiceNumber);
        }

        var now = DateTime.UtcNow;

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            SupplierId = supplierId,
            InvoiceNumber = invoiceNumber,
            Amount = request.Amount!.Value,
            Currency = request.Currency.Trim().ToUpperInvariant(),
            IssueDate = request.IssueDate!.Value,
            DueDate = request.DueDate!.Value,
            CostCenter = request.CostCenter.Trim(),
            Status = InvoiceStatus.Draft,
            CreatedAtUtc = now
        };

        invoice.AuditTrail.Add(new InvoiceAuditEntry
        {
            Id = Guid.NewGuid(),
            FromStatus = null,
            ToStatus = InvoiceStatus.Draft,
            ChangedBy = "system",
            ChangedAtUtc = now,
            Comment = "Invoice created"
        });

        db.Invoices.Add(invoice);
        await db.SaveChangesAsync(ct);

        var response = await FindResponseAsync(invoice.Id, ct);
        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<InvoiceResponse>> Update(Guid id, UpdateInvoiceRequest request, CancellationToken ct)
    {
        var invoice = await db.Invoices.FirstOrDefaultAsync(i => i.Id == id, ct);

        if (invoice is null)
        {
            return NotFound();
        }

        if (invoice.Status != InvoiceStatus.Draft)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Invoice cannot be edited",
                detail: $"Only draft invoices can be edited. This invoice is {invoice.Status}.");
        }

        var invoiceNumber = request.InvoiceNumber.Trim();

        if (await db.Invoices.AnyAsync(i => i.Id != id
                                          && i.SupplierId == invoice.SupplierId
                                          && i.InvoiceNumber == invoiceNumber, ct))
        {
            return DuplicateInvoice(invoiceNumber);
        }

        invoice.InvoiceNumber = invoiceNumber;
        invoice.Amount = request.Amount!.Value;
        invoice.Currency = request.Currency.Trim().ToUpperInvariant();
        invoice.IssueDate = request.IssueDate!.Value;
        invoice.DueDate = request.DueDate!.Value;
        invoice.CostCenter = request.CostCenter.Trim();
        invoice.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return Ok(await FindResponseAsync(id, ct));
    }

    private Task<InvoiceResponse?> FindResponseAsync(Guid id, CancellationToken ct) =>
        db.Invoices
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(ToResponse)
            .FirstOrDefaultAsync(ct);

    private ObjectResult DuplicateInvoice(string invoiceNumber) =>
        Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Duplicate invoice",
            detail: $"This supplier has already sent invoice {invoiceNumber}.");
}