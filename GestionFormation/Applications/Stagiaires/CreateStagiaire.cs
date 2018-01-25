using System.Collections.Generic;
using System.Text;
using GestionFormation.CoreDomain.Stagiaires;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Stagiaires
{
    public class CreateStagiaire: ActionCommand
    {
        public CreateStagiaire(EventBus eventBus) : base(eventBus)
        {
        }

        public Stagiaire Execute(string nom, string prenom)
        {
            var stagiaire = Stagiaire.Create(nom, prenom);
            PublishUncommitedEvents(stagiaire);
            return stagiaire;
        }
    }
}
