using System;
using GestionFormation.CoreDomain.Companies;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Companies
{
    public class DeleteCompany : ActionCommand
    {
        public DeleteCompany(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid companyId)
        {
            var company = GetAggregate<Company>(companyId);
            company.Delete();
            PublishUncommitedEvents(company);
        }
    }
}