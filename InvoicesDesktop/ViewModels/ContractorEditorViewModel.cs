using InvoicesDesktop.Models;
using InvoicesDesktop.Models.Requests;
using InvoicesDesktop.Mvvm;

namespace InvoicesDesktop.ViewModels;

public class ContractorEditorViewModel : ObservableObject {

    private int _contractorId;
    private string _contractorName = string.Empty;
    private string _address = string.Empty;
    private string _country = string.Empty;
    private string _postCode = string.Empty;

    public ContractorEditorViewModel() {
        IsNew = true;
    }

    public ContractorEditorViewModel(ContractorModel contractor) {
        IsNew           = false;
        _contractorId   = contractor.ContractorId;
        _contractorName = contractor.ContractorName;
        _address        = contractor.Address;
        _country        = contractor.Country;
        _postCode       = contractor.PostCode;
    }

    public bool IsNew { get; }

    public string Title => IsNew ? "New contractor" : "Edit contractor";

    public int ContractorId {
        get => _contractorId;
        set => SetProperty(ref _contractorId, value);
    }

    public string ContractorName {
        get => _contractorName;
        set {
            if (SetProperty(ref _contractorName, value)) {
                OnPropertyChanged(nameof(CanSave));
            }
        }
    }

    public string Address {
        get => _address;
        set => SetProperty(ref _address, value);
    }

    public string Country {
        get => _country;
        set => SetProperty(ref _country, value);
    }

    public string PostCode {
        get => _postCode;
        set => SetProperty(ref _postCode, value);
    }

    public bool CanSave => !string.IsNullOrWhiteSpace(ContractorName);

    public ContractorRequest ToRequest() => new() {
        ContractorId   = ContractorId,
        ContractorName = ContractorName,
        Address        = Address,
        Country        = Country,
        PostCode       = PostCode
    };
}
