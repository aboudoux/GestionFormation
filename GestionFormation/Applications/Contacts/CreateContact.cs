using System;
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
                throw new Exception("Impossible de créer le contact car la société à laquele vous voulez le rattacher n'existe pas");

            var contact = Contact.Create(societeId, nom, prenom, email, telephone);
            PublishUncommitedEvents(contact);
            return contact;
        }
    }
}
