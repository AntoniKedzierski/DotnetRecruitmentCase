using AutoMapper;
using CommonObjects.Invoices;
using InvoicesApi.Mappers;
using InvoicesApi.Requests;
using InvoicesDatabase;
using InvoicesDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoicesApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InvoicesController(InvoicesDbContext db, IMapper mapper) : ControllerBase {

    private readonly InvoicesDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<ActionResult<List<InvoiceDto>>> GetAll() {
        var invoices = await _db.Invoices
            .Include(i => i.Contractor)
            .Include(i => i.Positions).ThenInclude(p => p.Item)
            .ToListAsync();
        return invoices.ToDtos<InvoiceDto>(_mapper).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InvoiceDto>> GetById(Guid id) {
        var invoice = await _db.Invoices
            .Include(i => i.Contractor)
            .Include(i => i.Positions).ThenInclude(p => p.Item)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice is null) {
            return NotFound();
        }

        return invoice.ToDto<InvoiceDto>(_mapper);
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceDto>> Create(CreateInvoiceRequest request) {
        var positions = request.Positions.Select(p => new InvoicePosition {
            Id            = Guid.NewGuid(),
            ItemId        = p.ItemId,
            Quantity      = p.Quantity,
            UnitPrice     = p.UnitPrice,
            Discount      = p.Discount,
            AccountNumber = p.AccountNumber,
            Description   = p.Description
        }).ToList();

        var invoice = new Invoice {
            Id            = Guid.NewGuid(),
            ContractorId  = request.ContractorId,
            InvoiceNumber = request.InvoiceNumber,
            Description   = request.Description,
            SaleDate      = request.SaleDate,
            InvoiceDate   = request.InvoiceDate,
            DueDate       = request.DueDate,
            VatRate       = request.VatRate,
            NetValue      = ComputeNetValue(positions),
            Currency      = request.Currency,
            Paid          = request.Paid,
            Positions     = positions
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        await _db.Entry(invoice).Reference(i => i.Contractor).LoadAsync();
        foreach (var pos in invoice.Positions) {
            await _db.Entry(pos).Reference(p => p.Item).LoadAsync();
        }

        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice.ToDto<InvoiceDto>(_mapper));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<InvoiceDto>> Update(Guid id, UpdateInvoiceRequest request) {
        var invoice = await _db.Invoices
            .Include(i => i.Contractor)
            .Include(i => i.Positions).ThenInclude(p => p.Item)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice is null) {
            return NotFound();
        }

        _db.InvoicePositions.RemoveRange(invoice.Positions);

        var positions = request.Positions.Select(p => new InvoicePosition {
            Id = Guid.NewGuid(),
            InvoiceId = id,
            ItemId = p.ItemId,
            Quantity = p.Quantity,
            UnitPrice = p.UnitPrice,
            Discount = p.Discount,
            AccountNumber = p.AccountNumber,
            Description = p.Description
        }).ToList();

        invoice.ContractorId = request.ContractorId;
        invoice.InvoiceNumber = request.InvoiceNumber;
        invoice.Description = request.Description;
        invoice.SaleDate = request.SaleDate;
        invoice.InvoiceDate = request.InvoiceDate;
        invoice.DueDate = request.DueDate;
        invoice.VatRate = request.VatRate;
        invoice.Currency = request.Currency;
        invoice.Paid = request.Paid;
        invoice.NetValue = ComputeNetValue(positions);
        invoice.Positions = positions;

        await _db.SaveChangesAsync();

        var contractorLoadTask = _db.Entry(invoice)
            .Reference(i => i.Contractor)
            .LoadAsync();

        var itemLoadTasks = invoice.Positions
            .Select(pos => _db.Entry(pos)
                .Reference(p => p.Item)
                .LoadAsync());

        await Task.WhenAll(itemLoadTasks.Append(contractorLoadTask));

        return invoice.ToDto<InvoiceDto>(_mapper);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) {
        var invoice = await _db.Invoices.FindAsync(id);
        if (invoice is null) {
            return NotFound();
        }

        _db.Invoices.Remove(invoice);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static double ComputeNetValue(IEnumerable<InvoicePosition> positions)
        => Math.Round(positions.Sum(p => p.Quantity * p.UnitPrice * (1 - p.Discount)), 2);
}
