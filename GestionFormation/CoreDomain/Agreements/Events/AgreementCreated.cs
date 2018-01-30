using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Agreements.Events
{
    public class AgreementCreated : DomainEvent
    {
        public Guid ContactId { get; }
        public string Agreement { get; }
        public AgreementType AgreementType { get; }

        public AgreementCreated(Guid aggregateId, int sequence, Guid contactId, long agreementNumber, AgreementType agreementType) : base(aggregateId, sequence)
        {
            ContactId = contactId;
            AgreementType = agreementType;
            Agreement = DateTime.Now.Year + " " + agreementNumber + " T";
        }

        protected override string Description => "Convention créée";
    }
}