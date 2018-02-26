using GestionFormation.Applications.Trainings;

namespace DataMigration.Creators
{
    public class TrainingCreator : Creator
    {
        public TrainingCreator(ApplicationService applicationService) : base(applicationService)
        {
        }

        public void Create(string trainingName)
        {
            if (Mapper.Exists(trainingName)) return;

            var training = App.Command<CreateTraining>().Execute(trainingName, 8, 0);
            Mapper.Add(trainingName, training.AggregateId);
        }
    }
}