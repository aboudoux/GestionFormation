using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Grid;

namespace GestionFormation.App.Views.EditableLists
{
    /// <summary>
    /// Logique d'interaction pour EditableList.xaml
    /// </summary>
    public partial class EditableList
    {
        public EditableList()
        {
            InitializeComponent();
        }

        private void DataControlBase_OnAutoGeneratingColumn(object sender, AutoGeneratingColumnEventArgs e)
        {
            if (e.Column.FieldType == typeof(Color))
            {                
                e.Column.EditFormTemplate = MyGrid.Resources["ColorTemplate"] as DataTemplate;
            }
        }

        private async void TableView_OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            await ((sender as TableView).DataContext as IUpdatableListVm).UpdateCommand.ExecuteAsync();
            e.Handled = true;
        }
    }
}
