using System;
using GestionFormation.CoreDomain.Students;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Students
{
    public class DeleteStudent : ActionCommand
    {
        public DeleteStudent(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid studentId)
        {
            var student = GetAggregate<Student>(studentId);
            student.Delete();
            PublishUncommitedEvents(student);
        }
    }
}