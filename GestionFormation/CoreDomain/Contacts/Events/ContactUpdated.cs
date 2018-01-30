using System;

namespace GestionFormation.CoreDomain.Contacts.Events
{
    public class ContactUpdated : ContactCreated
    {
        public ContactUpdated(Guid aggregateId, int sequence, Guid societeId, string nom, string prenom, string email, string telephone) : base(aggregateId, sequence, societeId, nom, prenom, email, telephone)
        {
        }
    }
}