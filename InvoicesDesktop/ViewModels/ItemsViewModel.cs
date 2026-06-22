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

public class ItemsViewModel : SectionViewModel {

    private ItemModel? _selectedItem;

    public ItemsViewModel(InvoicesApiClient api, ISnackbarMessageQueue messageQueue)
        : base(api, messageQueue) {
        AddCommand    = new AsyncRelayCommand(AddAsync, () => !IsBusy);
        EditCommand   = new AsyncRelayCommand(p => EditAsync(p as ItemModel), p => !IsBusy && p is ItemModel);
        DeleteCommand = new AsyncRelayCommand(p => DeleteAsync(p as ItemModel), p => !IsBusy && p is ItemModel);
    }

    public override string Title => "Sale items";

    public override PackIconKind Icon => PackIconKind.PackageVariantClosed;

    public ObservableCollection<ItemModel> Items { get; } = [];

    public ItemModel? SelectedItem {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public ICommand AddCommand { get; }

    public ICommand EditCommand { get; }

    public ICommand DeleteCommand { get; }

    public override async Task LoadAsync() {
        IsBusy = true;
        try {
            var items = await Api.GetItemsAsync();
            Items.Clear();
            foreach (var item in items) {
                Items.Add(item);
            }
        }
        catch (Exception ex) {
            Notify($"Failed to load items: {ex.Message}");
        }
        finally {
            IsBusy = false;
        }
    }

    private async Task AddAsync() {
        var editor = new ItemEditorViewModel();
        var result = await DialogHost.Show(new ItemEditorDialog { DataContext = editor }, RootDialogIdentifier);
        if (result is not true) {
            return;
        }

        IsBusy = true;
        try {
            var created = await Api.CreateItemAsync(new ItemRequest { Name = editor.Name });
            if (created is not null) {
                Items.Add(created);
            }
            Notify("Item created.");
        }
        catch (Exception ex) {
            Notify($"Failed to create item: {ex.Message}");
        }
        finally {
            IsBusy = false;
        }
    }

    private async Task EditAsync(ItemModel? item) {
        if (item is null) {
            return;
        }

        var editor = new ItemEditorViewModel(item);
        var result = await DialogHost.Show(new ItemEditorDialog { DataContext = editor }, RootDialogIdentifier);
        if (result is not true) {
            return;
        }

        IsBusy = true;
        try {
            var updated = await Api.UpdateItemAsync(item.Id, new ItemRequest { Name = editor.Name });
            if (updated is not null) {
                var index = Items.IndexOf(item);
                if (index >= 0) {
                    Items[index] = updated;
                }
            }
            Notify("Item updated.");
        }
        catch (Exception ex) {
            Notify($"Failed to update item: {ex.Message}");
        }
        finally {
            IsBusy = false;
        }
    }

    private async Task DeleteAsync(ItemModel? item) {
        if (item is null) {
            return;
        }

        var confirm = new ConfirmDialogViewModel("Delete item", $"Delete \"{item.Name}\"? This action cannot be undone.");
        var result  = await DialogHost.Show(new ConfirmDialog { DataContext = confirm }, RootDialogIdentifier);
        if (result is not true) {
            return;
        }

        IsBusy = true;
        try {
            await Api.DeleteItemAsync(item.Id);
            Items.Remove(item);
            Notify("Item deleted.");
        }
        catch (Exception ex) {
            Notify($"Failed to delete item: {ex.Message}");
        }
        finally {
            IsBusy = false;
        }
    }
}
