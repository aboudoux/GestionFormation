using System;
using GestionFormation.CoreDomain.Locations;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.CoreDomain.Trainers;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Sessions
{
    public class UpdateSession : ActionCommand
    {
        public UpdateSession(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid sessionId, Guid trainingId, DateTime start, int duration, int nbrSeats, Guid? locationId, Guid? trainerId)
        {
            var session = GetAggregate<Session>(sessionId);
            if (session == null)
                throw new SessionNotExistsException(sessionId);           

            var lieux = Update<Location>(locationId, session, session.LocationId, start, duration);
            var formateurs = Update<Trainer>(trainerId, session, session.TrainerId, start, duration);            

            session.Update(trainingId, start, duration, nbrSeats, locationId, trainerId);

            PublishUncommitedEvents(lieux.Item1, lieux.Item2, formateurs.Item1, formateurs.Item2, session);
        }

        private Tuple<TAggregate, TAggregate> Update<TAggregate>(Guid? aggregateId, Session session, Guid? aggregateIdInSession, DateTime dateDebut, int durée) where TAggregate : AggregateRoot, IAssignable
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            TAggregate source = null;
            TAggregate dest = null;

            if (aggregateId != aggregateIdInSession)
            {
                source = aggregateIdInSession.HasValue ? GetAggregate<TAggregate>(aggregateIdInSession.Value) : null;
                dest = aggregateId.HasValue ? GetAggregate<TAggregate>(aggregateId.Value) : null;

                source?.UnAssign(dateDebut, durée);
                dest?.Assign(dateDebut, durée);
            }
            else if ((session.SessionStart != dateDebut || session.Duration != durée) && aggregateId.HasValue)
            {
                dest = aggregateId.HasValue ? GetAggregate<TAggregate>(aggregateId.Value) : null;
                dest?.ChangeAssignation(session.SessionStart, session.Duration, dateDebut, durée);
            }

            return Tuple.Create(source, dest);
        }
    }
}