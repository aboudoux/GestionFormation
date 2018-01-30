using System;
using GestionFormation.CoreDomain.Trainers;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Formateurs
{
    public class DeleteFormateur : ActionCommand
    {
        public DeleteFormateur(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid formateurId)
        {
            var formateur = GetAggregate<Trainer>(formateurId);
            formateur.Delete();
            PublishUncommitedEvents(formateur);
        }
    }
}