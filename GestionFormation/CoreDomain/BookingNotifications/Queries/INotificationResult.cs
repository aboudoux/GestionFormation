using System;
using GestionFormation.CoreDomain.BookingNotifications.Projections;

namespace GestionFormation.CoreDomain.BookingNotifications.Queries
{
    public interface INotificationResult
    {
        Guid AggregateId { get; }
        string Label { get; }
        
        BookingNotificationType BookingNotificationType { get; }

        Guid? SeatId { get; }
        Guid SessionId { get; }
        Guid CompanyId { get; }
        Guid? AgreementId { get; }
    }
}