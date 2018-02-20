using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Command;
using GestionFormation.App.Core;
using GestionFormation.Applications.Seats;
using GestionFormation.Applications.Students;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Seats.Queries;
using GestionFormation.CoreDomain.Sessions.Queries;
using MessageBox = System.Windows.MessageBox;

namespace GestionFormation.App.Views.Sessions
{
    public class DeroulementWindowVm : PopupWindowVm
    {
        private readonly ISeatQueries _seatQueries;
        private readonly Guid _sessionId;
        private readonly IDocumentCreator _documentCreator;
        private readonly ISessionQueries _sessionQueries;
        private readonly IApplicationService _applicationService;
        private ObservableCollection<ISeatValidatedResult> _places;
        private ObservableCollection<ISeatValidatedResult> _selectedPlaces;
        private ICompleteSessionResult _sessionInfos;

        public DeroulementWindowVm(ISeatQueries seatQueries, Guid sessionId, IDocumentCreator documentCreator, ISessionQueries sessionQueries, IApplicationService applicationService)
        {
            _seatQueries = seatQueries ?? throw new ArgumentNullException(nameof(seatQueries));
            _sessionId = sessionId;
            _documentCreator = documentCreator ?? throw new ArgumentNullException(nameof(documentCreator));
            _sessionQueries = sessionQueries ?? throw new ArgumentNullException(nameof(sessionQueries));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            RefreshCommand = new RelayCommandAsync(ExecuteRefreshAsync);

            SelectedPlaces = new ObservableCollection<ISeatValidatedResult>();
            SelectedPlaces.CollectionChanged += (sender, args) =>
            {
                PrintCertificatAssiduiteCommand.RaiseCanExecuteChanged();
                PrintQuestionnaireCommand.RaiseCanExecuteChanged();
                PrintDiplomeCommand.RaiseCanExecuteChanged();
                AbsenceCommand.RaiseCanExecuteChanged();
            };
            
            PrintFeuillePresenceCommand = new RelayCommand(ExecutePrintFeuillePresence);
            PrintCertificatAssiduiteCommand = new RelayCommand(ExecutePrintCertificatAssiduite, () => SelectedPlaces.Any());
            PrintQuestionnaireCommand = new RelayCommand(ExecutePrintQuestionnaire, () => SelectedPlaces.Any());
            PrintDiplomeCommand = new RelayCommand(ExecutePrintDiplome, () => SelectedPlaces.Any());
            AbsenceCommand = new RelayCommandAsync(ExecuteAbsenceAsync, () => SelectedPlaces.Any());
        }        

        public ObservableCollection<ISeatValidatedResult> Places
        {
            get => _places;
            set { Set(()=>Places, ref _places, value); }
        }

        public ObservableCollection<ISeatValidatedResult> SelectedPlaces
        {
            get => _selectedPlaces;
            set { Set(()=>SelectedPlaces, ref _selectedPlaces, value); }
        }        

        public override async Task Init()
        {
            await RefreshCommand.ExecuteAsync();
        }

        public override string Title => "Déroulement de la formation";

        public RelayCommandAsync RefreshCommand { get; }
        private async Task ExecuteRefreshAsync()
        {
            var t1 = Task.Run(() => _seatQueries.GetValidatedSeats(_sessionId));
            var t2 = Task.Run(() => _sessionQueries.GetSession(_sessionId));

            await Task.WhenAll(t1, t2);

            Places = new ObservableCollection<ISeatValidatedResult>(t1.Result);
            _sessionInfos = t2.Result;
        }

        public RelayCommandAsync AbsenceCommand { get; }
        private async Task ExecuteAbsenceAsync()
        {
            if( MessageBoxResult.Yes != MessageBox.Show(
                "Attention : vous êtes sur le point de signaler une absence pour les stagiaires sélectionnés.\r\nVoulez vous continuer ?",
                "Signaler une absence", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) )
                return;

            foreach (var place in _selectedPlaces)
            {
                await HandleMessageBoxError.ExecuteAsync(async () =>
                {
                    await Task.Run(() => _applicationService.Command<ReportMissingStudent>().Execute(place.SeatId));
                });
            }

            await RefreshCommand.ExecuteAsync();
        }

        public RelayCommand PrintFeuillePresenceCommand { get; }
        private void ExecutePrintFeuillePresence()
        {
            HandleMessageBoxError.Execute(()=>{ 
                var document = _documentCreator.CreateTimesheet(_sessionInfos.Training, _sessionInfos.SessionStart, _sessionInfos.Duration, _sessionInfos.Location, _sessionInfos.Trainer, Places.Select(a=>new Participant(a.Student, a.Company)).ToList());
                Process.Start(document);
            });
        }

        public RelayCommand PrintCertificatAssiduiteCommand { get; }
        private void ExecutePrintCertificatAssiduite()
        {
            foreach (var place in _selectedPlaces)
            {
                HandleMessageBoxError.Execute(() =>
                {
                    var document = _documentCreator.CreateCertificateOfAttendance(place.Student, place.Company, _sessionInfos.Training, _sessionInfos.Location, _sessionInfos.Duration, _sessionInfos.Trainer, _sessionInfos.SessionStart);
                    Process.Start(document);
                });
            }
        }

        public RelayCommand PrintQuestionnaireCommand { get; }
        private void ExecutePrintQuestionnaire()
        {
            foreach (var place in _selectedPlaces)
            {
                HandleMessageBoxError.Execute(() =>
                {
                    var document = _documentCreator.CreateSurvey(_sessionInfos.Trainer, _sessionInfos.Training);
                    Process.Start(document);
                });
            }
        }

        public RelayCommand PrintDiplomeCommand { get; }
        private void ExecutePrintDiplome()
        {
            foreach (var place in _selectedPlaces)
            {
                HandleMessageBoxError.Execute(() =>
                {
                    var document = _documentCreator.CreateDegree(place.Student, place.Company, _sessionInfos.SessionStart, _sessionInfos.SessionStart.AddDays(_sessionInfos.Duration - 1), _sessionInfos.Trainer);
                    Process.Start(document);
                });
            }
        }
    }
}