using System.Collections.Generic;
using System.Threading.Tasks;
using InvoicesDesktop.Mvvm;
using InvoicesDesktop.Services;
using MaterialDesignThemes.Wpf;

namespace InvoicesDesktop.ViewModels;

public class MainViewModel : ObservableObject {

    private SectionViewModel _currentSection;

    public MainViewModel() {
        var api = new InvoicesApiClient();
        MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

        Sections = [
            new InvoicesViewModel(api, MessageQueue),
            new ContractorsViewModel(api, MessageQueue),
            new ItemsViewModel(api, MessageQueue)
        ];

        _currentSection = Sections[0];
    }

    public ISnackbarMessageQueue MessageQueue { get; }

    public IReadOnlyList<SectionViewModel> Sections { get; }

    public SectionViewModel CurrentSection {
        get => _currentSection;
        set {
            if (SetProperty(ref _currentSection, value) && value is not null) {
                _ = value.LoadAsync();
            }
        }
    }

    public async Task InitializeAsync() => await CurrentSection.LoadAsync();
}
