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

        public static Contact Create(string nom, string prenom, string email, string telephone)
        {
            var contact = new Contact(History.Empty);
            contact.AggregateId = Guid.NewGuid();
            contact.UncommitedEvents.Add(new ContactCreated(contact.AggregateId, 1, nom, prenom, email, telephone));
            return contact;
        }

        public void Update(string nom, string prenom, string email, string telephone)
        {
            Update(new ContactUpdated(AggregateId, GetNextSequence(), nom, prenom, email, telephone));
        }

        public void Delete()
        {
            Delete(new ContactDeleted(AggregateId, GetNextSequence()));
        }
    }
}
