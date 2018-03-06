using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Agreements.Events
{
    public class  AgreementUpdated : DomainEvent
    {
        public decimal PricePerDayAndPerStudent { get; }        
        public decimal PackagePrice { get; }

        public AgreementUpdated(Guid aggregateId, int sequence, decimal pricePerDayAndPerStudent, decimal packagePrice) : base(aggregateId, sequence)
        {
            PricePerDayAndPerStudent = pricePerDayAndPerStudent;            
            PackagePrice = packagePrice;
        }

        protected override string Description => "Convention mise à jour";
    }
}