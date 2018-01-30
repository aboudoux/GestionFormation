using System.Collections.Generic;
using System.Text;
using GestionFormation.CoreDomain.Companies;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Societes
{
    public class CreateSociete : ActionCommand
    {
        public CreateSociete(EventBus eventBus) : base(eventBus)
        {
        }

        public Company Execute(string nom, string adresse, string codepostal, string ville)
        {
            var societe = Company.Create(nom, adresse, codepostal, ville);
            PublishUncommitedEvents(societe);
            return societe;
        }
    }   
}
