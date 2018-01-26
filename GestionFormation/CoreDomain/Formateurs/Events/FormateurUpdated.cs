using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs.Events
{
    public class FormateurUpdated : DomainEvent
    {
        public string Nom { get; }
        public string Prenom { get; }
        public string Email { get; }

        public FormateurUpdated(Guid aggregateId, int sequence, string nom, string prenom, string email) : base(aggregateId, sequence)
        {
            Nom = nom;
            Prenom = prenom;
        }

        protected override string Description => "Formateur modifié";
    }
}