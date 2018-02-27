using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GestionFormation.App.Core;
using GestionFormation.App.Views;
using GestionFormation.App.Views.Admins.History;
using GestionFormation.App.Views.Admins.Replayers;
using GestionFormation.App.Views.EditableLists;
using GestionFormation.App.Views.EditableLists.Utilisateurs;
using GestionFormation.App.Views.Listers;
using GestionFormation.App.Views.Logins;
using GestionFormation.App.Views.Seats;
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
        private ObservableCollection<NotificationItem> _notifications;
        private NotificationItem _selectedNotification;

        public MainWindowsVm(IApplicationService applicationService, INotificationQueries notificationQueries)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _notificationQueries = notificationQueries;

            OpenTrainingList = new RelayCommandAsync(async ()=> await OpenDocument<TrainingListVm>());
            OpenScheduler = new RelayCommandAsync(async () => await OpenDocument<SessionSchedulerVm>());
            OpenTrainerList = new RelayCommandAsync(async () => await OpenDocument<TrainerListVm>());
            OpenLocationList = new RelayCommandAsync(async () => await OpenDocument<LocationListVm>());
            OpenStudentList = new RelayCommandAsync(async () => await OpenDocument<StudentListVm>());
            OpenCompanyList = new RelayCommandAsync(async () => await OpenDocument<CompanyListVm>());
            OpenContactList = new RelayCommandAsync(async () => await OpenDocument<ContactListVm>());
            OpenUserList = new RelayCommandAsync(async () => await OpenDocument<UtilisateurListVm>());
            OpenEventReplayer = new RelayCommandAsync(async ()=> await _applicationService.OpenPopup<EventReplayerWindowVm>());
            OpenLoginWindow = new RelayCommandAsync(ExecuteOpenLoginAsync);
            RefreshNotifications = new RelayCommandAsync(ExecuteRefreshNotificationsAsync);
            OpenNotificationCommand = new RelayCommandAsync(ExecuteOpenNotificationAsync);
            OpenHistory = new RelayCommandAsync(async()=>await OpenDocument<HistoryWindowVm>());
            OpenSeatList = new RelayCommandAsync(async ()=> await OpenDocument<SeatsListerVm>());
            DeleteSelectedNotification = new RelayCommandAsync(ExecuteDeleteSelectedNotificationAsync, ()=> SelectedNotification != null);
            
            Title = "Gestion formation - non connecté";
            Security = new Security(applicationService);
        }        

        public Security Security { get; }

        public string Title
        {
            get => _title;
            set { Set(()=>Title, ref _title, value); }
        }

        public RelayCommandAsync OpenSeatList { get; }
        public RelayCommandAsync OpenTrainingList { get; }
        public RelayCommandAsync OpenScheduler { get; }
        public RelayCommandAsync OpenTrainerList { get; }
        public RelayCommandAsync OpenLocationList { get; }
        public RelayCommandAsync OpenStudentList { get; }
        public RelayCommandAsync OpenCompanyList { get; }
        public RelayCommandAsync OpenContactList { get; }
        public RelayCommandAsync OpenUserList { get; }
        public RelayCommandAsync OpenEventReplayer { get; }
        public RelayCommandAsync OpenHistory { get; }
        public RelayCommandAsync OpenLoginWindow { get; }
        private async Task ExecuteOpenLoginAsync()
        {
            var vm = await _applicationService.OpenPopup<LoginWindowsVm>();
            if(!vm.IsValidated)
                Environment.Exit(1);

            Title = "Gestion formation - " + _applicationService.LoggedUser;
            RaisePropertyChanged(()=>Security);

            var t1 = RefreshNotifications.ExecuteAsync();
            var t2 = OpenScheduler.ExecuteAsync();

            await Task.WhenAll(t1, t2);
        }

        public RelayCommandAsync RefreshNotifications { get; }
        private async Task ExecuteRefreshNotificationsAsync()
        {
            var items = await Task.Run(()=>_notificationQueries.GetAll(_applicationService.LoggedUser.Role).Select(a=>new NotificationItem(a)));
            Notifications = new ObservableCollection<NotificationItem>(items);
        }

        public ObservableCollection<NotificationItem> Notifications
        {
            get => _notifications;
            set { Set(()=>Notifications, ref _notifications, value); }
        }

        public NotificationItem SelectedNotification
        {
            get => _selectedNotification;
            set
            {
                Set(()=>SelectedNotification, ref _selectedNotification, value);
                DeleteSelectedNotification.RaiseCanExecuteChanged();
            }
        }

        public RelayCommandAsync OpenNotificationCommand { get; }
        private async Task ExecuteOpenNotificationAsync()
        {
            if(SelectedNotification.AgreementId.HasValue)
                await _applicationService.OpenPopup<ManageAgreementWindowVm>(SelectedNotification.AgreementId.Value);
            else
                await _applicationService.OpenPopup<SeatsWindowVm>(SelectedNotification.SessionId, 10);

            await RefreshNotifications.ExecuteAsync();
        }

        public RelayCommandAsync DeleteSelectedNotification { get; }
        private async Task ExecuteDeleteSelectedNotificationAsync()
        {
            if( MessageBoxResult.No == MessageBox.Show("Vous êtes sur le point de retirer cette notification.\r\nEtes vous sûr de vouloir continuer ?", "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) )
                return;

            await Task.Run(()=>_applicationService.Command<RemoveNotification>().Execute(SelectedNotification.SessionId, SelectedNotification.NotificationId));
            await RefreshNotifications.ExecuteAsync();
        }

        private async Task OpenDocument<TVm>() 
            where TVm : ViewModelBase, ILoadableVm            
        {
            var vm = _applicationService.OpenDocument<TVm>();
            await vm.LoadCommand.ExecuteAsync();
        }
    }

    public class NotificationItem
    {
        private readonly string _label;

        public NotificationItem(INotificationResult result)
        {
            _label = result.Label;
            SessionId = result.SessionId;
            AgreementId = result.AgreementId;
            NotificationId = result.AggregateId;
        }

        public Guid SessionId { get; }
        public Guid? AgreementId { get; }
        public Guid NotificationId { get; }

        public override string ToString()
        {
            return _label;
        }
    }
}