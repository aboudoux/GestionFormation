using System;
using System.Reflection;
using DevExpress.Xpf.Docking;
using GalaSoft.MvvmLight.Messaging;
using GestionFormation.App.Views;
using GestionFormation.App.Views.Admins.History;
using GestionFormation.App.Views.Admins.Replayers;
using GestionFormation.App.Views.EditableLists;
using GestionFormation.App.Views.EditableLists.Utilisateurs;
using GestionFormation.App.Views.Listers;
using GestionFormation.App.Views.Logins;
using GestionFormation.App.Views.Seats;
using GestionFormation.App.Views.Sessions;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;
using ChangePasswordWindow = GestionFormation.App.Views.EditableLists.Utilisateurs.ChangePasswordWindow;

using CreateItem = GestionFormation.App.Views.EditableLists.CreateItem;
using CreateSessionWindow = GestionFormation.App.Views.Sessions.CreateSessionWindow;
using EditableList = GestionFormation.App.Views.EditableLists.EditableList;
using EventReplayerWindow = GestionFormation.App.Views.Admins.Replayers.EventReplayerWindow;
using HistoryWindow = GestionFormation.App.Views.Admins.History.HistoryWindow;
using LoginWindow = GestionFormation.App.Views.Logins.LoginWindow;
using SessionScheduler = GestionFormation.App.Views.Sessions.SessionScheduler;
using UtilisateurList = GestionFormation.App.Views.EditableLists.Utilisateurs.UtilisateurList;

namespace GestionFormation.App.Core
{
    public static class Bootstrapper
    {
        private static ApplicationService _applicationService;
        private static SqlEventStore _eventStore;
        public static IApplicationService Start(DocumentGroup documentGroup)
        {
            if (documentGroup == null) throw new ArgumentNullException(nameof(documentGroup));

            BootstrapQuery.Launch();

            var pagelocator = PageLocator.With()
                .Element<TrainerListVm, EditableList>()
                .Element<LocationListVm, EditableList>()
                .Element<StudentListVm, EditableList>()
                .Element<CompanyListVm, EditableList>()
                .Element<CreateItemVm, CreateItem>()
                .Element<ContactListVm, EditableList>()
                .Element<SessionSchedulerVm, SessionScheduler>()
                .Element<CreateSessionWindowVm, CreateSessionWindow>()
                .Element<SeatsWindowVm, SeatsWindow>()
                .Element<ReasonWindowVm, ReasonWindow>()                
                .Element<CreateAgreementWindowVm, CreateAgreementWindow>()                
                .Element<ManageAgreementWindowVm, ManageAgreementWindow>()                
                .Element<EventReplayerWindowVm, EventReplayerWindow>()                
                .Element<UtilisateurListVm, UtilisateurList>()
                .Element<ChangePasswordWindowVm, ChangePasswordWindow>()
                .Element<LoginWindowsVm, LoginWindow>()
                .Element<ChangeRoleWindowVm, ChangeRoleWindow>()
                .Element<TrainingFlowWindowVm, TrainingFlowWindow>()
                .Element<HistoryWindowVm, HistoryWindow>()
                .Element<SeatsListerVm, ListerWindow>()
                .Element<CreateContactWindowVm, CreateContactWindow>()
                .Element<CloseSessionWindowVm, CloseSessionWindow>()
                .Element<TrainingListVm, EditableList>()
                .Build();

            var dispatcher = new EventDispatcher();
            dispatcher.AutoRegisterAllEventHandler();

            _eventStore = new SqlEventStore(new DomainEventJsonEventSerializer(), new EventStamping(new LocalApplicationUser()));
            _applicationService = new ApplicationService(pagelocator, documentGroup, new EventBus(dispatcher, _eventStore), new Messenger());
            _applicationService.AutoRegisterSimpleDependencies(Assembly.GetAssembly(typeof(IRuntimeDependency)));

            return _applicationService;
        }

        public static IApplicationService SetLoggedUser(LoggedUser loggedUser)
        {
            if (loggedUser == null) throw new ArgumentNullException(nameof(loggedUser));
            _eventStore.SetEventStamping(new EventStamping(loggedUser));
            _applicationService.LoggedUser = loggedUser;
            return _applicationService;          
        }
    }
}