using System;
using GestionFormation.Applications.Contacts.Exceptions;
using GestionFormation.CoreDomain.Companies.Queries;
using GestionFormation.CoreDomain.Contacts;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Contacts
{
    public class CreateContact : ActionCommand
    {
        private readonly ICompanyQueries _companyQueries;

        public CreateContact(EventBus eventBus, ICompanyQueries companyQueries) : base(eventBus)
        {
            _companyQueries = companyQueries ?? throw new ArgumentNullException(nameof(companyQueries));
        }

        public Contact Execute(Guid societeId, string nom, string prenom, string email, string telephone)
        {
            if(!_companyQueries.Exists(societeId))
                throw new CreateContactException();

            var contact = Contact.Create(societeId, nom, prenom, email, telephone);
            PublishUncommitedEvents(contact);
            return contact;
        }
    }
}
