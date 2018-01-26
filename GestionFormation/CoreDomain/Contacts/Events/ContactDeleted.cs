using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Contacts.Events
{
    public class ContactDeleted : DomainEvent
    {
        public ContactDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Contact supprimé";
    }
}