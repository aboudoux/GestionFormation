using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GestionFormation.App.Core;
using GestionFormation.App.Views.EditableLists;
using GestionFormation.Applications.Places;
using GestionFormation.Applications.Sessions;
using GestionFormation.Applications.Societes;
using GestionFormation.Applications.Stagiaires;
using GestionFormation.CoreDomain.Conventions;
using GestionFormation.CoreDomain.Places;
using GestionFormation.CoreDomain.Places.Queries;
using GestionFormation.CoreDomain.Sessions.Queries;
using GestionFormation.CoreDomain.Societes.Queries;
using GestionFormation.CoreDomain.Stagiaires.Queries;

namespace GestionFormation.App.Views.Places
{
    public class PlacesWindowVm : PopupWindowVm
    {
        private readonly Guid _sessionId;
        private readonly int _sessionPlaces;
        private readonly IApplicationService _applicationService;
        private readonly ISessionQueries _sessionQueries;
        private readonly ISocieteQueries _societeQueries;
        private readonly IStagiaireQueries _stagiaireQueries;
        private readonly IPlacesQueries _placesQueries;

        public PlacesWindowVm(Guid sessionId,  int sessionPlaces, IPlacesQueries placesQueries, ISocieteQueries societeQueries, IStagiaireQueries stagiaireQueries, IApplicationService applicationService, ISessionQueries sessionQueries)
        {
            _sessionId = sessionId;
            _sessionPlaces = sessionPlaces;
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _sessionQueries = sessionQueries ?? throw new ArgumentNullException(nameof(sessionQueries));
            _societeQueries = societeQueries ?? throw new ArgumentNullException(nameof(societeQueries));
            _stagiaireQueries = stagiaireQueries ?? throw new ArgumentNullException(nameof(stagiaireQueries));
            _placesQueries = placesQueries ?? throw new ArgumentNullException(nameof(placesQueries));

            AddPlaceCommand = new RelayCommandAsync(ExecuteAddPlaceAsync, () => SelectedSociete != null && SelectedStagiaire != null);
            CreateStagiaireCommand = new RelayCommandAsync(ExecuteCreateStagiaireAsync);
            CreateSocieteCommand = new RelayCommandAsync(ExecuteCreateSocieteAsync);

            SelectedPlaces = new ObservableCollection<PlaceItem>();
            SelectedPlaces.CollectionChanged += (sender, args) =>
            {
                AnnulerPlaceCommand.RaiseCanExecuteChanged();
                ValiderPlaceCommand.RaiseCanExecuteChanged();
                RefuserPlaceCommand.RaiseCanExecuteChanged();
            };

            AnnulerPlaceCommand = new RelayCommandAsync(ExecuteAnnulerPlaceAsync, () => SelectedPlaces.Any());
            ValiderPlaceCommand = new RelayCommandAsync(ExecuteValiderPlaceAsync, () => SelectedPlaces.Any());
            RefuserPlaceCommand= new RelayCommandAsync(ExecuteRefuserPlaceAsync, () => SelectedPlaces.Any());
            RefreshPlaceCommand = new RelayCommandAsync(ExecuteRefreshPlaceAsync);

            GenererConventionCommand = new RelayCommandAsync(ExecuteGenererConventionAsync);
            OpenConventionCommand = new RelayCommandAsync(ExecuteOpenConventionAsync);
            Security = new Security(applicationService);
        }

        private ObservableCollection<PlaceItem> _places;
        private ObservableCollection<Item> _stagiaires;
        private ObservableCollection<Item> _societes;
        private Item _selectedStagiaireId;
        private Item _selectedSocieteId;
        private int _placesTotal;
        private int _placesDisponibles;
        private int _placesReservees;
        private int _placesAttenteValidation;
        private int _placesValide;
        private ObservableCollection<PlaceItem> _selectedPlaces;
        private SessionInfos _sessionInfos;
        public string Title => "Gestion des places";

        public Security Security { get; }

        public SessionInfos SessionInfos
        {
            get => _sessionInfos;
            set { Set(()=>SessionInfos, ref _sessionInfos, value); }
        }

        public ObservableCollection<PlaceItem> Places
        {
            get => _places;
            set { Set(() => Places, ref _places, value); }
        }

        public ObservableCollection<Item> Stagiaires
        {
            get => _stagiaires;
            set { Set(() => Stagiaires, ref _stagiaires, value); }
        }

        public ObservableCollection<Item> Societes
        {
            get => _societes;
            set { Set(()=>Societes, ref _societes, value); }
        }

        public Item SelectedStagiaire
        {
            get => _selectedStagiaireId;
            set
            {
                Set(()=> SelectedStagiaire, ref _selectedStagiaireId, value);
                AddPlaceCommand.RaiseCanExecuteChanged();
            }
        }

