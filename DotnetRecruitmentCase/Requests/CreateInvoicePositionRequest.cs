namespace InvoicesApi.Requests;

public class CreateInvoicePositionRequest {

    public Guid ItemId { get; set; }

    public double Quantity { get; set; }

    public double UnitPrice { get; set; }

    public double Discount { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
