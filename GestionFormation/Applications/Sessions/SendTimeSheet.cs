using System;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Sessions
{
    public class SendTimeSheet : ActionCommand
    {
        public SendTimeSheet(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid sessionId, Guid documentId)
        {
            var session = GetAggregate<Session>(sessionId);
            session.SendTimesheet(documentId);
            PublishUncommitedEvents(session);
        }
    }
}