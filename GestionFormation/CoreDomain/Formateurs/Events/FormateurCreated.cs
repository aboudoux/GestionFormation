using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs.Events
{
    public class FormateurCreated : DomainEvent
    {
        public string Nom { get; }
        public string Prenom { get; }
        public string Email { get; }

        public FormateurCreated(Guid aggregateId, int sequence, string nom, string prenom, string email) : base(aggregateId, sequence)
        {
            Nom = nom;
            Prenom = prenom;
            Email = email;
        }
    }
}