using GestionFormation.Applications.Trainers;

namespace DataMigration.Creators
{
    public class TrainerCreator : Creator
    {
        public TrainerCreator(ApplicationService applicationService) : base(applicationService)
        {
        }

        public void Create(string trainerName)
        {
            if(trainerName.IsEmpty())
                return;
            
            if (Mapper.Exists(ConstructKey(trainerName)))
                return;

            var tn = new Name(trainerName);

            var trainer = App.Command<CreateTrainer>().Execute(tn.Lastname, tn.Firstname, string.Empty);
            Mapper.Add(ConstructKey(trainerName), trainer.AggregateId);
        }

        public override string ConstructKey(string source)
        {
            var tn = new Name(source);
            return tn.ToString();
        }
    }
}