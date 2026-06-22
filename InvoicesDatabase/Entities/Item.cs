namespace InvoicesDatabase.Entities;

public class Item : AbstractEntity {

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<InvoicePosition> Positions { get; set; } = [];
}
