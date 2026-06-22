using System.Collections.Generic;
using InvoicesDesktop.Models;
using InvoicesDesktop.Mvvm;

namespace InvoicesDesktop.ViewModels;

/// <summary>
/// Editable row representing a single invoice position inside the invoice editor.
/// </summary>
public class InvoicePositionEditorViewModel : ObservableObject {

    private ItemModel? _selectedItem;
    private double _quantity = 1;
    private double _unitPrice;
    private double _discount;
    private string _accountNumber = string.Empty;
    private string _description = string.Empty;

    public InvoicePositionEditorViewModel(IReadOnlyList<ItemModel> items) {
        AvailableItems = items;
    }

    public InvoicePositionEditorViewModel(IReadOnlyList<ItemModel> items, InvoicePositionModel position)
        : this(items) {
        _quantity      = position.Quantity;
        _unitPrice     = position.UnitPrice;
        _discount      = position.Discount;
        _accountNumber = position.AccountNumber;
        _description   = position.Description;

        if (position.Item is not null) {
            foreach (var candidate in items) {
                if (candidate.Id == position.Item.Id) {
                    _selectedItem = candidate;
                    break;
                }
            }
        }
    }

    public IReadOnlyList<ItemModel> AvailableItems { get; }

    public ItemModel? SelectedItem {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public double Quantity {
        get => _quantity;
        set {
            if (SetProperty(ref _quantity, value)) {
                OnPropertyChanged(nameof(LineNet));
            }
        }
    }

    public double UnitPrice {
        get => _unitPrice;
        set {
            if (SetProperty(ref _unitPrice, value)) {
                OnPropertyChanged(nameof(LineNet));
            }
        }
    }

    public double Discount {
        get => _discount;
        set {
            if (SetProperty(ref _discount, value)) {
                OnPropertyChanged(nameof(LineNet));
            }
        }
    }

    public string AccountNumber {
        get => _accountNumber;
        set => SetProperty(ref _accountNumber, value);
    }

    public string Description {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public double LineNet => Math.Round(Quantity * UnitPrice * (1 - Discount), 2);
}
