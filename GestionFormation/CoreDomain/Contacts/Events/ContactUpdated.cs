using System;

namespace GestionFormation.CoreDomain.Contacts.Events
{
    public class ContactUpdated : ContactCreated
    {
        public ContactUpdated(Guid aggregateId, int sequence, string nom, string prenom, string email, string telephone) : base(aggregateId, sequence, nom, prenom, email, telephone)
        {
        }
    }
}