using System;
using System.Threading.Tasks;

namespace GestionFormation.App.Views.EditableLists
{
    /// <summary>
    /// Clase utilis�e par devexpress en reflexion pour faire de l'�dition
    /// </summary>
    public abstract class EditableItem    
    {

        protected EditableItem()
        {            
        }

        protected EditableItem(Guid id)
        {
            Id = id;
        }
        protected Guid Id { get; }
        public Guid GetId()
        {
            return Id;
        }       
    }
}