using System;
using GestionFormation.CoreDomain.Reminders.Projections;

namespace GestionFormation.CoreDomain.Reminders.Queries
{
    public interface IReminderResult
    {
        string Label { get; }
        
        RappelType ReminderType { get; }

        Guid? SeatId { get; set; }
        Guid? SessionId { get; set; }
        Guid? CompanyId { get; set; }
        Guid? AgreementId { get; set; }
    }
}