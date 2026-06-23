namespace InvoicesApi.Requests;

public class CreateContractorRequest {

    public int ContractorId { get; set; }

    public string ContractorName { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string PostCode { get; set; } = string.Empty;
}
