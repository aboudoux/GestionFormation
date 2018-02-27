using GestionFormation.CoreDomain.Sessions.Queries;

namespace GestionFormation.App.Views.Seats
{
    public class SessionInfos
    {
        public ICompleteSessionResult Result { get; }

        public SessionInfos(ICompleteSessionResult result)
        {
            Result = result;
            TrainingName = result.Training;
            TrainerName = result.Trainer.ToString();
            TrainingLocation = result.Location;
            TrainingDuration = $"Le {result.SessionStart:d} sur {result.Duration} jour(s)";
        }
        public string TrainingName { get; }
        public string TrainingDuration { get; }
        public string TrainerName { get; }
        public string TrainingLocation { get; }
    }
}