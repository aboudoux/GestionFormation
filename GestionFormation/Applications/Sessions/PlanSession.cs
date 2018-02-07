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
        public Session Execute(Guid formationId, DateTime debut, int durée, int nombrePlace, Guid? lieuId, Guid? formateurId)
        {
            Trainer trainer = null;
            if (formateurId.HasValue)
            {
                trainer = GetAggregate<Trainer>(formateurId.Value);
                if( trainer == null)
                    throw new TrainerNotExistsException();
                trainer.Assign(debut, durée);
            }

            Location location = null;
            if (lieuId.HasValue)
            {
                location = GetAggregate<Location>(lieuId.Value);
                if (location == null)
                    throw new LocationNotExistsException();
                location.Assign(debut, durée);
            }
            
            var session = Session.Plan(formationId, debut, durée, nombrePlace, lieuId, formateurId);
            var notification = NotificationManager.Create(session.AggregateId);

            PublishUncommitedEvents(trainer, location, session, notification);

            return session;
        }
    }    
    
}