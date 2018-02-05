using System;
using GestionFormation.CoreDomain.Companies;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Companies
{
    public class UpdateCompany : ActionCommand
    {
        public UpdateCompany(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid companyId, string name, string address, string zipCode, string city)
        {
            var societe = GetAggregate<Company>(companyId);
            societe.Update(name, address, zipCode, city);
            PublishUncommitedEvents(societe);
        }
    }
}