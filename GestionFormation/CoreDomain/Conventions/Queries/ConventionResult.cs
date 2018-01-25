using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Conventions.Queries
{
    public class ConventionResult : IConventionResult
    {
        public ConventionResult(Guid conventionId, string societe, string contact, string conventionNumber, List<Guid> placesId)
        {
            ConventionId = conventionId;
            Societe = societe;
            Contact = contact;
            Places = placesId ?? new List<Guid>();
            ConventionNumber = conventionNumber;
        }

        public Guid ConventionId { get; }
        public string Societe { get; }
        public string Contact { get; }
        public List<Guid> Places { get; }
        public string ConventionNumber { get; }
    }
}