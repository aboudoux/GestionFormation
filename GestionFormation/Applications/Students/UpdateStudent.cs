using System;
using GestionFormation.CoreDomain.Students;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Students
{
    public class UpdateStudent : ActionCommand
    {
        public UpdateStudent(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid studentId, string lastname, string firstname)
        {
            var stagiaire = GetAggregate<Student>(studentId);
            stagiaire.Update(lastname, firstname);
            PublishUncommitedEvents(stagiaire);
        }
    }
}