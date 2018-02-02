using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Students.Events
{
    public class StudentCreated : DomainEvent
    {
        public string Lastname { get; }
        public string Firstname { get; }

        public StudentCreated(Guid aggregateId, int sequence, string lastname, string firstname) : base(aggregateId, sequence)
        {
            Lastname = lastname;
            Firstname = firstname;            
        }

        protected override string Description => "Stagiaire créé";
    }
}