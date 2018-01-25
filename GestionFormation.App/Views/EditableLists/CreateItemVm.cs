using GestionFormation.App.Core;

namespace GestionFormation.App.Views.EditableLists
{
    public class CreateItemVm : PopupWindowVm
    {
        public string Title { get; }
        public object Item { get; }

     
        public CreateItemVm(string title, object item)
        {
            Title = title;
            Item = item;
        }               
    }
}