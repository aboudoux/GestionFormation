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
            if(companyName.IsEmpty()) return;
            if(contact.IsEmpty()) return;
            
            if(Mapper.Exists(ConstructKey(contact))) return;

            var contactName = new Name(contact);
            var createdContact = App.Command<CreateContact>().Execute(_companyCreator.GetCompanyId(companyName), contactName.Lastname, contactName.Firstname, email, telephone);

            Mapper.Add(ConstructKey(contact), createdContact.AggregateId);
        }

        public override string ConstructKey(string source)
        {
            var contactName = new Name(source);
            return contactName.ToString();
        }
    }
}