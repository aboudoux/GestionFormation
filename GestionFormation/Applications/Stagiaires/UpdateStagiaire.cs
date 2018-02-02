using System;
using GestionFormation.CoreDomain.Students;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Stagiaires
{
    public class UpdateStagiaire : ActionCommand
    {
        protected UpdateStagiaire(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid stagiaireId, string nom, string prenom)
        {
            var stagiaire = GetAggregate<Student>(stagiaireId);
            stagiaire.Update(nom, prenom);
            PublishUncommitedEvents(stagiaire);
        }
    }
}