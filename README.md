# InvoiceFlow

An invoice approval portal for accounts payable teams. Suppliers' invoices move through a
Draft → Submitted → Approved/Rejected → Paid workflow, with role-based access and a full audit
trail of who changed what and when.

**Status:** 🚧 In progress — being built over 7 days.

## Tech stack
- **Backend:** ASP.NET Core (.NET 10), Entity Framework Core, SQL Server
- **Frontend:** Angular (coming soon)
- **Architecture:** Clean Architecture (Domain / Infrastructure / API)

## Done so far
- Domain model for suppliers, invoices and audit entries
- EF Core configuration with SQL Server migrations
- Duplicate invoice protection (unique supplier + invoice number)

## Run locally
```bash
cd api
dotnet ef database update -p src/InvoiceFlow.Infrastructure -s src/InvoiceFlow.Api
dotnet run --project src/InvoiceFlow.Api
```
Requires the .NET 10 SDK and a local SQL Server instance (see `appsettings.Development.json`).