using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain.Users;

namespace GestionFormation.CoreDomain.BookingNotifications.Queries
{
    public interface INotificationQueries
    {
        IEnumerable<INotificationResult> GetAll(UserRole role);
        IEnumerable<INotificationResult> GetAll(Guid sessionId, Guid companyId);
        IEnumerable<INotificationResult> GetFromAgreement(Guid agreementId);
        IEnumerable<INotificationResult> GetFromSeat(Guid seatId);
    }
}
