using GestionFormation.CoreDomain.Students;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Students
{
    public class CreateStudent: ActionCommand
    {
        public CreateStudent(EventBus eventBus) : base(eventBus)
        {
        }

        public Student Execute(string lastname, string firstname)
        {
            var student = Student.Create(lastname, firstname);
            PublishUncommitedEvents(student);
            return student;
        }
    }
}
