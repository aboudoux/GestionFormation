using System;
using GestionFormation.CoreDomain.Formateurs;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Formateurs
{
    public class UpdateFormateur : ActionCommand
    {
        public UpdateFormateur(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid formateurId, string nom, string prenom, string email)
        {
            var formateur = GetAggregate<Formateur>(formateurId);
            formateur.Update(nom, prenom, email);
            PublishUncommitedEvents(formateur);
        }
    }
}