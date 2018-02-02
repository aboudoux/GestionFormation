using System;
using GestionFormation.CoreDomain.Trainers;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Trainers
{
    public class DeleteTrainer : ActionCommand
    {
        public DeleteTrainer(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid trainerId)
        {
            var formateur = GetAggregate<Trainer>(trainerId);
            formateur.Delete();
            PublishUncommitedEvents(formateur);
        }
    }
}