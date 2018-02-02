using GestionFormation.CoreDomain.Trainers;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Trainers
{
    public class CreateTrainer : ActionCommand
    {
        public CreateTrainer(EventBus eventBus) : base(eventBus)
        {
        }

        public Trainer Execute(string lastname, string firstname, string email)
        {
            var trainer = Trainer.Create(lastname, firstname, email);
            PublishUncommitedEvents(trainer);
            return trainer;
        }
    }
}
