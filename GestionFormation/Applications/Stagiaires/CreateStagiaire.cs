using System.Collections.Generic;
using System.Text;
using GestionFormation.CoreDomain.Students;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Stagiaires
{
    public class CreateStagiaire: ActionCommand
    {
        public CreateStagiaire(EventBus eventBus) : base(eventBus)
        {
        }

        public Student Execute(string nom, string prenom)
        {
            var stagiaire = Student.Create(nom, prenom);
            PublishUncommitedEvents(stagiaire);
            return stagiaire;
        }
    }
}
