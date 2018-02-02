using System.Collections.Generic;
using GestionFormation.CoreDomain.Users;

namespace GestionFormation.CoreDomain.Reminders.Queries
{
    public interface IReminderQueries
    {
        IEnumerable<IReminderResult> GetAll(UserRole role);
    }
}
