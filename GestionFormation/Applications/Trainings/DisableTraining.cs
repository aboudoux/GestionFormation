using System;
using System.Collections.Generic;
using System.Text;
using GestionFormation.CoreDomain.Trainings;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Trainings
{
    public class DisableTraining : ActionCommand
    {
        public DisableTraining(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid trainingId)
        {
            var training = GetAggregate<Training>(trainingId);
            training.Disable();
            PublishUncommitedEvents(training);
        }
    }
}
