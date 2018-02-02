using System;
using GestionFormation.CoreDomain.Reminders.Projections;

namespace GestionFormation.CoreDomain.Reminders.Queries
{
    public class ReminderResult : IReminderResult
    {
        public ReminderResult(ReminderSqlEntity entity)
        {
            Label = entity.Label;
            ReminderType = entity.ReminderType;
            SeatId = entity.SeatId;
            SessionId = entity.SessionId;
            SessionId = entity.SessionId;
            AgreementId = entity.AgreementId;
        }

        public string Label { get; }
        public RappelType ReminderType { get; }
        public Guid? SeatId { get; set; }
        public Guid? SessionId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? AgreementId { get; set; }
    }
}