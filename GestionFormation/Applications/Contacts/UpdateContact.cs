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

        public void Execute(Guid contactId, Guid societeId, string nom, string prenom, string email, string telephone)
        {
            var contact = GetAggregate<Contact>(contactId);
            contact.Update(societeId, nom, prenom, email, telephone);
            PublishUncommitedEvents(contact);
        }
    }
}