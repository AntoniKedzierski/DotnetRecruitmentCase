using System;
using System.Collections.Generic;
using System.Text;

namespace CommonObjects.Invoices;

public class ContractorDto : AbstractDto {

    public Guid Id { get; set; }

    public int ContractorId { get; set; }

    public string ContractorName { get; set; }

    public string Address { get; set; }

    public string Country { get; set; }

    public string PostCode { get; set; }

}