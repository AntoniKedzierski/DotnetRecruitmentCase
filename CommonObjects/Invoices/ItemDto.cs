using System;
using System.Collections.Generic;
using System.Text;

namespace CommonObjects.Invoices; 

public class ItemDto : AbstractDto {

    public Guid Id { get; set; }

    public string Name { get; set; }
}
