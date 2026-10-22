using InvoiceFlow.Api.Contracts.Suppliers;
using InvoiceFlow.Domain.Entities;
using InvoiceFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceFlow.Api.Controllers;

[ApiController]
[Route("api/suppliers")]
public class SuppliersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SupplierResponse>>> GetAll(CancellationToken ct)
    {
        var suppliers = await db.Suppliers
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SupplierResponse(s.Id, s.Name, s.VatNumber, s.Email, s.CreatedAtUtc))
            .ToListAsync(ct);

        return Ok(suppliers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SupplierResponse>> GetById(Guid id, CancellationToken ct)
    {
        var supplier = await db.Suppliers
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SupplierResponse(s.Id, s.Name, s.VatNumber, s.Email, s.CreatedAtUtc))
            .FirstOrDefaultAsync(ct);

        return supplier is null ? NotFound() : Ok(supplier);
    }

    [HttpPost]
    public async Task<ActionResult<SupplierResponse>> Create(CreateSupplierRequest request, CancellationToken ct)
    {
        var vatNumber = request.VatNumber.Trim().ToUpperInvariant();

        if (await db.Suppliers.AnyAsync(s => s.VatNumber == vatNumber, ct))
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Supplier already exists",
                detail: $"A supplier with VAT number {vatNumber} is already registered.");
        }

        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            VatNumber = vatNumber,
            Email = request.Email.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Suppliers.Add(supplier);
        await db.SaveChangesAsync(ct);

        var response = new SupplierResponse(
            supplier.Id, supplier.Name, supplier.VatNumber, supplier.Email, supplier.CreatedAtUtc);

        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, response);
    }
}