using System.Threading.Tasks;
using System.Windows.Input;
using InvoicesDesktop.Mvvm;
using InvoicesDesktop.Services;
using MaterialDesignThemes.Wpf;

namespace InvoicesDesktop.ViewModels;

/// <summary>
/// Base class for the navigable sections of the application.
/// </summary>
public abstract class SectionViewModel : ObservableObject {

    public const string RootDialogIdentifier = "RootDialog";

    private bool _isBusy;

    protected SectionViewModel(InvoicesApiClient api, ISnackbarMessageQueue messageQueue) {
        Api          = api;
        MessageQueue = messageQueue;
        RefreshCommand = new AsyncRelayCommand(LoadAsync, () => !IsBusy);
    }

    protected InvoicesApiClient Api { get; }

    protected ISnackbarMessageQueue MessageQueue { get; }

    public abstract string Title { get; }

    public abstract PackIconKind Icon { get; }

    public ICommand RefreshCommand { get; }

    public bool IsBusy {
        get => _isBusy;
        protected set => SetProperty(ref _isBusy, value);
    }

    public abstract Task LoadAsync();

    protected void Notify(string message) => MessageQueue.Enqueue(message);
}
