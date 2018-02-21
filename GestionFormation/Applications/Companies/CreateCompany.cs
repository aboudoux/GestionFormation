using System;
using GestionFormation.Applications.Companies.Exceptions;
using GestionFormation.CoreDomain.Companies;
using GestionFormation.CoreDomain.Companies.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Companies
{
    public class CreateCompany : ActionCommand
    {
        private readonly ICompanyQueries _companyQueries;

        public CreateCompany(EventBus eventBus, ICompanyQueries companyQueries) : base(eventBus)
        {
            _companyQueries = companyQueries ?? throw new ArgumentNullException(nameof(companyQueries));
        }

        public Company Execute(string name, string address, string zipCode, string city)
        {
            if (_companyQueries.GetIdIfExists(name).HasValue)
                throw new CompanyAlreadyExistsException(name);

            var company = Company.Create(name, address, zipCode, city);
            PublishUncommitedEvents(company);
            return company;
        }
    }   
}
