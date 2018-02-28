using System;
using GestionFormation.CoreDomain.Locations;
using GestionFormation.CoreDomain.Locations.Exceptions;
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.CoreDomain.Trainers;
using GestionFormation.CoreDomain.Trainers.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Sessions
{
    public class PlanSession : ActionCommand
    {
        public PlanSession(EventBus eventBus) : base(eventBus)
        { 
        }
        public Session Execute(Guid trainingId, DateTime start, int duration, int nbrSeats, Guid? locationId, Guid? trainerId)
        {
            Trainer trainer = null;
            if (trainerId.HasValue)
            {
                trainer = GetAggregate<Trainer>(trainerId.Value);
                if( trainer == null)
                    throw new TrainerNotExistsException();
                trainer.Assign(start, duration);
            }

            Location location = null;
            if (locationId.HasValue)
            {
                location = GetAggregate<Location>(locationId.Value);
                if (location == null)
                    throw new LocationNotExistsException();
                location.Assign(start, duration);
            }
            
            var session = Session.Plan(trainingId, start, duration, nbrSeats, locationId, trainerId);
            var notification = NotificationManager.Create(session.AggregateId);

            PublishUncommitedEvents(trainer, location, session, notification);

            return session;
        }
    }    
    
}