using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Contacts.Events
{
    public class ContactCreated : DomainEvent
    {
        public string Lastname { get; }
        public string Firstname { get; }
        public string Email { get; }
        public string Telephone { get; }
        public Guid CompanyId { get; }

        public ContactCreated(Guid aggregateId, int sequence, Guid companyId, string lastname, string firstname, string email, string telephone) : base(aggregateId, sequence)
        {
            Lastname = lastname;
            Firstname = firstname;
            Email = email;
            Telephone = telephone;
            CompanyId = companyId;
        }

        protected override string Description => "Contact créé";
    }
}