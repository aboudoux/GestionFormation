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
            var fullname = new Name(name);

            if (Mapper.Exists(fullname.ToString())) return;
                                  
            var student = App.Command<CreateStudent>().Execute(fullname.Lastname, fullname.Firstname);
            Mapper.Add(fullname.ToString(), student.AggregateId);
        }
    }
}