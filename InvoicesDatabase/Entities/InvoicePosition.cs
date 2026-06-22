namespace InvoicesDatabase.Entities;

public class InvoicePosition : AbstractEntity {

    public Guid Id { get; set; }

    public Guid InvoiceId { get; set; }

    public Invoice Invoice { get; set; } = null!;

    public Guid ItemId { get; set; }

    public Item Item { get; set; } = null!;

    public double Quantity { get; set; }

    public double UnitPrice { get; set; }

    public double Discount { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
