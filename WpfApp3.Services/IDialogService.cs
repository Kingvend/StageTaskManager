using ProjectName.Models;

namespace ProjectName.Services;

public interface IDialogService
{
    /// <summary>Открывает модальное окно деталей задачи. Возвращает true, если задача была подтверждена.</summary>
    bool ShowTaskDetails(Stage stage, ProjectTask task);
}