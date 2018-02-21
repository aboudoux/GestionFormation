using System;
using GestionFormation.Applications.Companies.Exceptions;
using GestionFormation.CoreDomain.Companies;
using GestionFormation.CoreDomain.Companies.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Companies
{
    public class UpdateCompany : ActionCommand
    {
        private readonly ICompanyQueries _companyQueries;

        public UpdateCompany(EventBus eventBus, ICompanyQueries companyQueries) : base(eventBus)
        {
            _companyQueries = companyQueries ?? throw new ArgumentNullException(nameof(companyQueries));
        }

        public void Execute(Guid companyId, string name, string address, string zipCode, string city)
        {
            var existingCompanyId = _companyQueries.GetIdIfExists(name);
            if(existingCompanyId.HasValue && existingCompanyId.Value != companyId)            
                throw new CompanyAlreadyExistsException(name);

            var societe = GetAggregate<Company>(companyId);
            societe.Update(name, address, zipCode, city);
            PublishUncommitedEvents(societe);
        }
    }
}