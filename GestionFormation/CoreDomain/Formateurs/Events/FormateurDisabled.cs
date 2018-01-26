using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs.Events
{
    public class FormateurDisabled : DomainEvent
    {
        public FormateurDisabled(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Formateur désactivé";
    }
}