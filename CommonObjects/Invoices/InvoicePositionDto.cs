using System;
using System.Collections.Generic;
using System.Text;

namespace CommonObjects.Invoices; 

public class InvoicePositionDto : AbstractDto {

    public ItemDto Item { get; set; }

    public double Quantity { get; set; }

    public double UnitPrice { get; set; }

    public double Discount { get; set; }

    public string AccountNumber { get; set; }

    public string Description { get; set; }

}
