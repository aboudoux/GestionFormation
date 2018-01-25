using System.Collections.Generic;
using System.Text;
using GestionFormation.CoreDomain.Formateurs;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Formateurs
{
    public class CreateFormateur : ActionCommand
    {
        public CreateFormateur(EventBus eventBus) : base(eventBus)
        {
        }

        public Formateur Execute(string nom, string prenom, string email)
        {
            var formateur = Formateur.Create(nom, prenom, email);
            PublishUncommitedEvents(formateur);
            return formateur;
        }
    }
}
