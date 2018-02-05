using System;
using GestionFormation.CoreDomain.Trainings;
using GestionFormation.CoreDomain.Trainings.Exceptions;
using GestionFormation.CoreDomain.Trainings.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Trainings
{
    public class UpdateTraining : ActionCommand
    {
        private readonly ITrainingQueries _queries;

        public UpdateTraining(EventBus eventBus, ITrainingQueries queries) : base(eventBus)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        public void Execute(Guid trainingId, string newName, int seats)
        {
            var foundTraining = _queries.GetTrainingId(newName);
            if (foundTraining.HasValue && foundTraining.Value != trainingId)
                throw new TrainingAlreadyExistsException(newName);

            var training = GetAggregate<Training>(trainingId);
            training.Update(newName, seats);
            PublishUncommitedEvents(training);
        }
    }
}