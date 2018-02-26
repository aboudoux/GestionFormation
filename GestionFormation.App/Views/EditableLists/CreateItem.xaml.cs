using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.LayoutControl;

namespace GestionFormation.App.Views.EditableLists
{
    /// <summary>
    /// Logique d'interaction pour CreateItem.xaml
    /// </summary>
    public partial class CreateItem
    {
        public CreateItem()
        {
            InitializeComponent();
        }

        private void DataLayoutControl_OnAutoGeneratingItem(object sender,
            DataLayoutControlAutoGeneratingItemEventArgs e)
        {
            var dataLayoutControl = (DataLayoutControl) sender;

            if (e.PropertyType == typeof(Color))
            {
                var template = dataLayoutControl.Resources["ColorEditor"] as DataTemplate;
                if (template == null) return;
                e.Item.Content = template.LoadContent() as UIElement;
            }
        }
    }
}
