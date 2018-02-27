using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GestionFormation.App.Core;
using GestionFormation.App.Views.EditableLists;
using GestionFormation.Applications.Locations;
using GestionFormation.Applications.Sessions;
using GestionFormation.Applications.Trainers;
using GestionFormation.Applications.Trainings;
using GestionFormation.CoreDomain.Locations.Queries;
using GestionFormation.CoreDomain.Trainers.Queries;
using GestionFormation.CoreDomain.Trainings.Queries;

namespace GestionFormation.App.Views.Sessions
{
    public class CreateSessionWindowVm : PopupWindowVm
    {
        private readonly AppointmentItem _appointmentItem;
        private readonly IApplicationService _applicationService;
        private readonly ILocationQueries _locationQueries;
        private readonly ITrainerQueries _trainerQueries;
        private readonly ITrainingQueries _trainingQueries;
        private ObservableCollection<TrainingItem> _trainings;
        private TrainingItem _selectedTraining;
        private ObservableCollection<TrainerItem> _trainers;
        private TrainerItem _selectedTrainer;
        private ObservableCollection<Location> _locations;
        private Location _selectedLocation;
        private DateTime? _startSession;
        private int _duration;
        private int _seats;
        private Guid? _sessionId;        

        public override string Title => "Planifier une nouvelle session";

        public CreateSessionWindowVm(IApplicationService applicationService, ITrainingQueries trainingQueries, ITrainerQueries trainerQueries, ILocationQueries locationQueries, AppointmentItem appointmentItem)
        {
            _appointmentItem = appointmentItem ?? throw new ArgumentNullException(nameof(appointmentItem));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _locationQueries = locationQueries ?? throw new ArgumentNullException(nameof(locationQueries));
            _trainerQueries = trainerQueries ?? throw new ArgumentNullException(nameof(trainerQueries));
            _trainingQueries = trainingQueries ?? throw new ArgumentNullException(nameof(trainingQueries));

            AddTrainingCommand = new RelayCommandAsync(ExecuteAddTrainingAsync);
            AddLocationCommand = new RelayCommandAsync(ExecuteAddLocationAsync);
            AddTrainerCommand = new RelayCommandAsync(ExecuteAddTrainerAsync);
        }        

        public ObservableCollection<TrainingItem> Trainings
        {
            get => _trainings;
            set{Set(() => Trainings, ref _trainings, value);}
        }

        public TrainingItem SelectedTraining
        {
            get => _selectedTraining;
            set
            {
                Set(() => SelectedTraining, ref _selectedTraining, value);

                if(value == null) return;
                if (Seats == 0 || value.Seats < Seats)
                    Seats = value.Seats;
            }
        }

        public ObservableCollection<TrainerItem> Trainers
        {
            get => _trainers;
            set { Set(() => Trainers, ref _trainers, value); }
        }

        public TrainerItem SelectedTrainer
        {
            get => _selectedTrainer;
            set { Set(() => SelectedTrainer, ref _selectedTrainer, value); }
        }

        public ObservableCollection<Location> Locations
        {
            get => _locations;
            set { Set(() => Locations, ref _locations, value); }
        }

        public Location SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                Set(()=>SelectedLocation, ref _selectedLocation, value );

