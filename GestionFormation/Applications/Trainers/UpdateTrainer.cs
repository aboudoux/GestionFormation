using System;
using GestionFormation.CoreDomain.Trainers;
using GestionFormation.CoreDomain.Trainers.Exceptions;
using GestionFormation.CoreDomain.Trainers.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Trainers
{
    public class UpdateTrainer : ActionCommand
    {
        private readonly ITrainerQueries _trainerQueries;

        public UpdateTrainer(EventBus eventBus, ITrainerQueries trainerQueries) : base(eventBus)
        {
            _trainerQueries = trainerQueries ?? throw new ArgumentNullException(nameof(trainerQueries));
        }

        public void Execute(Guid trainerId, string lastname, string firsname, string email)
        {
            var existingTrainerId = _trainerQueries.GetTrainer(lastname, firsname);
            if( existingTrainerId.HasValue && existingTrainerId.Value != trainerId)
                throw new TrainerAlreadyExistsException();

            var trainer = GetAggregate<Trainer>(trainerId);
            trainer.Update(lastname, firsname, email);
            PublishUncommitedEvents(trainer);
        }
    }
}