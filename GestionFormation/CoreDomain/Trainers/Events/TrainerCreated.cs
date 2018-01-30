using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainers.Events
{
    public class TrainerCreated : DomainEvent
    {
        public string Lastname { get; }
        public string Firstname { get; }
        public string Email { get; }

        public TrainerCreated(Guid aggregateId, int sequence, string lastname, string firstname, string email) : base(aggregateId, sequence)
        {
            Lastname = lastname;
            Firstname = firstname;
            Email = email;
        }

        protected override string Description => "Formateur créé";
    }
}