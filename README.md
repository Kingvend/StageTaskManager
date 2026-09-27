WPF-приложение на .NET 7 с использованием CommunityToolkit.Mvvm и Microsoft.Extensions.DependencyInjection для управления проектами, их этапами и задачами, с механизмом согласования задач и сохранением черновиков в JSON.

Структура решения
text
ProjectName.sln
├── ProjectName.Models          (net7.0, classlib)
├── ProjectName.Services        (net7.0, classlib)
└── ProjectName.Wpf             (net7.0-windows, WinExe)
ProjectName.Models
Библиотека предметной области и DTO для черновиков. Не зависит от WPF.

Доменные модели

Project — Id: Guid, название, описание, даты начала/окончания, статус, ответственный, список Stage.

Stage — название, описание, порядковый номер, даты, статус, список ProjectTask.

ProjectTask — PairId: Guid (идентификатор пары «этап-задача»), название, описание, статус, список ApprovalInfo.

ApprovalInfo — роль, флаг согласования, комментарий.

Enum'ы

ProjectStatus, StageStatus, TaskStatus — статусы.

AgreementRole — TechnicalSpecialist, GroupLead, BlockLead.

Контракты

IAgreementBlock — интерфейс блока согласования: Role, IsAgreed, Comment, методы ToDictionary() и LoadFrom(...).

DTO черновиков (Drafts/)

ProjectDraftDto — метаданные проекта для project.json.

PairDraftDto — данные пары «этап-задача» для {pairGuid}.json: PairId, ProjectId, SavedAt, слепки Stage/Task, словарь Blocks.

StageSnapshot, TaskSnapshot — вложенные слепки.

ProjectName.Services
Сервисный слой без зависимости от UI.

