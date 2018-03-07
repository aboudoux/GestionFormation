using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GestionFormation.App.Core;
using GestionFormation.Applications.Seats;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Seats.Queries;
using GestionFormation.CoreDomain.Sessions.Queries;
using MessageBox = System.Windows.MessageBox;

namespace GestionFormation.App.Views.Sessions
{
    public class TrainingFlowWindowVm : PopupWindowVm
    {
        private readonly ISeatQueries _seatQueries;
        private readonly Guid _sessionId;
        private readonly IDocumentCreator _documentCreator;
        private readonly ISessionQueries _sessionQueries;
        private readonly IApplicationService _applicationService;
        private readonly IComputerService _computerService;
        private ObservableCollection<ISeatValidatedResult> _seats;
        private ObservableCollection<ISeatValidatedResult> _selectedSeats;
        private ICompleteSessionResult _sessionInfos;

        public TrainingFlowWindowVm(ISeatQueries seatQueries, Guid sessionId, IDocumentCreator documentCreator, ISessionQueries sessionQueries, IApplicationService applicationService, IComputerService computerService)
        {
            _seatQueries = seatQueries ?? throw new ArgumentNullException(nameof(seatQueries));
            _sessionId = sessionId;
            _documentCreator = documentCreator ?? throw new ArgumentNullException(nameof(documentCreator));
            _sessionQueries = sessionQueries ?? throw new ArgumentNullException(nameof(sessionQueries));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _computerService = computerService ?? throw new ArgumentNullException(nameof(computerService));
            RefreshCommand = new RelayCommandAsync(ExecuteRefreshAsync);

            SelectedSeats = new ObservableCollection<ISeatValidatedResult>();
            SelectedSeats.CollectionChanged += (sender, args) =>
            {
                PrintCertificatOfAttendanceCommand.RaiseCanExecuteChanged();
                PrintSurveyCommand.RaiseCanExecuteChanged();
                PrintDegreeCommand.RaiseCanExecuteChanged();
                MissingCommand.RaiseCanExecuteChanged();
            };
            
            PrintTimesheetCommand = new RelayCommand(ExecutePrintFeuillePresence);
            PrintCertificatOfAttendanceCommand = new RelayCommand(ExecutePrintCertificatAssiduite, () => SelectedSeats.Any());
            PrintSurveyCommand = new RelayCommand(ExecutePrintQuestionnaire, () => SelectedSeats.Any());
            PrintDegreeCommand = new RelayCommand(ExecutePrintDiplome, () => SelectedSeats.Any());
            MissingCommand = new RelayCommandAsync(ExecuteAbsenceAsync, () => SelectedSeats.Any());
            PrintAllDocumentCommand = new RelayCommandAsync(ExecutePrintAllDocumentAsync);
        }        

        public ObservableCollection<ISeatValidatedResult> Seats
        {
            get => _seats;
            set { Set(()=>Seats, ref _seats, value); }
        }

        public ObservableCollection<ISeatValidatedResult> SelectedSeats
        {
            get => _selectedSeats;
            set { Set(()=>SelectedSeats, ref _selectedSeats, value); }
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

            Seats = new ObservableCollection<ISeatValidatedResult>(t1.Result);
            _sessionInfos = t2.Result;
        }

        public RelayCommandAsync MissingCommand { get; }
        private async Task ExecuteAbsenceAsync()
        {
            if( MessageBoxResult.Yes != MessageBox.Show(
                "Attention : vous êtes sur le point de signaler une absence pour les stagiaires sélectionnés.\r\nVoulez vous continuer ?",
                "Signaler une absence", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) )
                return;

            foreach (var place in _selectedSeats)
            {
                await HandleMessageBoxError.ExecuteAsync(async () =>
                {
                    await Task.Run(() => _applicationService.Command<ReportMissingStudent>().Execute(place.SeatId));
                });
            }

            await RefreshCommand.ExecuteAsync();
        }

        public RelayCommand PrintTimesheetCommand { get; }
        private void ExecutePrintFeuillePresence()
        {
            PrintTimesheet(document => Process.Start(document));            
        }

        public RelayCommand PrintCertificatOfAttendanceCommand { get; }
        private void ExecutePrintCertificatAssiduite()
        {
            PrintCertificatOfAttendance(document => Process.Start(document));            
        }

        public RelayCommand PrintSurveyCommand { get; }
        private void ExecutePrintQuestionnaire()
        {
            PrintSurvey(document => Process.Start(document));
        }

        public RelayCommand PrintDegreeCommand { get; }
        private void ExecutePrintDiplome()
        {
            PrintDegree(document => Process.Start(document));
        }

        public RelayCommandAsync PrintAllDocumentCommand { get; }
        private Task ExecutePrintAllDocumentAsync()
        {
            PrintTimesheet(_computerService.Print);
            PrintCertificatOfAttendance(_computerService.Print);
            PrintSurvey(_computerService.Print);
            PrintDegree(_computerService.Print);

            return Task.CompletedTask;
        }        

        private void PrintTimesheet(Action<string> printAction)
        {
            HandleMessageBoxError.Execute(() => {
                var document = _documentCreator.CreateTimesheet(_sessionInfos.Training, _sessionInfos.SessionStart, _sessionInfos.Duration, _sessionInfos.Location, _sessionInfos.Trainer, Seats.Select(a => new Attendee(a.Student, a.Company)).ToList());
                printAction(document);
            });            
        }
        private void PrintCertificatOfAttendance(Action<string> printAction)
        {
            ForeachSeat(seat => _documentCreator.CreateCertificateOfAttendance(seat.Student, seat.Company, _sessionInfos.Training, _sessionInfos.Location, _sessionInfos.Duration, _sessionInfos.Trainer, _sessionInfos.SessionStart), printAction);
        }
        private void PrintSurvey(Action<string> printAction)
        {
            ForeachSeat(seat => _documentCreator.CreateSurvey(_sessionInfos.Trainer, _sessionInfos.Training), printAction);
        }
        private void PrintDegree(Action<string> printAction)
        {
            ForeachSeat(seat => _documentCreator.CreateDegree(seat.Student, seat.Company, _sessionInfos.SessionStart, _sessionInfos.SessionStart.AddDays(_sessionInfos.Duration - 1), _sessionInfos.Trainer), printAction);
        }

        private void ForeachSeat(Func<ISeatValidatedResult, string> createDocument, Action<string> action)
        {
            foreach (var seat in Seats)
            {
                HandleMessageBoxError.Execute(() =>
                {
                    var document = createDocument(seat);
                    action(document);
                });
            }
        }
    }
}