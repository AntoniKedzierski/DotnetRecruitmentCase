using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using InvoicesDesktop.Models;
using InvoicesDesktop.Mvvm;
using InvoicesDesktop.Services;
using InvoicesDesktop.Views.Dialogs;
using MaterialDesignThemes.Wpf;

namespace InvoicesDesktop.ViewModels;

public class InvoicesViewModel : SectionViewModel {

    private InvoiceModel? _selectedInvoice;

    public InvoicesViewModel(InvoicesApiClient api, ISnackbarMessageQueue messageQueue)
        : base(api, messageQueue) {
        CreateCommand = new AsyncRelayCommand(CreateAsync, () => !IsBusy);
        EditCommand   = new AsyncRelayCommand(p => EditAsync(p as InvoiceModel), p => !IsBusy && p is InvoiceModel);
        DeleteCommand = new AsyncRelayCommand(p => DeleteAsync(p as InvoiceModel), p => !IsBusy && p is InvoiceModel);
    }

    public override string Title => "Invoices";

    public override PackIconKind Icon => PackIconKind.FileDocumentMultiple;

    public ObservableCollection<InvoiceModel> Invoices { get; } = [];

    public InvoiceModel? SelectedInvoice {
        get => _selectedInvoice;
        set => SetProperty(ref _selectedInvoice, value);
    }

    public ICommand CreateCommand { get; }

    public ICommand EditCommand { get; }

    public ICommand DeleteCommand { get; }

    public override async Task LoadAsync() {
        IsBusy = true;
        try {
            var invoices = await Api.GetInvoicesAsync();
            Invoices.Clear();
            foreach (var invoice in invoices) {
                Invoices.Add(invoice);
            }
        }
        catch (Exception ex) {
            Notify($"Failed to load invoices: {ex.Message}");
        }
        finally {
            IsBusy = false;
        }
    }

    private async Task CreateAsync() {
        var (contractors, items) = await LoadEditorDependenciesAsync();
        if (contractors is null || items is null) {
            return;
        }

        if (contractors.Count == 0 || items.Count == 0) {
            Notify("Add at least one contractor and one sale item first.");
            return;
        }

        var editor = new InvoiceEditorViewModel(contractors, items);
        var result = await DialogHost.Show(new InvoiceEditorDialog { DataContext = editor }, RootDialogIdentifier);
        if (result is not true) {
            return;
        }

        IsBusy = true;
        try {
            var created = await Api.CreateInvoiceAsync(editor.ToRequest());
            if (created is not null) {
                Invoices.Add(created);
            }
            Notify("Invoice created.");
        }
        catch (Exception ex) {
            Notify($"Failed to create invoice: {ex.Message}");
        }
        finally {
            IsBusy = false;
        }
    }

    private async Task EditAsync(InvoiceModel? invoice) {
        if (invoice is null) {
            return;
        }

        var (contractors, items) = await LoadEditorDependenciesAsync();
        if (contractors is null || items is null) {
            return;
        }

        var editor = new InvoiceEditorViewModel(contractors, items, invoice);
        var result = await DialogHost.Show(new InvoiceEditorDialog { DataContext = editor }, RootDialogIdentifier);
        if (result is not true) {
            return;
        }

        IsBusy = true;
        try {
            var updated = await Api.UpdateInvoiceAsync(invoice.Id, editor.ToRequest());
            if (updated is not null) {
                var index = Invoices.IndexOf(invoice);
                if (index >= 0) {
                    Invoices[index] = updated;
                }
            }
            Notify("Invoice updated.");
        }
        catch (Exception ex) {
            Notify($"Failed to update invoice: {ex.Message}");
        }
        finally {
            IsBusy = false;
        }
    }

    private async Task DeleteAsync(InvoiceModel? invoice) {
        if (invoice is null) {
            return;
        }

        var confirm = new ConfirmDialogViewModel(
            "Delete invoice",
            $"Delete invoice \"{invoice.InvoiceNumber}\"? This action cannot be undone.");
        var result = await DialogHost.Show(new ConfirmDialog { DataContext = confirm }, RootDialogIdentifier);
        if (result is not true) {
            return;
        }

        IsBusy = true;
        try {
            await Api.DeleteInvoiceAsync(invoice.Id);
            Invoices.Remove(invoice);
            Notify("Invoice deleted.");
        }
        catch (Exception ex) {
            Notify($"Failed to delete invoice: {ex.Message}");
        }
        finally {
            IsBusy = false;
        }
    }

    private async Task<(List<ContractorModel>? Contractors, List<ItemModel>? Items)> LoadEditorDependenciesAsync() {
        IsBusy = true;
        try {
            var contractors = await Api.GetContractorsAsync();
            var items       = await Api.GetItemsAsync();
            return (contractors, items);
        }
        catch (Exception ex) {
            Notify($"Failed to load invoice form data: {ex.Message}");
            return (null, null);
        }
        finally {
            IsBusy = false;
        }
    }
}
