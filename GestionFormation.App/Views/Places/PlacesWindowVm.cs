using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GestionFormation.App.Core;
using GestionFormation.App.Views.EditableLists;
using GestionFormation.Applications.Companies;
using GestionFormation.Applications.Seats;
using GestionFormation.Applications.Sessions;
using GestionFormation.Applications.Students;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Companies.Queries;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Seats.Queries;
using GestionFormation.CoreDomain.Sessions.Queries;
using GestionFormation.CoreDomain.Students.Queries;

namespace GestionFormation.App.Views.Places
{
    public class PlacesWindowVm : PopupWindowVm
    {
        private readonly Guid _sessionId;
        private readonly int _sessionPlaces;
        private readonly IApplicationService _applicationService;
        private readonly ISessionQueries _sessionQueries;
        private readonly ICompanyQueries _companyQueries;
        private readonly IStudentQueries _studentQueries;
        private readonly ISeatQueries _seatQueries;

        public PlacesWindowVm(Guid sessionId,  int sessionPlaces, ISeatQueries seatQueries, ICompanyQueries companyQueries, IStudentQueries studentQueries, IApplicationService applicationService, ISessionQueries sessionQueries)
        {
            _sessionId = sessionId;
            _sessionPlaces = sessionPlaces;
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _sessionQueries = sessionQueries ?? throw new ArgumentNullException(nameof(sessionQueries));
            _companyQueries = companyQueries ?? throw new ArgumentNullException(nameof(companyQueries));
            _studentQueries = studentQueries ?? throw new ArgumentNullException(nameof(studentQueries));
            _seatQueries = seatQueries ?? throw new ArgumentNullException(nameof(seatQueries));

            AddPlaceCommand = new RelayCommandAsync(()=>ExecuteAddPlaceAsync(false), () => SelectedSociete != null && SelectedStagiaire != null);
            AddValidatedPlaceCommand = new RelayCommandAsync(()=>ExecuteAddPlaceAsync(true), () => SelectedSociete != null && SelectedStagiaire != null);
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
                AddValidatedPlaceCommand.RaiseCanExecuteChanged();
            }
        }

