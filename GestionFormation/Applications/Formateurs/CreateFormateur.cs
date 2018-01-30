using System.Collections.Generic;
using System.Text;
using GestionFormation.CoreDomain.Trainers;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Formateurs
{
    public class CreateFormateur : ActionCommand
    {
        public CreateFormateur(EventBus eventBus) : base(eventBus)
        {
        }

        public Trainer Execute(string nom, string prenom, string email)
        {
            var formateur = Trainer.Create(nom, prenom, email);
            PublishUncommitedEvents(formateur);
            return formateur;
        }
    }
}