                if (value == null) return;
                if (Seats == 0 || value.Seats < Seats)
                    Seats = value.Seats;
            }
        }

        protected override async Task ExecuteValiderAsync()
        {
            var error = false;
            if (SelectedTraining == null)
            {
                MessageBox.Show("Vous n'avez pas renseigné la formation", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                error = true;
            }
            else if (SelectedTrainer == null)
            {
                MessageBox.Show("Vous n'avez pas renseigné le formateur", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                error = true;
            }
            else if (SelectedLocation == null)
            {
                MessageBox.Show("Vous n'avez pas renseigné le lieu", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                error = true;
            }

            if (Seats > SelectedTraining?.Seats)
            {
                if (MessageBox.Show("Vous avez défini plus de places que ne peut en accueillir la formation.\r\nEtes-vous sûr de vouloir valider ?", "ATTENTION", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            }
            if (Seats > SelectedLocation?.Seats)
            {
                if (MessageBox.Show("Vous avez défini plus de places que ne peut en accueillir la lieu.\r\nEtes-vous sûr de vouloir valider ?", "ATTENTION", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            }

            if (!error)
            {
                try
                {
                    if (_sessionId.HasValue)
                        await Task.Run(() => _applicationService.Command<UpdateSession>().Execute(_sessionId.Value, SelectedTraining.Id, StartSession.Value, Duration, Seats, SelectedLocation.Id, SelectedTrainer.Id));
                    else
                        await Task.Run(() => _applicationService.Command<PlanSession>().Execute(SelectedTraining.Id, StartSession.Value, Duration, Seats, SelectedLocation.Id, SelectedTrainer.Id));

                    await base.ExecuteValiderAsync();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.LastMessage(), "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
            
        public DateTime? StartSession
        {
            get => _startSession;
            set { Set(() => StartSession, ref _startSession, value); }
        }

        public int Duration
        {
            get => _duration;
            set { Set(()=>Duration, ref _duration, value); }
        }

        public int Seats
        {
            get => _seats;
            set
            {
                Set(() => Seats, ref _seats, value);                
            }
        }

        public RelayCommandAsync AddTrainingCommand { get; }
        private async Task ExecuteAddTrainingAsync()
        {
            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer une formation", new EditableTraining());
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync( async ()=>{
                    var item = vm.Item as EditableTraining;
                    var newItem = await Task.Run(() => _applicationService.Command<CreateTraining>().Execute(item.Name, item.Seats, ColorHelper.ToInt(item.Color)));
                    await InitFormations(newItem.AggregateId);
                });
            }
        }

        public RelayCommandAsync AddLocationCommand { get; }
        private async Task ExecuteAddLocationAsync()
        {
            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer un lieu", new EditableLocation());
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync(async () => {                   
                    var item = vm.Item as EditableLocation;
                    var newItem = await Task.Run(() => _applicationService.Command<CreateLocation>().Execute(item.Name, item.Address, item.Seats));
                    await InitLieux(newItem.AggregateId);
                });
            }
        }

        public RelayCommandAsync AddTrainerCommand { get; }
        private async Task ExecuteAddTrainerAsync()
        {
            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer un formateur", new EditableTrainer());
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync(async () => {
                    var item = vm.Item as EditableTrainer;
                    var newItem = await Task.Run(() => _applicationService.Command<CreateTrainer>().Execute(item.Lastname, item.Firstname, item.Email));
                    await InitTrainings(newItem.AggregateId);
                });
            }
        }

        public override async Task Init()
        {
            var t1 = InitFormations(_appointmentItem.FormationId);
            var t2 = InitTrainings(_appointmentItem.FormateurId);
            var t3 = InitLieux(_appointmentItem.LieuId);

            StartSession = _appointmentItem.Start;
            Duration = _appointmentItem.Durée;
            _sessionId = _appointmentItem.SessionId;

            await Task.WhenAll(t1, t2, t3);

            Seats = _appointmentItem.Places;
        }

        private async Task InitFormations(Guid? selectedFormationId)
        {
            var formationsTask = await Task.Run(() => _trainingQueries.GetAll().Select(a => new TrainingItem(a)));
            Trainings = new ObservableCollection<TrainingItem>(formationsTask);
            SelectedTraining = Trainings.FirstOrDefault(a => a.Id == selectedFormationId);
        }

        private async Task InitTrainings(Guid? selectedFormateurId)
        {
            var formateursTask = await Task.Run(() => _trainerQueries.GetAll().Select(a => new TrainerItem(a)));
            Trainers = new ObservableCollection<TrainerItem>(formateursTask);
            SelectedTrainer = Trainers.FirstOrDefault(a => a.Id == selectedFormateurId);
        }

        private async Task InitLieux(Guid? selectedLieuId)
        {
            var lieuxTask = await Task.Run(() => _locationQueries.GetAll().Select(a => new Location(a)));
            Locations = new ObservableCollection<Location>(lieuxTask);
            SelectedLocation = Locations.FirstOrDefault(a => a.Id == selectedLieuId);
        }
    }
   
    public class TrainerItem
    {
        private readonly ITrainerResult _result;

        public TrainerItem(ITrainerResult result)
        {
            _result = result;
        }

        public Guid Id => _result.Id;

        public override string ToString()
        {
            return _result.Firstname + " " + _result.Lastname;
        }
    }

    public class TrainingItem
    {
        private readonly ITrainingResult _result;

        public TrainingItem(ITrainingResult result)
        {
            _result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public Guid Id => _result.Id;
        public int Seats => _result.Seats;

        public override string ToString()
        {
            return _result.Name;
        }
    }

    public class Location
    {
        private readonly ILocationResult _result;

        public Location(ILocationResult result)
        {
            _result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public Guid Id => _result.LocationId;
        public int Seats => _result.Seats;

        public override string ToString()
        {
            return _result.Name;
        }
    }

    public class AppointmentItem
    {
        public AppointmentItem(DateTime? start, int durée, Guid? formateurId, Guid? lieuId, Guid? formationId, int places, Guid? sessionId)
        {
            Start = start;
            Durée = durée;
            FormateurId = formateurId;
            LieuId = lieuId;
            FormationId = formationId;
            Places = places;
            SessionId = sessionId;
        }

        public DateTime? Start { get; }
        public int Durée { get; }
        public Guid? FormationId { get; }
        public Guid? FormateurId { get; }
        public Guid? LieuId { get; }
        public int Places { get; }
        public Guid? SessionId { get; }
    }
}