        public Item SelectedSociete
        {
            get => _selectedSocieteId;
            set
            {
                Set(()=> SelectedSociete, ref _selectedSocieteId, value);
                AddPlaceCommand.RaiseCanExecuteChanged();
                AddValidatedPlaceCommand.RaiseCanExecuteChanged();
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
            PlacesAttenteValidation = Places.Count(a=>a.Statut == SeatStatus.ToValidate);
            PlacesValide = Places.Count(a => a.Statut == SeatStatus.Valid);
            PlacesReservees = PlacesValide + PlacesAttenteValidation;
            PlacesDisponibles = PlacesTotal - PlacesReservees;
        }

        public override async Task Init()
        {
            var stagiaires = Task.Run(() => _studentQueries.GetAll().Select(a => new Item {Id = a.Id, Label = a.Firstname + " " + a.Lastname}));
            var societes = Task.Run(()=>_companyQueries.GetAll().Select(a=>new Item { Id = a.CompanyId, Label = a.Name}));
            var session = Task.Run(() => _sessionQueries.GetSession(_sessionId));

            await Task.WhenAll(stagiaires, societes, session);

            Stagiaires = new ObservableCollection<Item>(stagiaires.Result);
            Societes = new ObservableCollection<Item>(societes.Result);
            SessionInfos = new SessionInfos(session.Result);

            await RefreshPlaces();
            RefreshCompteurs();
        }

        public RelayCommandAsync AddPlaceCommand { get; set; }
        public RelayCommandAsync AddValidatedPlaceCommand { get; set; }
        private async Task ExecuteAddPlaceAsync(bool validate)
        {
            if (SelectedStagiaire == null || SelectedSociete == null)
            {
                MessageBox.Show("Veuillez indiquer le stagiaire ET la société pour réserver une place dans cette session.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await HandleMessageBoxError.ExecuteAsync(async () =>
            {
                var seat = await Task.Run(() => _applicationService.Command<ReserveSeat>().Execute(_sessionId, SelectedStagiaire.Id, SelectedSociete.Id,!validate));
                if(validate)
                    await Task.Run(()=>_applicationService.Command<ValidateSeat>().Execute(seat.AggregateId));

                await RefreshPlaces();
                SelectedStagiaire = null;
                SelectedSociete = null;
            });
        }

        private async Task RefreshPlaces()
        {
            var items = await Task.Run(() => _seatQueries.GetAll(_sessionId).Select(a => new PlaceItem(a, Stagiaires.First(b=>b.Id == a.StudentId ).Label, Societes.First(b=>b.Id == a.CompanyId).Label)));
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
                    var newStagiaire = await Task.Run(() => _applicationService.Command<CreateStudent>().Execute(item.Nom, item.Prenom));

                    var stagiaires = await Task.Run(() => _studentQueries.GetAll().Select(a => new Item { Id = a.Id, Label = a.Firstname + " " + a.Lastname }));
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
                    var newSociete = await Task.Run(() => _applicationService.Command<CreateCompany>().Execute(item.Nom, item.Adresse, item.CodePostal, item.Ville));

                    var societes = await Task.Run(() => _companyQueries.GetAll().Select(a => new Item { Id = a.CompanyId, Label = a.Name}));
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
                        await Task.Run(() =>
                        {
                            _applicationService.Command<CancelSeat>().Execute(selectedPlace.PlaceId, vm.Raison);
                            
                        });                        
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
                    await Task.Run(() =>
                    {
                        _applicationService.Command<ValidateSeat>().Execute(selectedPlace.PlaceId);
                    });                    
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
                        await Task.Run(() =>
                        {
                            _applicationService.Command<RefuseSeat>().Execute(selectedPlace.PlaceId, vm.Raison);
                        });                        
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
            await _applicationService.OpenPopup<CreateConventionWindowVm>(_sessionInfos, SelectedPlaces.ToList());
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
        public PlaceItem(ISeatResult result, string stagiaireNom, string societeNom)
        {
            StagiaireNom = stagiaireNom;
            SocieteNom = societeNom;
            PlaceId = result.SeatId;
            StagiaireId = result.StudentId;
            SocieteId = result.CompanyId;
            Statut = result.Status;
            Raison = result.Reason;
            ConventionId = result.AgreementId;
            Convention = result.Agreementnumber;
            AgreementType = result.AgreementType;

            EtatConvention = new EtatConvention(result);
            EtatPlace = new EtatPlace(result.Status);            
        }

        public Guid PlaceId { get; }

        public Guid StagiaireId { get; }
        public Guid SocieteId { get;  }

        public string StagiaireNom { get; }
        public string SocieteNom { get; }

        public SeatStatus Statut { get; set; }
        public string Raison { get; }

        public EtatPlace EtatPlace { get; }
        
        public string Convention { get; }
        public EtatConvention EtatConvention { get; }
        public Guid? ConventionId { get; }
        public AgreementType AgreementType { get; }               
    }

    public class EtatPlace
    {
        private SeatStatus Statut;
        public EtatPlace(SeatStatus status)
        {
            Statut = status;
        }

        public string Label
        {
            get
            {
                switch (Statut)
                {
                    case SeatStatus.ToValidate: return "Attente de validation";
                    case SeatStatus.Canceled: return "Annulée";
                    case SeatStatus.Refused: return "Refusée";
                    case SeatStatus.Valid: return "Validée";
                    default: throw new Exception($"Le statut {Statut} est introuvable");
                }
            }
        }

        public string Icon
        {
            get
            {
                var iconPath = "/Images/Etats/Places/";
                switch (Statut)
                {
                    case SeatStatus.ToValidate: return iconPath + "pending_validation.png";
                    case SeatStatus.Canceled: return iconPath + "canceled.png";
                    case SeatStatus.Refused: return iconPath + "refused.png";
                    case SeatStatus.Valid: return iconPath + "validated.png";
                    default: throw new Exception($"Le statut {Statut} est introuvable");
                }
            }
        }
    }

    public class EtatConvention
    {
        public EtatConvention(ISeatResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.AgreementId.HasValue)
            
                Label = (result.AgreementSigned ? "Signée" : "Attente de signature");
            else
                Label = result.AgreementRevoked ? "Révoquée" : "Non générée";
        }

        public string Label { get; }

        public string Icon
        {
            get
            {
                var iconPath = "/Images/Etats/Conventions/";
                switch (Label)
                {
                    case "Révoquée": return iconPath + "revoked.png";
                    case "Signée": return iconPath + "signed.png";
                    case "Attente de signature": return iconPath + "signature_pending.png";
                    case "Non générée": return iconPath + "to_create.png";

                    default: throw new Exception($"le statut {Label} est introuvable.");
                }
            }
        }
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
            FormationName = result.Training;
            FormateurName = result.Trainer.ToString();
            FormationLieu = result.Location;
            FormationDuree = $"Le {result.SessionStart:d} sur {result.Duration} jour(s)";
        }
        public string FormationName { get; }
        public string FormationDuree { get; }
        public string FormateurName { get; }
        public string FormationLieu { get; }
    }
}
