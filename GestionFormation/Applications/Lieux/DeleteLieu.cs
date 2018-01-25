using System;
using GestionFormation.CoreDomain.Lieux;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Lieux
{
    public class DeleteLieu : ActionCommand
    {
        public DeleteLieu(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid lieuId)
        {
            var lieu = GetAggregate<Lieu>(lieuId);
            lieu.Delete();
            PublishUncommitedEvents(lieu);
        }
    }
}