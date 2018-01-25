using System;
using GestionFormation.CoreDomain.Stagiaires;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Stagiaires
{
    public class DeleteStagiaire : ActionCommand
    {
        protected DeleteStagiaire(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid stagiaireId)
        {
            var stagiaire = GetAggregate<Stagiaire>(stagiaireId);
            stagiaire.Delete();
            PublishUncommitedEvents(stagiaire);
        }
    }
}