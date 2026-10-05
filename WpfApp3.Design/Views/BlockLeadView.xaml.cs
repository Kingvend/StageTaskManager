using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ProjectName.Wpf.ViewModels.AgreementBlocks;

namespace ProjectName.Wpf.Views;

public partial class BlockLeadView : UserControl
{
    public BlockLeadView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is not BlockLeadAgreementViewModel vm) return;

        PlanGrid.Columns.Clear();

        PlanGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Показатели",
            Binding = new Binding(nameof(BusinessPlanRowViewModel.Indicator)),
            Width = new DataGridLength(180),
            IsReadOnly = true,
        });

        for (var i = 0; i < vm.Years.Count; i++)
        {
            var idx = i;
            PlanGrid.Columns.Add(new DataGridTextColumn
            {
                Header = vm.Years[i].ToString(),
                Binding = new Binding($"Cells[{idx}].Value")
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            });
        }
    }
}