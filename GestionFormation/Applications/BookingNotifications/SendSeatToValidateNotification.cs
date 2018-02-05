using System;
using System.Text;
using GestionFormation.CoreDomain.BookingNotifications;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.BookingNotifications
{
    public class SendSeatToValidateNotification : ActionCommand
    {
        public SendSeatToValidateNotification(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid sessionId, Guid companyId, Guid seatId)
        {
            var notification = BookingNotification.SendSeatToValidate(sessionId, companyId, seatId);            
            PublishUncommitedEvents(notification);
        }
    }
}
