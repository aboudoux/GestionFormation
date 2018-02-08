using System;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Students
{
    public class ReportMissingStudent : ActionCommand
    {
        public ReportMissingStudent(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid sessionId, Guid studentId)
        {
            var session = GetAggregate<Session>(sessionId);
            session.ReportMissingStudent(studentId);
            PublishUncommitedEvents(session);
        }
    }
}