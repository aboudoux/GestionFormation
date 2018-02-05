using System;
using GestionFormation.CoreDomain.Contacts.Events;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Contacts
{
    public class Contact : AggregateRootUpdatableAndDeletable<ContactUpdated, ContactDeleted>
    {
        public Contact(History history) : base(history)
        {
        }

        public static Contact Create(Guid companyId, string lastname, string firstname, string email, string telephone)
        {            
            companyId.EnsureNotEmpty(nameof(companyId));

            var contact = new Contact(History.Empty);
            contact.AggregateId = Guid.NewGuid();
            contact.UncommitedEvents.Add(new ContactCreated(contact.AggregateId, 1, companyId, lastname, firstname, email, telephone));
            return contact;
        }

        public void Update(Guid companyId, string lastname, string firstname, string email, string telephone)
        {            
            companyId.EnsureNotEmpty(nameof(companyId));
            Update(new ContactUpdated(AggregateId, GetNextSequence(), companyId, lastname, firstname, email, telephone));
        }

        public void Delete()
        {
            Delete(new ContactDeleted(AggregateId, GetNextSequence()));
        }
    }
}
