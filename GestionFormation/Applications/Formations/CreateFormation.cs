using System;
using GestionFormation.CoreDomain.Trainings;
using GestionFormation.CoreDomain.Trainings.Exceptions;
using GestionFormation.CoreDomain.Trainings.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Formations
{
    public class CreateFormation : ActionCommand
    {
        private readonly ITrainingQueries _queries;

        public CreateFormation(EventBus eventBus, ITrainingQueries queries) : base(eventBus)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }
        public Training Execute(string nom, int places)
        {            
            if (_queries.GetTrainingId(nom).HasValue)
                throw new TrainingAlreadyExistsException(nom);
            
            var formation = Training.Create(nom, places);
            PublishUncommitedEvents(formation);
            return formation;
        }
    }
}
