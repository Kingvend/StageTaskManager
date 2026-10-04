using ProjectName.Models;

namespace ProjectName.Services;

public interface IDialogService
{
    /// <summary>Открывает модальное окно деталей задачи. true — задача подтверждена.</summary>
    bool ShowTaskDetails(StageTaskPair pair);

    /// <summary>Показывает информационное сообщение.</summary>
    void ShowMessage(string message, string title = "Сообщение");

    /// <summary>Показывает сообщение об ошибке.</summary>
    void ShowError(string message, string title = "Ошибка");
}