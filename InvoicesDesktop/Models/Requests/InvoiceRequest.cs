using System.Collections.Generic;

namespace InvoicesDesktop.Models.Requests;

public class InvoiceRequest {

    public Guid ContractorId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    public DateTime InvoiceDate { get; set; }

    public DateTime DueDate { get; set; }

    public double VatRate { get; set; }

    public string Currency { get; set; } = string.Empty;

    public bool Paid { get; set; }

    public List<InvoicePositionRequest> Positions { get; set; } = [];
}
