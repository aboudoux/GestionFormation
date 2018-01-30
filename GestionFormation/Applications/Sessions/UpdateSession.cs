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

        public void Execute(Guid sessionId, Guid formationId, DateTime dateDebut, int durée, int nombrePlace, Guid? lieuId, Guid? formateurId)
        {
            var session = GetAggregate<Session>(sessionId);
            if (session == null)
                throw new SessionNotExistsException(sessionId);           

            var lieux = Update<Location>(lieuId, session, session.LocationId, dateDebut, durée);
            var formateurs = Update<Trainer>(formateurId, session, session.TrainerId, dateDebut, durée);            

            session.Update(formationId, dateDebut, durée, nombrePlace, lieuId, formateurId);

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