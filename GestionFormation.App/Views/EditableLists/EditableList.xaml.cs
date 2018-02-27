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

        private async void TableView_OnEditFormShowing(object sender, EditFormShowingEventArgs e)
        {
            var vm = ((sender as TableView).GetRowElementByRowHandle(e.RowHandle).DataContext as RowData).Row as EditableItem;
            e.Allow = false;
            vm.Edit();            
        }
    }
}
