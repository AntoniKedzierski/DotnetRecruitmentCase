using InvoicesDesktop.Models;
using InvoicesDesktop.Mvvm;

namespace InvoicesDesktop.ViewModels;

public class ItemEditorViewModel : ObservableObject {

    private string _name = string.Empty;

    public ItemEditorViewModel() {
        IsNew = true;
    }

    public ItemEditorViewModel(ItemModel item) {
        IsNew = false;
        _name = item.Name;
    }

    public bool IsNew { get; }

    public string Title => IsNew ? "New sale item" : "Edit sale item";

    public string Name {
        get => _name;
        set {
            if (SetProperty(ref _name, value)) {
                OnPropertyChanged(nameof(CanSave));
            }
        }
    }

    public bool CanSave => !string.IsNullOrWhiteSpace(Name);
}
