using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using InvoicesDesktop.Models;
using InvoicesDesktop.Models.Requests;
using InvoicesDesktop.Mvvm;
using InvoicesDesktop.Services;
using InvoicesDesktop.Views.Dialogs;
using MaterialDesignThemes.Wpf;

namespace InvoicesDesktop.ViewModels;

public class ContractorsViewModel : SectionViewModel {

    private ContractorModel? _selectedContractor;

    public ContractorsViewModel(InvoicesApiClient api, ISnackbarMessageQueue messageQueue)
        : base(api, messageQueue) {
        AddCommand    = new AsyncRelayCommand(AddAsync, () => !IsBusy);
        EditCommand   = new AsyncRelayCommand(p => EditAsync(p as ContractorModel), p => !IsBusy && p is ContractorModel);
        DeleteCommand = new AsyncRelayCommand(p => DeleteAsync(p as ContractorModel), p => !IsBusy && p is ContractorModel);
    }

    public override string Title => "Contractors";

    public override PackIconKind Icon => PackIconKind.AccountGroup;

    public ObservableCollection<ContractorModel> Contractors { get; } = [];

    public ContractorModel? SelectedContractor {
        get => _selectedContractor;
        set => SetProperty(ref _selectedContractor, value);
    }

    public ICommand AddCommand { get; }

    public ICommand EditCommand { get; }

    public ICommand DeleteCommand { get; }

    public override async Task LoadAsync() {
        IsBusy = true;
        try {
            var contractors = await Api.GetContractorsAsync();
            Contractors.Clear();
            foreach (var contractor in contractors) {
                Contractors.Add(contractor);
            }
        }
        catch (Exception ex) {
            Notify($"Failed to load contractors: {ex.Message}");
        }
        finally {
            IsBusy = false;
        }
    }

    private async Task AddAsync() {
        var editor = new ContractorEditorViewModel();
        var result = await DialogHost.Show(new ContractorEditorDialog { DataContext = editor }, RootDialogIdentifier);
        if (result is not true) {
            return;
        }

        IsBusy = true;
        try {
            var created = await Api.CreateContractorAsync(editor.ToRequest());
            if (created is not null) {
                Contractors.Add(created);
            }
            Notify("Contractor created.");
        }
        catch (Exception ex) {
            Notify($"Failed to create contractor: {ex.Message}");
        }
        finally {
            IsBusy = false;
        }
    }

    private async Task EditAsync(ContractorModel? contractor) {
        if (contractor is null) {
            return;
        }

        var editor = new ContractorEditorViewModel(contractor);
        var result = await DialogHost.Show(new ContractorEditorDialog { DataContext = editor }, RootDialogIdentifier);
        if (result is not true) {
            return;
        }

        IsBusy = true;
        try {
            var updated = await Api.UpdateContractorAsync(contractor.Id, editor.ToRequest());
            if (updated is not null) {
                var index = Contractors.IndexOf(contractor);
                if (index >= 0) {
                    Contractors[index] = updated;
                }
            }
            Notify("Contractor updated.");
        }
        catch (Exception ex) {
            Notify($"Failed to update contractor: {ex.Message}");
        }
        finally {
            IsBusy = false;
        }
    }

    private async Task DeleteAsync(ContractorModel? contractor) {
        if (contractor is null) {
            return;
        }

        var confirm = new ConfirmDialogViewModel(
            "Delete contractor",
            $"Delete \"{contractor.ContractorName}\"? This action cannot be undone.");
        var result = await DialogHost.Show(new ConfirmDialog { DataContext = confirm }, RootDialogIdentifier);
        if (result is not true) {
            return;
        }

        IsBusy = true;
        try {
            await Api.DeleteContractorAsync(contractor.Id);
            Contractors.Remove(contractor);
            Notify("Contractor deleted.");
        }
        catch (Exception ex) {
            Notify($"Failed to delete contractor: {ex.Message}");
        }
        finally {
            IsBusy = false;
        }
    }
}
