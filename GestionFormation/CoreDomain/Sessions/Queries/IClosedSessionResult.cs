using System;

namespace GestionFormation.CoreDomain.Sessions.Queries
{
    public interface IClosedSessionResult
    {
        string TrainingName { get; }
        DateTime Start { get; }
        Guid? SurveyId { get; }
        Guid? TimesheetId { get; }
    }
}