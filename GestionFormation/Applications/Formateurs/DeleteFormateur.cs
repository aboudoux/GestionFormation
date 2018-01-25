using System;
using GestionFormation.CoreDomain.Formateurs;
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
            var formateur = GetAggregate<Formateur>(formateurId);
            formateur.Delete();
            PublishUncommitedEvents(formateur);
        }
    }
}