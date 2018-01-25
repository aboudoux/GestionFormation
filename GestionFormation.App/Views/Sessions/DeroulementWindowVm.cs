using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GestionFormation.App.Core;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Places.Queries;
using GestionFormation.CoreDomain.Sessions.Queries;

namespace GestionFormation.App.Views.Sessions
{
    public class DeroulementWindowVm : PopupWindowVm
    {
        private readonly IPlacesQueries _placesQueries;
        private readonly Guid _sessionId;
        private readonly IDocumentRepository _documentRepository;
        private readonly ISessionQueries _sessionQueries;
        private ObservableCollection<IPlaceValidatedResult> _places;
        private ObservableCollection<IPlaceValidatedResult> _selectedPlaces;
        private ICompleteSessionResult _sessionInfos;

        public DeroulementWindowVm(IPlacesQueries placesQueries, Guid sessionId, IDocumentRepository documentRepository, ISessionQueries sessionQueries)
        {
            _placesQueries = placesQueries ?? throw new ArgumentNullException(nameof(placesQueries));
            _sessionId = sessionId;
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
            _sessionQueries = sessionQueries ?? throw new ArgumentNullException(nameof(sessionQueries));
            RefreshCommand = new RelayCommandAsync(ExecuteRefreshAsync);

            SelectedPlaces = new ObservableCollection<IPlaceValidatedResult>();
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

        public ObservableCollection<IPlaceValidatedResult> Places
        {
            get => _places;
            set { Set(()=>Places, ref _places, value); }
        }

        public ObservableCollection<IPlaceValidatedResult> SelectedPlaces
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
            var t1 = Task.Run(() => _placesQueries.GetValidatedPlaces(_sessionId));
            var t2 = Task.Run(() => _sessionQueries.GetSession(_sessionId));

            await Task.WhenAll(t1, t2);

            Places = new ObservableCollection<IPlaceValidatedResult>(t1.Result);
            _sessionInfos = t2.Result;
        }

        public RelayCommandAsync AbsenceCommand { get; }

        public RelayCommand PrintFeuillePresenceCommand { get; }
        private void ExecutePrintFeuillePresence()
        {
            HandleMessageBoxError.Execute(()=>{ 
                var document = _documentRepository.CreateFeuillePresence(_sessionInfos.Formation, _sessionInfos.DateDebut, _sessionInfos.Durée, _sessionInfos.Lieu, _sessionInfos.Formateur, Places.Select(a=>new Participant(a.Stagiaire, a.Societe)).ToList());
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
                    var document = _documentRepository.CreateCertificatAssiduite(place.Stagiaire, place.Societe, _sessionInfos.Formation, _sessionInfos.Lieu, _sessionInfos.Durée, _sessionInfos.Formateur, _sessionInfos.DateDebut);
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
                    var document = _documentRepository.CreateQuestionnaire(_sessionInfos.Formateur, _sessionInfos.Formation);
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
                    var document = _documentRepository.CreateDiplome(place.Stagiaire, place.Societe, _sessionInfos.DateDebut, _sessionInfos.DateDebut.AddDays(_sessionInfos.Durée - 1), _sessionInfos.Formateur);
                    Process.Start(document);
                });
            }
        }
    }
}