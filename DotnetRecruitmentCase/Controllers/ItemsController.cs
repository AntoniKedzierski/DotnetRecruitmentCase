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
public class ItemsController : ControllerBase {

    private readonly InvoicesDbContext _db;
    private readonly IMapper _mapper;

    public ItemsController(InvoicesDbContext db, IMapper mapper) {
        _db     = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ItemDto>>> GetAll() {
        var items = await _db.Items.ToListAsync();
        return items.ToDtos<ItemDto>(_mapper).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ItemDto>> GetById(Guid id) {
        var item = await _db.Items.FindAsync(id);
        if (item is null) return NotFound();
        return item.ToDto<ItemDto>(_mapper);
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> Create(CreateItemRequest request) {
        var item = new Item {
            Id   = Guid.NewGuid(),
            Name = request.Name
        };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item.ToDto<ItemDto>(_mapper));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ItemDto>> Update(Guid id, UpdateItemRequest request) {
        var item = await _db.Items.FindAsync(id);
        if (item is null) return NotFound();
        item.Name = request.Name;
        await _db.SaveChangesAsync();
        return item.ToDto<ItemDto>(_mapper);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) {
        var item = await _db.Items.FindAsync(id);
        if (item is null) return NotFound();
        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
