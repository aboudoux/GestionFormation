using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Users;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;

namespace GestionFormation.CoreDomain.Reminders.Queries
{
    public class ReminderSqlQueries : IReminderQueries
    {
        public IEnumerable<IReminderResult> GetAll(UserRole role)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Reminders.Where(a => a.AffectedRole == role).ToList().Select(a => new ReminderResult(a));
            }
        }
    }
}