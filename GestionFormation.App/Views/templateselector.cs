using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.GroupRowLayout;

namespace GestionFormation.App.Views
{
    class templateselector:  EditFormCellTemplateSelector
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
