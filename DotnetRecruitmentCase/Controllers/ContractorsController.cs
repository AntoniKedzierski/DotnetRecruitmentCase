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
public class ContractorsController : ControllerBase {

    private readonly InvoicesDbContext _db;
    private readonly IMapper _mapper;

    public ContractorsController(InvoicesDbContext db, IMapper mapper) {
        _db     = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ContractorDto>>> GetAll() {
        var contractors = await _db.Contractors.ToListAsync();
        return contractors.ToDtos<ContractorDto>(_mapper).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContractorDto>> GetById(Guid id) {
        var contractor = await _db.Contractors.FindAsync(id);
        if (contractor is null) return NotFound();
        return contractor.ToDto<ContractorDto>(_mapper);
    }

    [HttpPost]
    public async Task<ActionResult<ContractorDto>> Create(CreateContractorRequest request) {
        var contractor = new Contractor {
            Id             = Guid.NewGuid(),
            ContractorId   = request.ContractorId,
            ContractorName = request.ContractorName,
            Address        = request.Address,
            Country        = request.Country,
            PostCode       = request.PostCode
        };
        _db.Contractors.Add(contractor);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = contractor.Id }, contractor.ToDto<ContractorDto>(_mapper));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ContractorDto>> Update(Guid id, UpdateContractorRequest request) {
        var contractor = await _db.Contractors.FindAsync(id);
        if (contractor is null) return NotFound();
        contractor.ContractorId   = request.ContractorId;
        contractor.ContractorName = request.ContractorName;
        contractor.Address        = request.Address;
        contractor.Country        = request.Country;
        contractor.PostCode       = request.PostCode;
        await _db.SaveChangesAsync();
        return contractor.ToDto<ContractorDto>(_mapper);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) {
        var contractor = await _db.Contractors.FindAsync(id);
        if (contractor is null) return NotFound();
        _db.Contractors.Remove(contractor);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
