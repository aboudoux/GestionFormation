using System;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Agreements
{
    public class UpdateAgreement : ActionCommand
    {
        public UpdateAgreement(EventBus eventBus) : base(eventBus)
        {
        }

        public void ByDetailedPrice(Guid agreementId, decimal pricePerDayAndPerStudent)
        {
            var agreement = GetAggregate<Agreement>(agreementId);
            agreement.UpdatePricePerDayAndPerStudent(pricePerDayAndPerStudent);
            PublishUncommitedEvents(agreement);
            
        }

        public void ByPackagePrice(Guid agreementId, decimal packagePrice)
        {
            var agreement = GetAggregate<Agreement>(agreementId);
            agreement.UpdatePackagePrice(packagePrice);
            PublishUncommitedEvents(agreement);
        }        
    }
}