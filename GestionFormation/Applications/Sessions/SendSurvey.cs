using System;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Sessions
{
    public class SendSurvey : ActionCommand
    {
        public SendSurvey(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid sessionId, Guid documentId)
        {
            var session = GetAggregate<Session>(sessionId);
            session.SendSurvey(documentId);
            PublishUncommitedEvents(session);
        }
    }
}