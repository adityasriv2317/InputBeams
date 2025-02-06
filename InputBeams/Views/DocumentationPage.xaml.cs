using InputBeams.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace InputBeams.Views;

public sealed partial class DocumentationPage : Page
{
    public DocumentationViewModel ViewModel
    {
        get;
    }

    public DocumentationPage()
    {
        ViewModel = App.GetService<DocumentationViewModel>();
        InitializeComponent();
    }
}
