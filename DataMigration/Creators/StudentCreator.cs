using GestionFormation.Applications.Students;

namespace DataMigration.Creators
{
    public class StudentCreator : Creator
    {
        public StudentCreator(ApplicationService applicationService) : base(applicationService)
        {
        }

        public void Create(string name)
        {
            if(string.IsNullOrWhiteSpace(name)) return;
            if(Mapper.Exists(name)) return;
                      
            var fullname = new NameResolver(name);

            var student = App.Command<CreateStudent>().Execute(fullname.Lastname, fullname.Firstname);
            Mapper.Add(name, student.AggregateId);
        }
    }
}