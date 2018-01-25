using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Utilisateurs.Events
{
    public class UtilisateurUpdated : DomainEvent
    {
        public string Nom { get; }
        public string Prenom { get; }
        public string Email { get; }
        public bool IsEnabled { get; }

        public UtilisateurUpdated(Guid aggregateId, int sequence, string nom, string prenom, string email, bool isEnabled) : base(aggregateId, sequence)
        {
            Nom = nom;
            Prenom = prenom;
            Email = email;
            IsEnabled = isEnabled;
        }
    }
}