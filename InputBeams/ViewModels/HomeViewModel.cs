using CommunityToolkit.Mvvm.ComponentModel;
using InputBeams.Contracts.Services;
namespace InputBeams.ViewModels;

public partial class HomeViewModel : ObservableRecipient
{
    public INavigationService NavigationService
    {
        get;
    }
    public HomeViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }
}
