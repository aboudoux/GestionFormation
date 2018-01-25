using System;

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

        public EditableItem(Guid id)
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