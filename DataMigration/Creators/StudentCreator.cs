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
            if(name.IsEmpty()) return;
            
            if (Mapper.Exists(ConstructKey(name))) return;

            var fullname = new Name(name);
            var student = App.Command<CreateStudent>().Execute(fullname.Lastname, fullname.Firstname);

            Mapper.Add(ConstructKey(name), student.AggregateId);
        }

        public override string ConstructKey(string source)
        {
            var fullname = new Name(source);
            return fullname.ToString();
        }
    }
}