using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain.Agreements.Queries;

namespace GestionFormation.Infrastructure.Agreements.Queries
{
    public class AgreementResult : IAgreementResult
    {
        public AgreementResult(Guid conventionId, string societe, string contact, string conventionNumber, List<Guid> placesId)
        {
            AgreementId = conventionId;
            Company = societe;
            Contact = contact;
            Seats = placesId ?? new List<Guid>();
            AgreementNumber = conventionNumber;
        }

        public Guid AgreementId { get; }
        public string Company { get; }
        public string Contact { get; }
        public List<Guid> Seats { get; }
        public string AgreementNumber { get; }
    }
}