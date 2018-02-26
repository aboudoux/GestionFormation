using System;

using GestionFormation.CoreDomain.Trainings;
using GestionFormation.CoreDomain.Trainings.Exceptions;
using GestionFormation.CoreDomain.Trainings.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Trainings
{
    public class CreateTraining : ActionCommand
    {
        private readonly ITrainingQueries _queries;

        public CreateTraining(EventBus eventBus, ITrainingQueries queries) : base(eventBus)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }
        public Training Execute(string name, int seats, int color)
        {            
            if (_queries.GetTrainingId(name).HasValue)
                throw new TrainingAlreadyExistsException(name);
            
            var formation = Training.Create(name, seats, color);
            PublishUncommitedEvents(formation);
            return formation;
        }
    }
}
