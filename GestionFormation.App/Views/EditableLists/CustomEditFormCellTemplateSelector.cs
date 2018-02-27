using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Grid;

namespace GestionFormation.App.Views.EditableLists
{
    class CustomEditFormCellTemplateSelector:  EditFormCellTemplateSelector
    {
        public DataTemplate  ColorPickerTemplate { get; set;}

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var cell = item as EditGridCellData;

            switch (cell.Value)
            {                
                case Color c:
                    return ColorPickerTemplate;
                default:
                    return base.SelectTemplate(item, container);
                   
            }
        }
    }
}
