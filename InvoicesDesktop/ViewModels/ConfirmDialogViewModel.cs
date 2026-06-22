using InvoicesDesktop.Mvvm;

namespace InvoicesDesktop.ViewModels;

public class ConfirmDialogViewModel : ObservableObject {

    public ConfirmDialogViewModel(string title, string message) {
        Title   = title;
        Message = message;
    }

    public string Title { get; }

    public string Message { get; }
}
