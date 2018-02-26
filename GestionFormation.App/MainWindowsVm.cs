using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GestionFormation.App.Core;
using GestionFormation.App.Views;
using GestionFormation.App.Views.EditableLists;
using GestionFormation.App.Views.EditableLists.Formations;
using GestionFormation.App.Views.EditableLists.Utilisateurs;
using GestionFormation.App.Views.Historiques;
using GestionFormation.App.Views.Listers;
using GestionFormation.App.Views.Logins;
using GestionFormation.App.Views.Places;
using GestionFormation.App.Views.Sessions;
using GestionFormation.Applications.Notifications;
using GestionFormation.CoreDomain.Notifications.Queries;

namespace GestionFormation.App
{
    public class MainWindowsVm : ViewModelBase
    {
        private readonly IApplicationService _applicationService;
        private readonly INotificationQueries _notificationQueries;
        private string _title;
        private ObservableCollection<RappelItem> _rappels;
        private RappelItem _selectedRappel;

        public MainWindowsVm(IApplicationService applicationService, INotificationQueries notificationQueries)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _notificationQueries = notificationQueries;

            OpenFormationList = new RelayCommandAsync(async ()=> await OpenDocument<FormationListVm>());
            OpenScheduler = new RelayCommandAsync(async () => await OpenDocument<SessionSchedulerVm>());
            OpenFormateurList = new RelayCommandAsync(async () => await OpenDocument<FormateurListVm>());
            OpenLieuList = new RelayCommandAsync(async () => await OpenDocument<LieuListVm>());
            OpenStagiaireList = new RelayCommandAsync(async () => await OpenDocument<StagiaireListVm>());
            OpenSocieteList = new RelayCommandAsync(async () => await OpenDocument<SocieteListVm>());
            OpenContactList = new RelayCommandAsync(async () => await OpenDocument<ContactListVm>());
            OpenUtilisateurList = new RelayCommandAsync(async () => await OpenDocument<UtilisateurListVm>());
            OpenEventReplayer = new RelayCommandAsync(async ()=> await _applicationService.OpenPopup<EventReplayerWindowVm>());
            OpenLoginWindow = new RelayCommandAsync(ExecuteOpenLoginAsync);
            RefreshRappels = new RelayCommandAsync(ExecuteRefreshRappelAsync);
            OpenRappelCommand = new RelayCommandAsync(ExecuteOpenRappelAsync);
            OpenHistorique = new RelayCommandAsync(async()=>await OpenDocument<HistoriqueWindowVm>());
            OpenPlaceList = new RelayCommandAsync(async ()=> await OpenDocument<PlacesListerVm>());
            DeleteSelectedNotification = new RelayCommandAsync(ExecuteDeleteSelectedNotificationAsync, ()=> SelectedRappel != null);
            
            Title = "Gestion formation - non connecté";
            Security = new Security(applicationService);
        }        

        public Security Security { get; }

        public string Title
        {
            get => _title;
            set { Set(()=>Title, ref _title, value); }
        }

        public RelayCommandAsync OpenPlaceList { get; }
        public RelayCommandAsync OpenFormationList { get; }
        public RelayCommandAsync OpenScheduler { get; }
        public RelayCommandAsync OpenFormateurList { get; }
        public RelayCommandAsync OpenLieuList { get; }
        public RelayCommandAsync OpenStagiaireList { get; }
        public RelayCommandAsync OpenSocieteList { get; }
        public RelayCommandAsync OpenContactList { get; }
        public RelayCommandAsync OpenUtilisateurList { get; }
        public RelayCommandAsync OpenEventReplayer { get; }
        public RelayCommandAsync OpenHistorique { get; }
        public RelayCommandAsync OpenLoginWindow { get; }
        private async Task ExecuteOpenLoginAsync()
        {
            var vm = await _applicationService.OpenPopup<LoginWindowsVm>();
            if(!vm.IsValidated)
                Environment.Exit(1);

            Title = "Gestion formation - " + _applicationService.LoggedUser;
            RaisePropertyChanged(()=>Security);

            var t1 = RefreshRappels.ExecuteAsync();
            var t2 = OpenScheduler.ExecuteAsync();

            await Task.WhenAll(t1, t2);
        }

        public RelayCommandAsync RefreshRappels { get; }
        private async Task ExecuteRefreshRappelAsync()
        {
            var items = await Task.Run(()=>_notificationQueries.GetAll(_applicationService.LoggedUser.Role).Select(a=>new RappelItem(a)));
            Rappels = new ObservableCollection<RappelItem>(items);
        }

        public ObservableCollection<RappelItem> Rappels
        {
            get => _rappels;
            set { Set(()=>Rappels, ref _rappels, value); }
        }

        public RappelItem SelectedRappel
        {
            get => _selectedRappel;
            set
            {
                Set(()=>SelectedRappel, ref _selectedRappel, value);
                DeleteSelectedNotification.RaiseCanExecuteChanged();
            }
        }

        public RelayCommandAsync OpenRappelCommand { get; }
        private async Task ExecuteOpenRappelAsync()
        {
            if(SelectedRappel.ConventionId.HasValue)
                await _applicationService.OpenPopup<GestionConventionWindowVm>(SelectedRappel.ConventionId.Value);
            else
                await _applicationService.OpenPopup<PlacesWindowVm>(SelectedRappel.SessionId, 10);

            await RefreshRappels.ExecuteAsync();
        }

        public RelayCommandAsync DeleteSelectedNotification { get; }
        private async Task ExecuteDeleteSelectedNotificationAsync()
        {
            if( MessageBoxResult.No == MessageBox.Show("Vous êtes sur le point de retirer cette notification.\r\nEtes vous sûr de vouloir continuer ?", "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) )
                return;

            await Task.Run(()=>_applicationService.Command<RemoveNotification>().Execute(SelectedRappel.SessionId, SelectedRappel.NotificationId));
            await RefreshRappels.ExecuteAsync();
        }

        private async Task OpenDocument<TVm>() 
            where TVm : ViewModelBase, ILoadableVm            
        {
            var vm = _applicationService.OpenDocument<TVm>();
            await vm.LoadCommand.ExecuteAsync();
        }
    }

    public class RappelItem
    {
        private readonly string _label;

        public RappelItem(INotificationResult result)
        {
            _label = result.Label;
            SessionId = result.SessionId;
            ConventionId = result.AgreementId;
            NotificationId = result.AggregateId;
        }

        public Guid SessionId { get; }
        public Guid? ConventionId { get; }
        public Guid NotificationId { get; }

        public override string ToString()
        {
            return _label;
        }
    }
}