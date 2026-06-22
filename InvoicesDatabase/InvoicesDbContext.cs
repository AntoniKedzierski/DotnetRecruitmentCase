using InvoicesDatabase.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoicesDatabase;

public class InvoicesDbContext : DbContext {

    public InvoicesDbContext(DbContextOptions<InvoicesDbContext> options) : base(options) { }

    public DbSet<Contractor> Contractors => Set<Contractor>();

    public DbSet<Item> Items => Set<Item>();

    public DbSet<Invoice> Invoices => Set<Invoice>();

    public DbSet<InvoicePosition> InvoicePositions => Set<InvoicePosition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Contractor>(entity => {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.ContractorName).IsRequired();
            entity.HasMany(c => c.Invoices)
                  .WithOne(i => i.Contractor)
                  .HasForeignKey(i => i.ContractorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Item>(entity => {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Name).IsRequired();
        });

        modelBuilder.Entity<Invoice>(entity => {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.InvoiceNumber).IsRequired();
            entity.Property(i => i.Currency).IsRequired();
            entity.HasMany(i => i.Positions)
                  .WithOne(p => p.Invoice)
                  .HasForeignKey(p => p.InvoiceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InvoicePosition>(entity => {
            entity.HasKey(p => p.Id);
            entity.HasOne(p => p.Item)
                  .WithMany(i => i.Positions)
                  .HasForeignKey(p => p.ItemId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
