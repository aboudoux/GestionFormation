using System;
using GestionFormation.CoreDomain.Contacts;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Contacts
{
    public class UpdateContact : ActionCommand
    {
        public UpdateContact(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid contactId, string nom, string prenom, string email, string telephone)
        {
            var contact = GetAggregate<Contact>(contactId);
            contact.Update(nom, prenom, email, telephone);
            PublishUncommitedEvents(contact);
        }
    }
}