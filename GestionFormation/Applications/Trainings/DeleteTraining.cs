using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain.Locations;
using GestionFormation.CoreDomain.Locations.Exceptions;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.CoreDomain.Sessions.Queries;
using GestionFormation.CoreDomain.Trainers;
using GestionFormation.CoreDomain.Trainers.Exceptions;
using GestionFormation.CoreDomain.Trainings;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Trainings
{
    public class DeleteTraining : ActionCommand
    {
        private readonly ISessionQueries _sessionQueries;

        public DeleteTraining(EventBus eventBus, ISessionQueries sessionQueries) : base(eventBus)
        {
            _sessionQueries = sessionQueries ?? throw new ArgumentNullException(nameof(sessionQueries));
        }

        public void Execute(Guid trainingId)
        {            
            var aggregatesToPublish = DeleteAllAssociatedSessionAndUnassignFormateursAndLieux(trainingId);

            var training = GetAggregate<Training>(trainingId);
            training.Delete();
            
            aggregatesToPublish.Add(training);
            
            PublishUncommitedEvents(aggregatesToPublish.ToArray());
        }

        private List<AggregateRoot> DeleteAllAssociatedSessionAndUnassignFormateursAndLieux(Guid formationId)
        {
            var aggregatesToPublish = new List<AggregateRoot>();
            var trainersUnassigner = new Unassigner<Trainer, TrainerNotExistsException>(EventBus);
            var locationUnassigner = new Unassigner<Location, LocationNotExistsException>(EventBus);

            var allSessions = _sessionQueries.GetAll(formationId);

            foreach (var sessionResult in allSessions)
            {
                trainersUnassigner.Unassign(sessionResult.TrainerId, sessionResult.SessionStart, sessionResult.Duration);
                locationUnassigner.Unassign(sessionResult.LocationId, sessionResult.SessionStart, sessionResult.Duration);

                var session = GetAggregate<Session>(sessionResult.SessionId);
                session.Delete();
                aggregatesToPublish.Add(session);
            }

            aggregatesToPublish.AddRange(trainersUnassigner.GetUnassignedAggregates());
            aggregatesToPublish.AddRange(locationUnassigner.GetUnassignedAggregates());

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