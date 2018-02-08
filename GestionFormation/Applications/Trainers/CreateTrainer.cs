using System;
using GestionFormation.CoreDomain.Trainers;
using GestionFormation.CoreDomain.Trainers.Exceptions;
using GestionFormation.CoreDomain.Trainers.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Trainers
{
    public class CreateTrainer : ActionCommand
    {
        private readonly ITrainerQueries _trainerQueries;

        public CreateTrainer(EventBus eventBus, ITrainerQueries trainerQueries) : base(eventBus)
        {
            _trainerQueries = trainerQueries ?? throw new ArgumentNullException(nameof(trainerQueries));
        }

        public Trainer Execute(string lastname, string firstname, string email)
        {
            if(_trainerQueries.Exists(lastname, firstname))
                throw new TrainerAlreadyExistsException();

            var trainer = Trainer.Create(lastname, firstname, email);
            PublishUncommitedEvents(trainer);
            return trainer;
        }
    }
}
