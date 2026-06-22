namespace CommonObjects.Invoices;

public class InvoiceDto : AbstractDto {

    public Guid Id { get; set; }

    public ContractorDto Contractor { get; set; }

    public List<InvoicePositionDto> Positions { get; set; }

    public string InvoiceNumber { get; set; }

    public string Description { get; set; }

    public DateTime SaleDate { get; set; }

    public DateTime InvoiceDate { get; set; }

    public DateTime DueDate { get; set; }

    public double VatRate { get; set; }

    public double NetValue { get; set; }

    public string Currency { get; set; }

    public bool Paid { get; set; }
}