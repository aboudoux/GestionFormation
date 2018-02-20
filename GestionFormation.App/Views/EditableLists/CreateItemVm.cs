using GestionFormation.App.Core;

namespace GestionFormation.App.Views.EditableLists
{
    public class CreateItemVm : PopupWindowVm
    {
        public override string Title { get; }
     
        public CreateItemVm(string title, object item)
        {
            Title = title;
            Item = item;
        }               
    }
}