using System;

namespace GestionFormation.CoreDomain.Sessions.Queries
{
    public class ClosedSessionResultResult : IClosedSessionResult
    {
        public ClosedSessionResultResult(string trainingName, DateTime start, Guid? surveyId, Guid? timesheetId)
        {
            TrainingName = trainingName;
            Start = start;
            SurveyId = surveyId;
            TimesheetId = timesheetId;
        }
        public string TrainingName { get; }
        public DateTime Start { get; }
        public Guid? SurveyId { get; }
        public Guid? TimesheetId { get; }
    }
}