using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using InvoicesDesktop.Models;
using InvoicesDesktop.Models.Requests;
using InvoicesDesktop.Mvvm;

namespace InvoicesDesktop.ViewModels;

public class InvoiceEditorViewModel : ObservableObject {

    private ContractorModel? _selectedContractor;
    private string _invoiceNumber = string.Empty;
    private string _description = string.Empty;
    private DateTime _saleDate = DateTime.Today;
    private DateTime _invoiceDate = DateTime.Today;
    private DateTime _dueDate = DateTime.Today.AddDays(14);
    private double _vatRate = 0.23;
    private string _currency = "PLN";
    private bool _paid;

    public InvoiceEditorViewModel(IReadOnlyList<ContractorModel> contractors, IReadOnlyList<ItemModel> items) {
        IsNew        = true;
        Contractors  = contractors;
        Items        = items;
        _selectedContractor = contractors.FirstOrDefault();

        Positions.CollectionChanged += OnPositionsChanged;
        AddPositionCommand    = new RelayCommand(AddPosition);
        RemovePositionCommand = new RelayCommand(
            p => RemovePosition(p as InvoicePositionEditorViewModel),
            p => p is InvoicePositionEditorViewModel);

        AddPosition();
    }

    public InvoiceEditorViewModel(
        IReadOnlyList<ContractorModel> contractors,
        IReadOnlyList<ItemModel> items,
        InvoiceModel invoice) {
        IsNew         = false;
        Contractors   = contractors;
        Items         = items;
        _invoiceNumber = invoice.InvoiceNumber;
        _description   = invoice.Description;
        _saleDate      = invoice.SaleDate;
        _invoiceDate   = invoice.InvoiceDate;
        _dueDate       = invoice.DueDate;
        _vatRate       = invoice.VatRate;
        _currency      = invoice.Currency;
        _paid          = invoice.Paid;
        _selectedContractor = contractors.FirstOrDefault(c => c.Id == invoice.Contractor?.Id);

        Positions.CollectionChanged += OnPositionsChanged;
        AddPositionCommand    = new RelayCommand(AddPosition);
        RemovePositionCommand = new RelayCommand(
            p => RemovePosition(p as InvoicePositionEditorViewModel),
            p => p is InvoicePositionEditorViewModel);

        foreach (var position in invoice.Positions) {
            Positions.Add(new InvoicePositionEditorViewModel(items, position));
        }

        if (Positions.Count == 0) {
            AddPosition();
        }
    }

    public bool IsNew { get; }

    public string Title => IsNew ? "New invoice" : "Edit invoice";

    public IReadOnlyList<ContractorModel> Contractors { get; }

    public IReadOnlyList<ItemModel> Items { get; }

    public ObservableCollection<InvoicePositionEditorViewModel> Positions { get; } = [];

    public ICommand AddPositionCommand { get; }

    public ICommand RemovePositionCommand { get; }

    public ContractorModel? SelectedContractor {
        get => _selectedContractor;
        set {
            if (SetProperty(ref _selectedContractor, value)) {
                OnPropertyChanged(nameof(CanSave));
            }
        }
    }

    public string InvoiceNumber {
        get => _invoiceNumber;
        set {
            if (SetProperty(ref _invoiceNumber, value)) {
                OnPropertyChanged(nameof(CanSave));
            }
        }
    }

    public string Description {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public DateTime SaleDate {
        get => _saleDate;
        set => SetProperty(ref _saleDate, value);
    }

    public DateTime InvoiceDate {
        get => _invoiceDate;
        set => SetProperty(ref _invoiceDate, value);
    }

    public DateTime DueDate {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    public double VatRate {
        get => _vatRate;
        set {
            if (SetProperty(ref _vatRate, value)) {
                OnPropertyChanged(nameof(GrossValue));
            }
        }
    }

    public string Currency {
        get => _currency;
        set {
            if (SetProperty(ref _currency, value)) {
                OnPropertyChanged(nameof(CanSave));
            }
        }
    }

    public bool Paid {
        get => _paid;
        set => SetProperty(ref _paid, value);
    }

    public double NetValue => Math.Round(Positions.Sum(p => p.LineNet), 2);

    public double GrossValue => Math.Round(NetValue * (1 + VatRate), 2);

    public bool CanSave =>
        SelectedContractor is not null
        && !string.IsNullOrWhiteSpace(InvoiceNumber)
        && !string.IsNullOrWhiteSpace(Currency)
        && Positions.Count > 0
        && Positions.All(p => p.SelectedItem is not null);

    public InvoiceRequest ToRequest() => new() {
        ContractorId  = SelectedContractor!.Id,
        InvoiceNumber = InvoiceNumber,
        Description   = Description,
        SaleDate      = SaleDate,
        InvoiceDate   = InvoiceDate,
        DueDate       = DueDate,
        VatRate       = VatRate,
        Currency      = Currency,
        Paid          = Paid,
        Positions     = Positions.Select(p => new InvoicePositionRequest {
            ItemId        = p.SelectedItem!.Id,
            Quantity      = p.Quantity,
            UnitPrice     = p.UnitPrice,
            Discount      = p.Discount,
            AccountNumber = p.AccountNumber,
            Description   = p.Description
        }).ToList()
    };

    private void AddPosition() {
        var position = new InvoicePositionEditorViewModel(Items);
        Positions.Add(position);
    }

    private void RemovePosition(InvoicePositionEditorViewModel? position) {
        if (position is not null) {
            Positions.Remove(position);
        }
    }

    private void OnPositionsChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.OldItems is not null) {
            foreach (InvoicePositionEditorViewModel position in e.OldItems) {
                position.PropertyChanged -= OnPositionPropertyChanged;
            }
        }

        if (e.NewItems is not null) {
            foreach (InvoicePositionEditorViewModel position in e.NewItems) {
                position.PropertyChanged += OnPositionPropertyChanged;
            }
        }

        RaiseTotals();
    }

    private void OnPositionPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(InvoicePositionEditorViewModel.LineNet)
            or nameof(InvoicePositionEditorViewModel.SelectedItem)) {
            RaiseTotals();
        }
    }

    private void RaiseTotals() {
        OnPropertyChanged(nameof(NetValue));
        OnPropertyChanged(nameof(GrossValue));
        OnPropertyChanged(nameof(CanSave));
    }
}
