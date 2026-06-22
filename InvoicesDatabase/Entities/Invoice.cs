namespace InvoicesDatabase.Entities;

public class Invoice : AbstractEntity {

    public Guid Id { get; set; }

    public Guid ContractorId { get; set; }

    public Contractor Contractor { get; set; } = null!;

    public string InvoiceNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    public DateTime InvoiceDate { get; set; }

    public DateTime DueDate { get; set; }

    public double VatRate { get; set; }

    public double NetValue { get; set; }

    public string Currency { get; set; } = string.Empty;

    public bool Paid { get; set; }

    public ICollection<InvoicePosition> Positions { get; set; } = [];
}
