using System;

namespace GestionFormation.CoreDomain.Contacts.Events
{
    public class ContactUpdated : ContactCreated
    {
        public ContactUpdated(Guid aggregateId, int sequence, Guid companyId, string lastname, string firstname, string email, string telephone) : base(aggregateId, sequence, companyId, lastname, firstname, email, telephone)
        {
        }
    }
}