using GestionFormation.Applications.Companies;

namespace DataMigration.Creators
{
    public class CompanyCreator : Creator
    {
        public CompanyCreator(ApplicationService applicationService) : base(applicationService)
        {
        }

        public void Create(string name, string address, string zipCode, string city)
        {
            if(string.IsNullOrWhiteSpace(name))
                return;

            if(Mapper.Exists(name)) return;

            var company = App.Command<CreateCompany>().Execute(name, address, zipCode, city);
            Mapper.Add(name, company.AggregateId);
        }
    }
}