        public Item SelectedSociete
        {
            get => _selectedSocieteId;
            set
            {
                Set(()=> SelectedSociete, ref _selectedSocieteId, value);
                AddPlaceCommand.RaiseCanExecuteChanged();
            }
        }

        public int PlacesTotal
        {
            get => _placesTotal;
            set { Set(() => PlacesTotal, ref _placesTotal, value); }
        }

        public int PlacesDisponibles
        {
            get => _placesDisponibles;
            set { Set(()=>PlacesDisponibles, ref _placesDisponibles, value); }
        }

        public int PlacesReservees
        {
            get => _placesReservees;
            set { Set(()=> PlacesReservees, ref _placesReservees, value); }
        }

        public int PlacesAttenteValidation
        {
            get => _placesAttenteValidation;
            set { Set(()=>PlacesAttenteValidation, ref _placesAttenteValidation, value); }
        }

        public int PlacesValide
        {
            get => _placesValide;
            set { Set(()=>PlacesValide, ref _placesValide, value); }
        }

        private void RefreshCompteurs()
        {
            PlacesTotal = _sessionPlaces;
            PlacesAttenteValidation = Places.Count(a=>a.Statut == PlaceStatus.AValider);
            PlacesValide = Places.Count(a => a.Statut == PlaceStatus.Validé);
            PlacesReservees = PlacesValide + PlacesAttenteValidation;
            PlacesDisponibles = PlacesTotal - PlacesReservees;
        }

        public override async Task Init()
        {
            var stagiaires = Task.Run(() => _stagiaireQueries.GetAll().Select(a => new Item {Id = a.Id, Label = a.Prenom + " " + a.Nom}));
            var societes = Task.Run(()=>_societeQueries.GetAll().Select(a=>new Item { Id = a.SocieteId, Label = a.Nom}));
            var session = Task.Run(() => _sessionQueries.GetSession(_sessionId));

            await Task.WhenAll(stagiaires, societes, session);

            Stagiaires = new ObservableCollection<Item>(stagiaires.Result);
            Societes = new ObservableCollection<Item>(societes.Result);
            SessionInfos = new SessionInfos(session.Result);

            await RefreshPlaces();
            RefreshCompteurs();
        }

        public RelayCommandAsync AddPlaceCommand { get; set; }
        private async Task ExecuteAddPlaceAsync()
        {
            if (SelectedStagiaire == null || SelectedSociete == null)
            {
                MessageBox.Show("Veuillez indiquer le stagiaire ET la société pour réserver une place dans cette session.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await HandleMessageBoxError.ExecuteAsync(async () =>
            {
                await Task.Run(() => _applicationService.Command<ReservePlace>().Execute(_sessionId, SelectedStagiaire.Id, SelectedSociete.Id));
                await RefreshPlaces();
                SelectedStagiaire = null;
                SelectedSociete = null;
            });
        }

        private async Task RefreshPlaces()
        {
            var items = await Task.Run(() => _placesQueries.GetAll(_sessionId).Select(a => new PlaceItem(a, Stagiaires.First(b=>b.Id == a.StagiaireId ).Label, Societes.First(b=>b.Id == a.SocieteId).Label)));
            Places = new ObservableCollection<PlaceItem>(items);
            RefreshCompteurs();
        }

        public RelayCommandAsync CreateStagiaireCommand { get; }
        private async Task ExecuteCreateStagiaireAsync()
        {
            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer un stagiaire", new EditableStagiaire());
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync(async ()=>{                
                    var item = vm.Item as EditableStagiaire;
                    var newStagiaire = await Task.Run(() => _applicationService.Command<CreateStagiaire>().Execute(item.Nom, item.Prenom));

                    var stagiaires = await Task.Run(() => _stagiaireQueries.GetAll().Select(a => new Item { Id = a.Id, Label = a.Prenom + " " + a.Nom }));
                    Stagiaires = new ObservableCollection<Item>(stagiaires);
                    SelectedStagiaire = Stagiaires.FirstOrDefault(a => a.Id == newStagiaire.AggregateId);                
                });
            }
        }

        public RelayCommandAsync CreateSocieteCommand { get; }
        private async Task ExecuteCreateSocieteAsync()
        {
            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer une société", new EditableSociete());
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync(async () => { 
                    var item = vm.Item as EditableSociete;
                    var newSociete = await Task.Run(() => _applicationService.Command<CreateSociete>().Execute(item.Nom, item.Adresse, item.CodePostal, item.Ville));

                    var societes = await Task.Run(() => _societeQueries.GetAll().Select(a => new Item { Id = a.SocieteId, Label = a.Nom}));
                    Societes = new ObservableCollection<Item>(societes);
                    SelectedSociete = Societes.FirstOrDefault(a => a.Id == newSociete.AggregateId);

                });
            }
        }

        public ObservableCollection<PlaceItem> SelectedPlaces
        {
            get => _selectedPlaces;
            set { Set(()=>SelectedPlaces, ref _selectedPlaces, value); }
        }        

