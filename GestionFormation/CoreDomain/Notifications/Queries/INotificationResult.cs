using System;

namespace GestionFormation.CoreDomain.Notifications.Queries
{
    public interface INotificationResult
    {
        Guid AggregateId { get; }
        string Label { get; }
        
        NotificationType NotificationType { get; }

        Guid? SeatId { get; }
        Guid SessionId { get; }
        Guid CompanyId { get; }
        Guid? AgreementId { get; }
    }
}