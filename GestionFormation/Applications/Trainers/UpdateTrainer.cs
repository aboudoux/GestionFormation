using System;
using GestionFormation.CoreDomain.Trainers;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Trainers
{
    public class UpdateTrainer : ActionCommand
    {
        public UpdateTrainer(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid trainerId, string lastname, string firsname, string email)
        {
            var trainer = GetAggregate<Trainer>(trainerId);
            trainer.Update(lastname, firsname, email);
            PublishUncommitedEvents(trainer);
        }
    }
}