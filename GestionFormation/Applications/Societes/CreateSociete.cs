using System.Collections.Generic;
using System.Text;
using GestionFormation.CoreDomain.Societes;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Societes
{
    public class CreateSociete : ActionCommand
    {
        public CreateSociete(EventBus eventBus) : base(eventBus)
        {
        }

        public Societe Execute(string nom, string adresse, string codepostal, string ville)
        {
            var societe = Societe.Create(nom, adresse, codepostal, ville);
            PublishUncommitedEvents(societe);
            return societe;
        }
    }   
}
