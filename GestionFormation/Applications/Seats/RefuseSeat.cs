using System;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Seats
{
    public class RefuseSeat : ActionCommand
    {
        private readonly INotificationQueries _notificationManagerQueries;

        public RefuseSeat(EventBus eventBus, INotificationQueries notificationManagerQueries) : base(eventBus)
        {
            _notificationManagerQueries = notificationManagerQueries ?? throw new ArgumentNullException(nameof(notificationManagerQueries));
        }

        public void Execute(Guid seatId, string reason)
        {
            var seat = GetAggregate<Seat>(seatId);
            seat.Refuse(reason);

            var managerId = _notificationManagerQueries.GetNotificationManagerId(seat.SessionId);
            var manager = GetAggregate<NotificationManager>(managerId);
            manager.SignalSeatRefused(seatId, seat.CompanyId);

            Agreement agreement = null;
            if (seat.AssociatedAgreementId.HasValue)
            {
                agreement = GetAggregate<Agreement>(seat.AssociatedAgreementId.Value);
                agreement.Revoke();
                manager.SignalAgreementRevoked(agreement.AggregateId);
            }

            var session = GetAggregate<Session>(seat.SessionId);
            if(seat.StudentId.HasValue)
                session.ReleaseSeat(seat.StudentId.Value);

            PublishUncommitedEvents(seat, agreement, session, manager);
        }
    }
}