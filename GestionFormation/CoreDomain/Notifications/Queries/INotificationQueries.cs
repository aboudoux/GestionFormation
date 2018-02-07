using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain.Users;

namespace GestionFormation.CoreDomain.Notifications.Queries
{
    public interface INotificationQueries
    {
        IEnumerable<INotificationResult> GetAll(UserRole role);
        Guid GetNotificationManagerId(Guid sessionId);
    }
}
