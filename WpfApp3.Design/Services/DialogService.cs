using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Models;
using ProjectName.Services;
using ProjectName.Wpf.ViewModels;
using ProjectName.Wpf.Views;

namespace ProjectName.Wpf.Services;

public class DialogService : IDialogService
{
    private readonly IServiceProvider _sp;
    public DialogService(IServiceProvider sp) => _sp = sp;

    public bool ShowTaskDetails(StageTaskPair pair)
    {
        var vm = ActivatorUtilities.CreateInstance<TaskDetailsViewModel>(_sp, pair);
        var window = new TaskDetailsWindow(vm)
        {
            Owner = Application.Current.MainWindow
        };
        return window.ShowDialog() == true;
    }

    public void ShowMessage(string message, string title = "Сообщение")
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

    public void ShowError(string message, string title = "Ошибка")
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
}