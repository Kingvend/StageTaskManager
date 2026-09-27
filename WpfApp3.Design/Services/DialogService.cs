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

    public bool ShowTaskDetails(Stage stage, ProjectTask task)
    {
        var vm = ActivatorUtilities.CreateInstance<TaskDetailsViewModel>(_sp, stage, task);
        var window = new TaskDetailsWindow(vm)
        {
            Owner = Application.Current.MainWindow
        };

        return window.ShowDialog() == true;
    }
}