IProjectService / ProjectService — отдаёт текущий проект (пока демо-данные с фиксированными Guid'ами), содержит заглушку SaveAsync под будущее сохранение в БД.

IProjectContext / ProjectContext — app-level контекст: хранит загруженный Project, чтобы все ViewModel'и работали с одними и теми же экземплярами.

IDraftStorage / LocalJsonDraftStorage — сохранение/чтение черновиков:

SaveProjectAsync — создаёт project.json при первом сохранении, потом не трогает.

SavePairAsync — перезаписывает {pairGuid}.json со всеми вкладками.

TryLoadPairAsync — читает файл пары, конвертирует JsonElement в примитивы.

Настройки: UTF-8 без экранирования (JavaScriptEncoder.UnsafeRelaxedJsonEscaping), JsonStringEnumConverter, WriteIndented.

IDialogService — интерфейс для показа модальных окон (реализация в WPF).

ProjectName.Wpf
Приложение WPF: ViewModel'и, Views и инфраструктура.

App.xaml.cs — регистрация DI: сервисы (Singleton), MainViewModel (Singleton), MainWindow (Singleton), DialogService. Создание главного окна в OnStartup.

Services/DialogService.cs — реализация IDialogService: через ActivatorUtilities создаёт TaskDetailsViewModel, оборачивает в TaskDetailsWindow, вызывает ShowDialog(), возвращает DialogResult.

ViewModels
MainViewModel

CurrentProject, коллекция TaskItems (пары «этап-задача»).

LoadCommand — наполняет TaskItems, инициализирует IProjectContext, подписывается на StatusChanged каждой пары.

SaveCommand — с CanExecute = CanSave(): активна, когда все задачи проекта в статусе Completed; сама логика сохранения в БД пока заглушка.

StageTaskPairViewModel

Stage, Task, StageName, TaskName, Status.

OpenDetailsCommand — вызывает IDialogService.ShowTaskDetails; если задача подтверждена — обновляет Status и поднимает событие StatusChanged.

TaskDetailsViewModel

Три блока: TechnicalSpecialist, GroupLead, BlockLead (реализации IAgreementBlock).

Blocks — коллекция для привязки во вкладки.

SelectedBlock, CanConfirm (все три блока согласованы).

InitializeAsync — при открытии: если в памяти пусто, читает {pairGuid}.json и заполняет блоки.

SaveDraftAsync — SyncToApprovals + запись project.json (если нет) и {pairGuid}.json.

ConfirmCommand (с CanExecute = CanConfirm) — синхронизирует в память, ставит Task.Status = Completed, закрывает окно.

Событие RequestClose.

Автосохранение при закрытии окна отключено (код закомментирован).

AgreementBlocks/AgreementBlockViewModel — абстрактная база: IsAgreed, Comment, ToDictionary(), LoadFrom().

TechnicalSpecialistAgreementViewModel

GroupLeadAgreementViewModel

BlockLeadAgreementViewModel

Каждый переопределяет Role и DisplayName.

Views
MainWindow — три блока:

Данные по проекту.
Список задач: карточки Stage.Name + Task.Name + статус + кнопка «Открыть детали», всё в ScrollViewer.
Кнопка «Сохранить» (активна при всех выполненных задачах).
TaskDetailsWindow — модальное окно ShowDialog с TabControl (3 вкладки), кнопками «Сохранить черновик» и «Подтвердить выполнение».

AgreementBlockView — UserControl с базовым наполнением вкладки (CheckBox + поле комментария).

TechnicalSpecialistTabView, GroupLeadTabView, BlockLeadTabView — обёртки над AgreementBlockView; в будущем наполнение разойдётся.

Логика работы
Загрузка главного окна
MainWindow.Loaded → LoadCommand → ProjectService.GetCurrentProject() → сохранение в IProjectContext → построение TaskItems.

Согласование задачи
«Открыть детали» → DialogService.ShowTaskDetails.

TaskDetailsViewModel.InitializeAsync: если Task.Approvals пуст — читаем {pairGuid}.json.

Пользователь вводит данные на вкладках → OnBlockPropertyChanged → SyncToApprovals (данные сразу в памяти).

«Сохранить черновик» → запись файла.

«Подтвердить выполнение» доступна, когда все 3 вкладки согласованы (CanConfirm).

После подтверждения: Task.Status = Completed, окно закрывается, StageTaskPairViewModel шлёт StatusChanged → MainViewModel пересчитывает CanSave.

Когда последняя задача проекта завершена — активируется кнопка «Сохранить» в главном окне.

Формат хранения черновиков
text
%USERPROFILE%\Documents\ProjectName\drafts\{projectGuid}\
├── project.json               ← метаданные проекта, создаётся один раз
├── {pairGuid1}.json           ← вкладки пары №1, перезаписывается
├── {pairGuid2}.json
└── ...
Внутри каждого файла — SavedAt (UTC) для будущего разрешения конфликта «БД vs черновик».

Файл пары содержит Blocks — словарь Role → { IsAgreed, Comment, ... } (через ToDictionary()), готовый к расширению специфичными полями ролей.

Формат совместим с будущей БД-схемой: таблица «Проект» + таблица «Пары этап-задача» с JSON-колонкой и FK на проект.

Что реализовано
✅ Трёхуровневая архитектура (Models / Services / Wpf) на .NET 7.

✅ DI через Microsoft.Extensions.DependencyInjection.

✅ MVVM через CommunityToolkit.Mvvm (source generators).

✅ Модальное окно деталей задачи с тремя вкладками согласования.

✅ Общий интерфейс IAgreementBlock и специфичные реализации для каждой роли.

✅ Кнопка «Подтвердить выполнение» активируется только при согласовании всех вкладок.

✅ Кнопка «Сохранить» на главной форме активна только при всех завершённых задачах.

✅ In-memory синхронизация состояния вкладок в Task.Approvals.

✅ Загрузка состояния из черновика при первом открытии в сессии.

✅ Локальное сохранение черновиков в JSON (UTF-8 без экранирования), структура под будущую БД.

Что оставлено заглушкой / на будущее
Сохранение в БД: IProjectService.SaveAsync пуст; кнопка «Сохранить» пока ничего не делает.

Загрузка из БД в InitializeAsync — не реализована; приоритет «БД vs черновик по timestamp» заложен в структуре файлов, но логика не написана.

HTTP-отправка черновиков: IDraftStorage спроектирован под стратегию — можно добавить HttpDraftStorage или CompositeDraftStorage без правок ViewModel'ей.

Автосохранение при закрытии окна — отключено (по требованию), код закомментирован.

Уведомления пользователю об ошибках/успехе сохранения — не реализованы.

UI-редактирование метаданных проекта — не реализовано; project.json после первого создания не обновляется.

Редактирование вкладок деталей — сейчас только CheckBox «Согласовать»; наполнение под каждую роль будет расширяться через переопределение ToDictionary/LoadFrom и специфичные UserControl'ы.
