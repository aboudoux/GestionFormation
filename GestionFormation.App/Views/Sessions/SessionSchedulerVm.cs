using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Scheduling;
using GalaSoft.MvvmLight;
using GestionFormation.App.Core;
using GestionFormation.App.Views.Seats;
using GestionFormation.Applications.Sessions;
using GestionFormation.CoreDomain.Sessions.Queries;
using GestionFormation.CoreDomain.Users;
using ColorHelper = GestionFormation.App.Views.EditableLists.ColorHelper;

namespace GestionFormation.App.Views.Sessions
{
    public class SessionSchedulerVm : ViewModelBase, ILoadableVm
    {
        private readonly ISessionQueries _sessionQueries;
        private readonly IApplicationService _applicationService;
        private ObservableCollection<SessionItem> _sessions;
        private bool _isLoading;
        private ObservableCollection<SessionItem> _selectedSessions;
        public string Title => "Calendrier des formations";

        public SessionSchedulerVm(IApplicationService applicationService, ISessionQueries sessionQueries)
        {
            _sessionQueries = sessionQueries ?? throw new ArgumentNullException(nameof(sessionQueries));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            CreateSession = new RelayCommandAsync<AppointmentItemEventArgs>(ExecuteCreateSessionAsync);
            LoadCommand = new RelayCommandAsync(ExecuteLoadAsync);

            SelectedSessions = new ObservableCollection<SessionItem>();
            DeleteCommand = new RelayCommandAsync(ExecuteDeleteCommandAsync, () => SelectedSessions.Any());
            EditCommand = new RelayCommandAsync(ExecuteEditCommandAsync, () => SelectedSessions.Any());
            OpenPlacesCommand = new RelayCommandAsync(ExecutePlacesAsync, () => SelectedSessions.Any());
            OpenDeroulementCommand = new RelayCommandAsync(ExecuteOpenDeroulementAsync, () => SelectedSessions.Any());
            OpenClotureCommand = new RelayCommandAsync(ExecuteOpenClotureAsync, () => SelectedSessions.Any());
            SelectedSessions.CollectionChanged += (sender, args) =>
            {
                DeleteCommand.RaiseCanExecuteChanged();
                EditCommand.RaiseCanExecuteChanged();
                OpenPlacesCommand.RaiseCanExecuteChanged();
                OpenDeroulementCommand.RaiseCanExecuteChanged();
                OpenClotureCommand.RaiseCanExecuteChanged();
            };
            DropSession = new RelayCommandAsync<SessionDropped>(ExecuteDropSessionAsync);
            Security = new Security(applicationService);            
        }        

        public Security Security { get; }


