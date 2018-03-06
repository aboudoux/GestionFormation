using System;
using GestionFormation.CoreDomain.Trainers;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Trainers
{
    public class DisableTrainer : ActionCommand
    {
        public DisableTrainer(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid trainerId)
        {
            var trainer = GetAggregate<Trainer>(trainerId, true);
            trainer.Disable();
            PublishUncommitedEvents(trainer);
        }
    }
}