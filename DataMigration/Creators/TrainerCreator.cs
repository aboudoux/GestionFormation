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

            if(Mapper.Exists(trainerName))
                return;

            var nom = string.Empty;
            var prenom = string.Empty;

            var splitterName = trainerName.Split('.');
            if (splitterName.Length != 2)
                splitterName = trainerName.Split(' ');

            if (splitterName.Length == 2)
            {
                prenom = splitterName[0];
                nom = splitterName[1];
            }
            else
            {
                nom = trainerName;
                prenom = "NC";
            }

            var trainer = App.Command<CreateTrainer>().Execute(nom, prenom, string.Empty);
            Mapper.Add(trainerName, trainer.AggregateId);
        }
    }
}