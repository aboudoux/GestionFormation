using System;
using GestionFormation.CoreDomain.Trainings;
using GestionFormation.CoreDomain.Trainings.Exceptions;
using GestionFormation.CoreDomain.Trainings.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Formations
{
    public class UpdateFormation : ActionCommand
    {
        private readonly ITrainingQueries _queries;

        public UpdateFormation(EventBus eventBus, ITrainingQueries queries) : base(eventBus)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        public void Execute(Guid formationId, string newName, int places)
        {
            var foundFormation = _queries.GetTrainingId(newName);
            if (foundFormation.HasValue && foundFormation.Value != formationId)
                throw new TrainingAlreadyExistsException(newName);

            var formation = GetAggregate<Training>(formationId);
            formation.Update(newName, places);
            PublishUncommitedEvents(formation);
        }
    }
}