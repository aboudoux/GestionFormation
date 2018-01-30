using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Contacts.Events
{
    public class ContactCreated : DomainEvent
    {
        public string Nom { get; }
        public string Prenom { get; }
        public string Email { get; }
        public string Telephone { get; }
        public Guid SocieteId { get; }

        public ContactCreated(Guid aggregateId, int sequence, Guid societeId, string nom, string prenom, string email, string telephone) : base(aggregateId, sequence)
        {
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Telephone = telephone;
            SocieteId = societeId;
        }

        protected override string Description => "Contact créé";
    }
}