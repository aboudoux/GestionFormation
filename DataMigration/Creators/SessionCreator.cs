using System;
using GestionFormation.Applications.Seats;
using GestionFormation.Applications.Sessions;
using GestionFormation.Kernel;

namespace DataMigration.Creators
{
    public class SessionCreator : Creator
    {
        private readonly TrainingCreator _trainingCreator;
        private readonly TrainerCreator _trainerCreator;
        private readonly LocationCreator _locationCreator;
        private readonly StudentCreator _studentCreator;
        private readonly CompanyCreator _companyCreator;

        public SessionCreator(ApplicationService applicationService, TrainingCreator trainingCreator, TrainerCreator trainerCreator, LocationCreator locationCreator, StudentCreator studentCreator, CompanyCreator companyCreator) : base(applicationService)
        {
            _trainingCreator = trainingCreator ?? throw new ArgumentNullException(nameof(trainingCreator));
            _trainerCreator = trainerCreator ?? throw new ArgumentNullException(nameof(trainerCreator));
            _locationCreator = locationCreator ?? throw new ArgumentNullException(nameof(locationCreator));
            _studentCreator = studentCreator ?? throw new ArgumentNullException(nameof(studentCreator));
            _companyCreator = companyCreator ?? throw new ArgumentNullException(nameof(companyCreator));
        }

        public void Create(DateTime startSession, int duration, string training, string trainer, string location, string student, string company)
        {
            if(training.IsEmpty() || trainer.IsEmpty() || location.IsEmpty() || student.IsEmpty() || company.IsEmpty())
                return;

            var key = $"{training}-{startSession:d}";            
            try
            {
                if (!Mapper.Exists(key))
                {
                    var session = App.Command<PlanSession>().Execute(
                        _trainingCreator.GetId(_trainingCreator.ConstructKey(training)),
                        startSession, duration, 20,
                        _locationCreator.GetId(_locationCreator.ConstructKey(location)),
                        _trainerCreator.GetId(_trainerCreator.ConstructKey(trainer)));

                    Mapper.Add(key, session.AggregateId);
                }                    

                var seat = App.Command<ReserveSeat>().Execute(Mapper.GetId(key),
                    _studentCreator.GetId(_studentCreator.ConstructKey(student)),
                    _companyCreator.GetCompanyId(_companyCreator.ConstructKey(company)), false);

                App.Command<ValidateSeat>().Execute(seat.AggregateId, false);

            }
            catch (DomainException e)
            {
                Console.WriteLine(e.Message);                
            }
        }        
    }
}