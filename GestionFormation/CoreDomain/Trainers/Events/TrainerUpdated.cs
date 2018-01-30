using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainers.Events
{
    public class TrainerUpdated : DomainEvent
    {
        public string Lastname { get; }
        public string Firstname { get; }
        public string Email { get; }

        public TrainerUpdated(Guid aggregateId, int sequence, string lastname, string firstname, string email) : base(aggregateId, sequence)
        {
            Lastname = lastname;
            Firstname = firstname;
            Email = email;
        }

        protected override string Description => "Formateur modifié";
    }
}