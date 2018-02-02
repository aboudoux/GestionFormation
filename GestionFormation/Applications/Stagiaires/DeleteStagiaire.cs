using System;
using GestionFormation.CoreDomain.Students;
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
            var stagiaire = GetAggregate<Student>(stagiaireId);
            stagiaire.Delete();
            PublishUncommitedEvents(stagiaire);
        }
    }
}