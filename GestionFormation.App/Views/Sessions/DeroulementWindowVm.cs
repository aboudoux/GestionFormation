using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GestionFormation.App.Core;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Seats.Queries;
using GestionFormation.CoreDomain.Sessions.Queries;

namespace GestionFormation.App.Views.Sessions
{
    public class DeroulementWindowVm : PopupWindowVm
    {
        private readonly ISeatQueries _seatQueries;
        private readonly Guid _sessionId;
        private readonly IDocumentRepository _documentRepository;
        private readonly ISessionQueries _sessionQueries;
        private ObservableCollection<ISeatValidatedResult> _places;
        private ObservableCollection<ISeatValidatedResult> _selectedPlaces;
        private ICompleteSessionResult _sessionInfos;

        public DeroulementWindowVm(ISeatQueries seatQueries, Guid sessionId, IDocumentRepository documentRepository, ISessionQueries sessionQueries)
        {
            _seatQueries = seatQueries ?? throw new ArgumentNullException(nameof(seatQueries));
            _sessionId = sessionId;
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
            _sessionQueries = sessionQueries ?? throw new ArgumentNullException(nameof(sessionQueries));
            RefreshCommand = new RelayCommandAsync(ExecuteRefreshAsync);

            SelectedPlaces = new ObservableCollection<ISeatValidatedResult>();
            SelectedPlaces.CollectionChanged += (sender, args) =>
            {
                PrintCertificatAssiduiteCommand.RaiseCanExecuteChanged();
                PrintQuestionnaireCommand.RaiseCanExecuteChanged();
                PrintDiplomeCommand.RaiseCanExecuteChanged();
            };
            
            PrintFeuillePresenceCommand = new RelayCommand(ExecutePrintFeuillePresence);
            PrintCertificatAssiduiteCommand = new RelayCommand(ExecutePrintCertificatAssiduite, () => SelectedPlaces.Any());
            PrintQuestionnaireCommand = new RelayCommand(ExecutePrintQuestionnaire, () => SelectedPlaces.Any());
            PrintDiplomeCommand = new RelayCommand(ExecutePrintDiplome, () => SelectedPlaces.Any());
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

        public RelayCommand PrintFeuillePresenceCommand { get; }
        private void ExecutePrintFeuillePresence()
        {
            HandleMessageBoxError.Execute(()=>{ 
                var document = _documentRepository.CreateFeuillePresence(_sessionInfos.Training, _sessionInfos.SessionStart, _sessionInfos.Duration, _sessionInfos.Location, _sessionInfos.Trainer, Places.Select(a=>new Participant(a.Trainee, a.Company)).ToList());
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
                    var document = _documentRepository.CreateCertificatAssiduite(place.Trainee, place.Company, _sessionInfos.Training, _sessionInfos.Location, _sessionInfos.Duration, _sessionInfos.Trainer, _sessionInfos.SessionStart);
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
                    var document = _documentRepository.CreateQuestionnaire(_sessionInfos.Trainer, _sessionInfos.Training);
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
                    var document = _documentRepository.CreateDiplome(place.Trainee, place.Company, _sessionInfos.SessionStart, _sessionInfos.SessionStart.AddDays(_sessionInfos.Duration - 1), _sessionInfos.Trainer);
                    Process.Start(document);
                });
            }
        }
    }
}