        public RelayCommandAsync<AppointmentItemEventArgs> CreateSession { get; }
        private async Task ExecuteCreateSessionAsync(AppointmentItemEventArgs arg)
        {
            if (!Security[UserRole.Manager])
            {
                MessageBox.Show("Vous n'avez pas le droit d'ajouter une session.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var vm = await _applicationService.OpenPopup<CreateSessionWindowVm>(new AppointmentItem(arg.Appointment.Start, arg.Appointment.Duration.Days, null, null, null, 0, null));
            if (vm.IsValidated)
                await LoadCommand.ExecuteAsync();
        }

        public ObservableCollection<SessionItem> Sessions
        {
            get => _sessions;
            set { Set(()=>Sessions, ref _sessions, value); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { Set(() => IsLoading, ref _isLoading, value); }
        }

        public RelayCommandAsync LoadCommand { get; }
        private async Task ExecuteLoadAsync()
        {
            var loadedSession = await Task.Run(()=>_sessionQueries.GetAllCompleteSession().Select(a => new SessionItem(a)));
            Sessions = new ObservableCollection<SessionItem>(loadedSession);
        }

        public ObservableCollection<SessionItem> SelectedSessions
        {
            get => _selectedSessions;
            set { Set(()=>SelectedSessions, ref _selectedSessions, value); }
        }

        public RelayCommandAsync<SessionDropped> DropSession { get; }
        private async Task ExecuteDropSessionAsync(SessionDropped sessionItem)
        {
            var sessionToUpdate = SelectedSessions.FirstOrDefault(a => a.Id == sessionItem.SessionId);            
            if(sessionToUpdate == null )
                throw new Exception("Impossible de retrouver l'item sélectionnée");

            try
            {
                await Task.Run(() => _applicationService.Command<UpdateSession>().Execute(sessionItem.SessionId, sessionToUpdate.FormationId, sessionItem.NewStart, sessionItem.NewDurée, sessionToUpdate.Places, sessionToUpdate.LieuId, sessionToUpdate.FormateurId));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            await LoadCommand.ExecuteAsync();
        }

        public RelayCommandAsync DeleteCommand { get; }
        private async Task ExecuteDeleteCommandAsync()
        {
            if (!SelectedSessions.Any())
                return;

            if (MessageBoxResult.Yes == MessageBox.Show("Etes-vous sûr de vouloir supprimer la session sélectionnée ?", "Suppression d'une session", MessageBoxButton.YesNo, MessageBoxImage.Warning))
            {
                await Task.Run(() => _applicationService.Command<RemoveSession>().Execute(SelectedSessions.First().Id));
                await LoadCommand.ExecuteAsync();
            }
        }

        public RelayCommandAsync EditCommand { get; }
        private async Task ExecuteEditCommandAsync()
        {
            var selectedSession = SelectedSessions.First();
            var vm = await _applicationService.OpenPopup<CreateSessionWindowVm>(new AppointmentItem(selectedSession.Start, (selectedSession.End - selectedSession.Start).Days, selectedSession.FormateurId, selectedSession.LieuId, selectedSession.FormationId, selectedSession.Places, selectedSession.Id));
            if (vm.IsValidated)
                await LoadCommand.ExecuteAsync();
        }

        public RelayCommandAsync OpenPlacesCommand { get; }
        private async Task ExecutePlacesAsync()
        {
            var selectedSession = SelectedSessions.First();
            await _applicationService.OpenPopup<SeatsWindowVm>(selectedSession.Id, selectedSession.Places);
        }       
        
        public RelayCommandAsync OpenDeroulementCommand { get; }
        private async Task ExecuteOpenDeroulementAsync()
        {
            var selectedSession = SelectedSessions.First();
            await _applicationService.OpenPopup<TrainingFlowWindowVm>(selectedSession.Id);
        }

        public RelayCommandAsync OpenClotureCommand { get; }
        private async Task ExecuteOpenClotureAsync()
        {
            var selectedSession = SelectedSessions.First();
            await _applicationService.OpenPopup<CloseSessionWindowVm>(selectedSession.Id);
        }
    }

    public class SessionDropped
    {
        public Guid SessionId { get; }
        public DateTime NewStart { get; }
        public int NewDurée { get; }

        public SessionDropped(Guid sessionId, DateTime newStart, int newDurée)
        {
            SessionId = sessionId;
            NewStart = newStart;
            NewDurée = newDurée;
        }
    }

    public class SessionItem
    {
        public SessionItem(ICompleteSessionResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            Id = result.SessionId;
            Start = result.SessionStart;
            End = result.SessionStart.AddDays(result.Duration);
            Description = $"Formation assurée par {result.Trainer}\r\n" +
                          $"Durée de la formation : {result.Duration} jour(s)\r\n" +
                          $"{result.Seats} places dont :\r\n" +
                          $"- {result.ReservedSeats} réservée(s)\r\n" +
                          $"- {result.Seats - result.ReservedSeats} disponible(s)";
            Sujet = $"Formation {result.Training} - {result.Location}";

            Places = result.Seats;
            FormateurId = result.TrainerId;
            LieuId = result.LocationId;
            FormationId = result.TrainingId;

            Color = ColorHelper.FromInt(result.Color);
        }

        public Guid Id { get; }
        public DateTime Start { get; }
        public DateTime End { get; }
        public string Description { get; }
        public string Sujet { get; }
        public int Places { get; }
        public Guid? FormateurId { get; }
        public Guid? LieuId { get; }    
        public Guid FormationId { get; }
        public System.Windows.Media.Color Color { get; }
    }
}