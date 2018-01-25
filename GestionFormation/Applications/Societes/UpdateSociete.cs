using System;
using GestionFormation.CoreDomain.Societes;
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
            var societe = GetAggregate<Societe>(societeId);
            societe.Update(nom, adresse, codepostal, ville);
            PublishUncommitedEvents(societe);
        }
    }
}