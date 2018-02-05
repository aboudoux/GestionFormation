using GestionFormation.CoreDomain.Companies;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Companies
{
    public class CreateCompany : ActionCommand
    {
        public CreateCompany(EventBus eventBus) : base(eventBus)
        {
        }

        public Company Execute(string name, string address, string zipCode, string city)
        {
            var company = Company.Create(name, address, zipCode, city);
            PublishUncommitedEvents(company);
            return company;
        }
    }   
}
