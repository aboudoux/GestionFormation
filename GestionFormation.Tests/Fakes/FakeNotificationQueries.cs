using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.CoreDomain.Users;

namespace GestionFormation.Tests.Fakes
{
    public class FakeNotificationQueries : INotificationQueries
    {
        public IEnumerable<INotificationResult> GetAll(UserRole role)
        {
            throw new NotImplementedException();
        }

        public Guid GetNotificationManagerId(Guid sessionId)
        {
            return Guid.NewGuid();
        }
    }
}