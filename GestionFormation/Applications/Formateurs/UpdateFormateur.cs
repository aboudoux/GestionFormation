using System;
using GestionFormation.CoreDomain.Trainers;
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
            var formateur = GetAggregate<Trainer>(formateurId);
            formateur.Update(nom, prenom, email);
            PublishUncommitedEvents(formateur);
        }
    }
}