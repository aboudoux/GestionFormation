using System;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Notifications.Events
{
    public class SeatToDefineStudentNotificationSent : NotificationEvent
    {
        public Guid SeatId { get; }

        public SeatToDefineStudentNotificationSent(Guid aggregateId, int sequence, Guid sessionId, Guid companyId, Guid seatId, Guid notificationId) : base(aggregateId, sequence, sessionId, companyId, notificationId)
        {
            SeatId = seatId;
        }

        protected override string Description => "Stagiaire à définir pour une place envoyé";
    }
}