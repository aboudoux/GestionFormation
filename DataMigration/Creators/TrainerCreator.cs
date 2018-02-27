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
            if(string.IsNullOrWhiteSpace(trainerName))
                return;

            var tn = new Name(trainerName);

            if (Mapper.Exists(tn.ToString()))
                return;            

            var trainer = App.Command<CreateTrainer>().Execute(tn.Lastname, tn.Firstname, string.Empty);
            Mapper.Add(tn.ToString(), trainer.AggregateId);
        }
    }
}