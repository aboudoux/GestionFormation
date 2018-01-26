using System;
using GestionFormation.CoreDomain.Societes;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Societes
{
    public class DeleteSociete : ActionCommand
    {
        public DeleteSociete(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid societeId)
        {
            var societe = GetAggregate<Societe>(societeId);
            societe.Delete();
            PublishUncommitedEvents(societe);
        }
    }
}