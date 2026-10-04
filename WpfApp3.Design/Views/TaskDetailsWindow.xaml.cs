using System.Windows;
using ProjectName.Wpf.ViewModels;

namespace ProjectName.Wpf.Views;

public partial class TaskDetailsWindow : Window
{
    private readonly TaskDetailsViewModel _vm;

    public TaskDetailsWindow(TaskDetailsViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;

        _vm.RequestClose += OnRequestClose;

        Loaded += async (_, _) => await _vm.InitializeAsync();
    }

    private void OnRequestClose(object? sender, bool result)
        => DialogResult = result;
}