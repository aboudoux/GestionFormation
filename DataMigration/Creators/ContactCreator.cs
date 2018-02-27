using System;
using GestionFormation.Applications.Contacts;

namespace DataMigration.Creators
{
    public class ContactCreator : Creator
    {
        private readonly CompanyCreator _companyCreator;

        public ContactCreator(ApplicationService applicationService, CompanyCreator companyCreator) : base(applicationService)
        {
            _companyCreator = companyCreator ?? throw new ArgumentNullException(nameof(companyCreator));
        }

        public void Create(string contact, string email, string telephone, string companyName)
        {
            if(string.IsNullOrWhiteSpace(companyName)) return;
            if(string.IsNullOrWhiteSpace(contact)) return;

            var contactName = new Name(contact);
            if(Mapper.Exists(contactName.ToString())) return;

            var createdContact = App.Command<CreateContact>().Execute(_companyCreator.GetCompanyId(companyName), contactName.Lastname, contactName.Firstname, email, telephone);

            Mapper.Add(contactName.ToString(), createdContact.AggregateId);
        }
    }
}