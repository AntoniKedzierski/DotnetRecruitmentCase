namespace InvoicesDesktop.Models;

public class InvoicePositionModel {

    public ItemModel? Item { get; set; }

    public double Quantity { get; set; }

    public double UnitPrice { get; set; }

    public double Discount { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public double LineNet => Math.Round(Quantity * UnitPrice * (1 - Discount), 2);
}
