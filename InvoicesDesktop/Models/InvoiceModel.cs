using System.Collections.Generic;

namespace InvoicesDesktop.Models;

public class InvoiceModel {

    public Guid Id { get; set; }

    public ContractorModel? Contractor { get; set; }

    public List<InvoicePositionModel> Positions { get; set; } = [];

    public string InvoiceNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    public DateTime InvoiceDate { get; set; }

    public DateTime DueDate { get; set; }

    public double VatRate { get; set; }

    public double NetValue { get; set; }

    public string Currency { get; set; } = string.Empty;

    public bool Paid { get; set; }

    public double GrossValue => Math.Round(NetValue * (1 + VatRate), 2);
}
