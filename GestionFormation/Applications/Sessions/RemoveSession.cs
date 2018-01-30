using System;
using GestionFormation.CoreDomain.Locations;
using GestionFormation.CoreDomain.Locations.Exceptions;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.CoreDomain.Trainers;
using GestionFormation.CoreDomain.Trainers.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Sessions
{
    public class RemoveSession : ActionCommand
    {
        public RemoveSession(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid sessionId)
        {
            Trainer trainer = null;
            var session = GetAggregate<Session>(sessionId);

            if (session.TrainerId.HasValue)
            {
                trainer = GetAggregate<Trainer>(session.TrainerId.Value);
                if( trainer == null )
                    throw new TrainerNotExistsException();
                trainer.UnAssign(session.SessionStart, session.Duration);
            }

            Location location = null;
            if (session.LocationId.HasValue)
            {
                location = GetAggregate<Location>(session.LocationId.Value);
                if( location == null )
                    throw  new LocationNotExistsException();
                location.UnAssign(session.SessionStart, session.Duration);
            }

            session.Delete();
            PublishUncommitedEvents(trainer, location, session);
        }
    }
}