        public RelayCommandAsync AnnulerPlaceCommand { get; set; }
        private async Task ExecuteAnnulerPlaceAsync()
        {
            foreach (var selectedPlace in SelectedPlaces)
            {
                var vm = await _applicationService.OpenPopup<RaisonWindowVm>();
                if (vm.IsValidated)
                {
                    await HandleMessageBoxError.ExecuteAsync(async () => {
                        await Task.Run(() => _applicationService.Command<AnnulerPlace>().Execute(selectedPlace.PlaceId, vm.Raison));                        
                    });
                }
            }
            await RefreshPlaces();
        }
        public RelayCommandAsync ValiderPlaceCommand { get; set; }
        private async Task ExecuteValiderPlaceAsync()
        {
            foreach (var selectedPlace in SelectedPlaces)
            {
                await HandleMessageBoxError.ExecuteAsync(async () =>
                {
                    await Task.Run(() => _applicationService.Command<ValiderPlace>().Execute(selectedPlace.PlaceId));                    
                });
            }
            await RefreshPlaces();
        }
        public RelayCommandAsync RefuserPlaceCommand { get; set; }
        private async Task ExecuteRefuserPlaceAsync()
        {
            foreach (var selectedPlace in SelectedPlaces)
            {
                var vm = await _applicationService.OpenPopup<RaisonWindowVm>();
                if (vm.IsValidated)
                {
                    await HandleMessageBoxError.ExecuteAsync(async () =>
                    {
                        await Task.Run(() => _applicationService.Command<RefuserPlace>().Execute(selectedPlace.PlaceId, vm.Raison));                        
                    });
                }
            }
            await RefreshPlaces();
        }

        public RelayCommandAsync RefreshPlaceCommand { get; }
        private async Task ExecuteRefreshPlaceAsync()
        {
            await RefreshPlaces();
        }

        public RelayCommandAsync GenererConventionCommand { get; }
        private async Task ExecuteGenererConventionAsync()
        {
            await _applicationService.OpenPopup<CreateConventionWindowVm>(SelectedPlaces.ToList());
            await RefreshPlaces();
        }

        public RelayCommandAsync OpenConventionCommand { get; }
        private async Task ExecuteOpenConventionAsync()
        {
            var place = SelectedPlaces.First();
            if (place.ConventionId.HasValue && !string.IsNullOrWhiteSpace(place.Convention))
            {                
                await _applicationService.OpenPopup<GestionConventionWindowVm>(place.ConventionId);
                await RefreshPlaces();
            }
        }      
    }

    public class PlaceItem
    {
        public PlaceItem(IPlaceResult result, string stagiaireNom, string societeNom)
        {
            StagiaireNom = stagiaireNom;
            SocieteNom = societeNom;
            PlaceId = result.PlaceId;
            StagiaireId = result.StagiaireId;
            SocieteId = result.SocieteId;
            Statut = result.Status;
            Raison = result.Raison;
            ConventionId = result.ConventionId;
            Convention = result.NumeroConvention;
            TypeConvention = result.TypeConvention;

            if (result.ConventionId.HasValue)
            {
                if (string.IsNullOrWhiteSpace(result.NumeroConvention))
                    EtatConvention = "Révoquée";
                else                
                    EtatConvention = (result.ConventionSigned ? "Signée" : "Attente de signature");                
            }
            else
                EtatConvention = "Non générée";            
        }

        public Guid PlaceId { get; }

        public Guid StagiaireId { get; }
        public Guid SocieteId { get;  }

        public string StagiaireNom { get; }
        public string SocieteNom { get; }

        public PlaceStatus Statut { get; set; }
        public string Raison { get; }

        public string Etat
        {
            get
            {
                switch (Statut)
                {
                    case PlaceStatus.AValider: return "Attente de validation";
                    case PlaceStatus.Annulé: return "Annulée";
                    case PlaceStatus.Refusé: return "Refusée";
                    case PlaceStatus.Validé: return "Validée";
                        default: throw new Exception($"Le statut {Statut} est introuvable");
                }
            }
        }    
        
        public string Convention { get; }
        public string EtatConvention { get; }
        public Guid? ConventionId { get; }
        public TypeConvention TypeConvention { get; }
    }

    public class Item
    {
        public Guid Id { get; set; }
        public string Label { get; set; }        
    }

    public class SessionInfos
    {
        public ICompleteSessionResult Result { get; }

        public SessionInfos(ICompleteSessionResult result)
        {
            Result = result;
            FormationName = result.Formation;
            FormateurName = result.Formateur.ToString();
            FormationLieu = result.Lieu;
            FormationDuree = $"Le {result.DateDebut:d} sur {result.Durée} jour(s)";
        }
        public string FormationName { get; }
        public string FormationDuree { get; }
        public string FormateurName { get; }
        public string FormationLieu { get; }
    }
}
