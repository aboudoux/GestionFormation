using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Places.Events
{
    public class PlaceRefused : DomainEvent
    {
        public string Raison { get; }

        public PlaceRefused(Guid aggregateId, int sequence, string raison) : base(aggregateId, sequence)
        {
            Raison = raison;
        }

        protected override string Description => "Place refusée";
    }
}