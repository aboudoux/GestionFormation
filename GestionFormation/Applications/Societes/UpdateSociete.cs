using System;
using GestionFormation.CoreDomain.Companies;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Societes
{
    public class UpdateSociete : ActionCommand
    {
        public UpdateSociete(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid societeId, string nom, string adresse, string codepostal, string ville)
        {
            var societe = GetAggregate<Company>(societeId);
            societe.Update(nom, adresse, codepostal, ville);
            PublishUncommitedEvents(societe);
        }
    }
}