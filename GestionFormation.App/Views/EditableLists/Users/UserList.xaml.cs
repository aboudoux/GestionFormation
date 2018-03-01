using DevExpress.Xpf.Grid;

namespace GestionFormation.App.Views.EditableLists.Users
{
    /// <summary>
    /// Logique d'interaction pour EditableList.xaml
    /// </summary>
    public partial class UserList
    {
        public UserList()
        {
            InitializeComponent();
        }

        private async void TableView_OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            await ((sender as TableView).DataContext as IUpdatableListVm).UpdateCommand.ExecuteAsync();
            e.Handled = true;
        }
    }
}
