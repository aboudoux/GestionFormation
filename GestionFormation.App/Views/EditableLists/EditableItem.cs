using System;
using System.Threading.Tasks;

namespace GestionFormation.App.Views.EditableLists
{
    /// <summary>
    /// Clase utilisée par devexpress en reflexion pour faire de l'édition
    /// </summary>
    public abstract class EditableItem    
    {
        private readonly IUpdatableListVm _parent;

        protected EditableItem()
        {
            
        }

        protected EditableItem(Guid id, IUpdatableListVm parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Id = id;
        }
        protected Guid Id { get; }
        public Guid GetId()
        {
            return Id;
        }

        public virtual async Task Edit()
        {
            await _parent.UpdateCommand.ExecuteAsync();
        }

    }
}