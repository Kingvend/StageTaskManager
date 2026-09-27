using System.Windows;
using ProjectName.Wpf.ViewModels;

namespace ProjectName.Wpf.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;

        Loaded += async (_, _) => await vm.LoadCommand.ExecuteAsync(null);
    }
}