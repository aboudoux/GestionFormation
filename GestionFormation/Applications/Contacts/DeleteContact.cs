using System;
using GestionFormation.CoreDomain.Contacts;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Contacts
{
    public class DeleteContact : ActionCommand
    {
        public DeleteContact(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid contactId)
        {
            var contact = GetAggregate<Contact>(contactId);
            contact.Delete();
            PublishUncommitedEvents(contact);
        }
    }
}