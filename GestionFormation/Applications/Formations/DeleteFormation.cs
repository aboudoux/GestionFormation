using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain.Formateurs;
using GestionFormation.CoreDomain.Formateurs.Exceptions;
using GestionFormation.CoreDomain.Formations;
using GestionFormation.CoreDomain.Lieux;
using GestionFormation.CoreDomain.Lieux.Exceptions;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.CoreDomain.Sessions.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Formations
{
    public class DeleteFormation : ActionCommand
    {
        private readonly ISessionQueries _sessionQueries;

        public DeleteFormation(EventBus eventBus, ISessionQueries sessionQueries) : base(eventBus)
        {
            _sessionQueries = sessionQueries ?? throw new ArgumentNullException(nameof(sessionQueries));
        }

        public void Execute(Guid formationId)
        {            
            var aggregatesToPublish = DeleteAllAssociatedSessionAndUnassignFormateursAndLieux(formationId);

            var formation = GetAggregate<Formation>(formationId);
            formation.Delete();
            
            aggregatesToPublish.Add(formation);
            
            PublishUncommitedEvents(aggregatesToPublish.ToArray());
        }

        private List<AggregateRoot> DeleteAllAssociatedSessionAndUnassignFormateursAndLieux(Guid formationId)
        {
            var aggregatesToPublish = new List<AggregateRoot>();
            var formateursUnassigner = new Unassigner<Formateur, FormateurNotExistsException>(EventBus);
            var lieuUnassigner = new Unassigner<Lieu, LieuNotExistsException>(EventBus);

            var allSessions = _sessionQueries.GetAll(formationId);

            foreach (var sessionResult in allSessions)
            {
                formateursUnassigner.Unassign(sessionResult.FormateurId, sessionResult.DateDebut, sessionResult.Durée);
                lieuUnassigner.Unassign(sessionResult.LieuId, sessionResult.DateDebut, sessionResult.Durée);

                var session = GetAggregate<Session>(sessionResult.SessionId);
                session.Delete();
                aggregatesToPublish.Add(session);
            }

            aggregatesToPublish.AddRange(formateursUnassigner.GetUnassignedAggregates());
            aggregatesToPublish.AddRange(lieuUnassigner.GetUnassignedAggregates());

            return aggregatesToPublish;
        }

        private class Unassigner<TAggregate, TException> : ActionCommand
            where TAggregate : AggregateRoot, IAssignable
            where TException :  DomainException, new()
        {
            private readonly Dictionary<Guid, TAggregate> _loadedAggregates = new Dictionary<Guid, TAggregate>();

            public void Unassign(Guid? aggregateId, DateTime dateDebut, int durée)
            {
                if(!aggregateId.HasValue) return;

                var id = aggregateId.Value;
                if (!_loadedAggregates.ContainsKey(id))
                {
                    var aggregate = GetAggregate<TAggregate>(id);
                    if (aggregate == null)
                        throw new TException();
                    _loadedAggregates.Add(id, aggregate);
                }
                _loadedAggregates[id].UnAssign(dateDebut, durée);
            }

            public IEnumerable<AggregateRoot> GetUnassignedAggregates()
            {
                return _loadedAggregates.Values;
            }

            public Unassigner(EventBus eventBus) : base(eventBus)
            {
            }
        }
    }
}