﻿using System;
using GestionFormation.CoreDomain.Formateurs;
using GestionFormation.CoreDomain.Lieux;
using GestionFormation.CoreDomain.Sessions;
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

            var lieux = Update<Lieu>(lieuId, session, session.LieuId, dateDebut, durée);
            var formateurs = Update<Formateur>(formateurId, session, session.FormateurId, dateDebut, durée);            

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
            else if ((session.DateDebut != dateDebut || session.Durée != durée) && aggregateId.HasValue)
            {
                dest = aggregateId.HasValue ? GetAggregate<TAggregate>(aggregateId.Value) : null;
                dest?.ChangeAssignation(session.DateDebut, session.Durée, dateDebut, durée);
            }

            return Tuple.Create(source, dest);
        }
    }
}