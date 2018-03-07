using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GestionFormation.App.Core;
using GestionFormation.App.Views.EditableLists;
using GestionFormation.Applications.Companies;
using GestionFormation.Applications.Seats;
using GestionFormation.Applications.Sessions;
using GestionFormation.Applications.Students;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Companies.Queries;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Seats.Queries;
using GestionFormation.CoreDomain.Sessions.Queries;
using GestionFormation.CoreDomain.Students.Queries;

namespace GestionFormation.App.Views.Seats
{
    public class SeatsWindowVm : PopupWindowVm
    {
        private readonly Guid _sessionId;
        private readonly int _sessionPlaces;
        private readonly IApplicationService _applicationService;
        private readonly ISessionQueries _sessionQueries;
        private readonly ICompanyQueries _companyQueries;
        private readonly IStudentQueries _studentQueries;
        private readonly ISeatQueries _seatQueries;

        public SeatsWindowVm(Guid sessionId,  int sessionPlaces, ISeatQueries seatQueries, ICompanyQueries companyQueries, IStudentQueries studentQueries, IApplicationService applicationService, ISessionQueries sessionQueries)
        {
            _sessionId = sessionId;
            _sessionPlaces = sessionPlaces;
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _sessionQueries = sessionQueries ?? throw new ArgumentNullException(nameof(sessionQueries));
            _companyQueries = companyQueries ?? throw new ArgumentNullException(nameof(companyQueries));
            _studentQueries = studentQueries ?? throw new ArgumentNullException(nameof(studentQueries));
            _seatQueries = seatQueries ?? throw new ArgumentNullException(nameof(seatQueries));

            AddSeatCommand = new RelayCommandAsync(()=>ExecuteAddSeatAsync(false), () => SelectedCompany != null);
            AddValidatedSeatCommand = new RelayCommandAsync(()=>ExecuteAddSeatAsync(true), () => SelectedCompany != null && SelectedStudent != null);
            CreateStudentCommand = new RelayCommandAsync(ExecuteCreateStudentAsync);
            CreateCompanyCommand = new RelayCommandAsync(ExecuteCreateSocieteAsync);

            SelectedSeats = new ObservableCollection<SeatItem>();
            SelectedSeats.CollectionChanged += (sender, args) =>
            {
                CancelSeatCommand.RaiseCanExecuteChanged();
                ValidateSeatCommand.RaiseCanExecuteChanged();
                RefuseSeatCommand.RaiseCanExecuteChanged();
            };

            CancelSeatCommand = new RelayCommandAsync(ExecuteCancelSeatAsync, () => SelectedSeats.Any());
            ValidateSeatCommand = new RelayCommandAsync(ExecuteValidateSeatAsync, () => SelectedSeats.Any());
            RefuseSeatCommand= new RelayCommandAsync(ExecuteRefuseSeatAsync, () => SelectedSeats.Any());
            RefreshSeatsCommand = new RelayCommandAsync(ExecuteRefreshSeatsAsync);
            DefineStudentCommand = new RelayCommand(() => { DefineStudent = !DefineStudent; SelectedStudent = null; });
            EditStudentCommand = new RelayCommandAsync(ExecuteEditStudentAsync);

            GenerateAgreementCommand = new RelayCommandAsync(ExecuteGenerateAgreementAsync);
            OpenAgreementCommand = new RelayCommandAsync(ExecuteOpenAgreementAsync);
            Security = new Security(applicationService);
        }       

        private ObservableCollection<SeatItem> _seats;
        private ObservableCollection<Item> _students;
        private ObservableCollection<Item> _companies;
        private Item _selectedStudentId;
        private Item _selectedCompany;
        private int _totalSeats;
        private int _availableSeats;
        private int _bookedSeats;
        private int _seatsToValidate;
        private int _validatedSeats;
        private ObservableCollection<SeatItem> _selectedSeats;
        private SessionInfos _sessionInfos;
        private bool _defineStudent;
        public override string Title => "Gestion des places";

        public Security Security { get; }

        public SessionInfos SessionInfos
        {
            get => _sessionInfos;
            set { Set(()=>SessionInfos, ref _sessionInfos, value); }
        }

        public ObservableCollection<SeatItem> Seats
        {
            get => _seats;
            set { Set(() => Seats, ref _seats, value); }
        }

        public ObservableCollection<Item> Students
        {
            get => _students;
            set { Set(() => Students, ref _students, value); }
        }

        public ObservableCollection<Item> Companies
        {
            get => _companies;
            set { Set(()=>Companies, ref _companies, value); }
        }

        public Item SelectedStudent
        {
            get => _selectedStudentId;
            set
            {
                Set(()=> SelectedStudent, ref _selectedStudentId, value);
                AddSeatCommand.RaiseCanExecuteChanged();
                AddValidatedSeatCommand.RaiseCanExecuteChanged();
            }
        }

        public Item SelectedCompany
        {
            get => _selectedCompany;
            set
            {
                Set(()=> SelectedCompany, ref _selectedCompany, value);
                AddSeatCommand.RaiseCanExecuteChanged();
                AddValidatedSeatCommand.RaiseCanExecuteChanged();
            }
        }

        public int TotalSeats
        {
            get => _totalSeats;
            set { Set(() => TotalSeats, ref _totalSeats, value); }
        }

        public int AvailableSeats
        {
            get => _availableSeats;
            set { Set(()=>AvailableSeats, ref _availableSeats, value); }
        }

        public int BookedSeats
        {
            get => _bookedSeats;
            set { Set(()=> BookedSeats, ref _bookedSeats, value); }
        }

        public int SeatsToValidate
        {
            get => _seatsToValidate;
            set { Set(()=>SeatsToValidate, ref _seatsToValidate, value); }
        }

        public int ValidatedSeats
        {
            get => _validatedSeats;
            set { Set(()=>ValidatedSeats, ref _validatedSeats, value); }
        }

        public bool DefineStudent
        {
            get => _defineStudent;
            set { Set(()=>DefineStudent, ref _defineStudent, value); }
        }

        public RelayCommand DefineStudentCommand { get; }

        private void RefreshCompteurs()
        {
            TotalSeats = _sessionPlaces;
            SeatsToValidate = Seats.Count(a=>a.Statut == SeatStatus.ToValidate);
            ValidatedSeats = Seats.Count(a => a.Statut == SeatStatus.Valid);
            BookedSeats = ValidatedSeats + SeatsToValidate;
            AvailableSeats = TotalSeats - BookedSeats;
        }

        public override async Task Init()
        {
            var students = Task.Run(() => _studentQueries.GetAll().Select(a => new Item {Id = a.Id, Label = a.Firstname + " " + a.Lastname}));
            var companies = Task.Run(()=>_companyQueries.GetAll().Select(a=>new Item { Id = a.CompanyId, Label = a.Name}));
            var session = Task.Run(() => _sessionQueries.GetSession(_sessionId));

            await Task.WhenAll(students, companies, session);

            Students = new ObservableCollection<Item>(students.Result);
            Companies = new ObservableCollection<Item>(companies.Result);
            SessionInfos = new SessionInfos(session.Result);

            await RefreshSeats();
            RefreshCompteurs();
        }

        public RelayCommandAsync AddSeatCommand { get; set; }
        public RelayCommandAsync AddValidatedSeatCommand { get; set; }
        private async Task ExecuteAddSeatAsync(bool validate)
        {
            if (SelectedCompany == null)
            {
                MessageBox.Show("Veuillez indiquer le stagiaire ET la société pour réserver une place dans cette session.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await HandleMessageBoxError.ExecuteAsync(async () =>
            {
                var seat = await Task.Run(() => _applicationService.Command<BookSeat>().Execute(_sessionId, SelectedStudent?.Id, SelectedCompany.Id,!validate));
                if(validate)
                    await Task.Run(()=>_applicationService.Command<ValidateSeat>().Execute(seat.AggregateId));

                await RefreshSeats();
                SelectedStudent = null;
                SelectedCompany = null;
            });
        }

        private async Task RefreshSeats()
        {
            var items = await Task.Run(() => _seatQueries.GetAll(_sessionId).Select(a => new SeatItem(a, new FullName(a.StudentLastname, a.StudentFirstname).ToString(), a.CompanyName)));
            Seats = new ObservableCollection<SeatItem>(items);
            RefreshCompteurs();
        }

        public RelayCommandAsync CreateStudentCommand { get; }
        private async Task ExecuteCreateStudentAsync()
        {
            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer un stagiaire", new EditableStudent());
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync(async ()=>{                
                    var item = vm.Item as EditableStudent;
                    var newStagiaire = await Task.Run(() => _applicationService.Command<CreateStudent>().Execute(item.Lastname, item.Firstname));

                    var stagiaires = await Task.Run(() => _studentQueries.GetAll().Select(a => new Item { Id = a.Id, Label = a.Firstname + " " + a.Lastname }));
                    Students = new ObservableCollection<Item>(stagiaires);
                    SelectedStudent = Students.FirstOrDefault(a => a.Id == newStagiaire.AggregateId);                
                });
            }
        }

        public RelayCommandAsync CreateCompanyCommand { get; }
        private async Task ExecuteCreateSocieteAsync()
        {
            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer une société", new EditableCompany());
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync(async () => { 
                    var item = vm.Item as EditableCompany;
                    var newSociete = await Task.Run(() => _applicationService.Command<CreateCompany>().Execute(item.Name, item.Address, item.ZipCode, item.City));

                    var societes = await Task.Run(() => _companyQueries.GetAll().Select(a => new Item { Id = a.CompanyId, Label = a.Name}));
                    Companies = new ObservableCollection<Item>(societes);
                    SelectedCompany = Companies.FirstOrDefault(a => a.Id == newSociete.AggregateId);

                });
            }
        }

        public ObservableCollection<SeatItem> SelectedSeats
        {
            get => _selectedSeats;
            set { Set(()=>SelectedSeats, ref _selectedSeats, value); }
        }        

        public RelayCommandAsync CancelSeatCommand { get; set; }
        private async Task ExecuteCancelSeatAsync()
        {
            foreach (var selectedPlace in SelectedSeats)
            {
                var vm = await _applicationService.OpenPopup<ReasonWindowVm>();
                if (vm.IsValidated)
                {
                    await HandleMessageBoxError.ExecuteAsync(async () => {
                        await Task.Run(() =>
                        {
                            _applicationService.Command<CancelSeat>().Execute(selectedPlace.SeatId, vm.Reason);
                            
                        });                        
                    });
                }
            }
            await RefreshSeats();
        }
        public RelayCommandAsync ValidateSeatCommand { get; set; }
        private async Task ExecuteValidateSeatAsync()
        {
            foreach (var selectedPlace in SelectedSeats)
            {
                await HandleMessageBoxError.ExecuteAsync(async () =>
                {
                    await Task.Run(() =>
                    {
                        _applicationService.Command<ValidateSeat>().Execute(selectedPlace.SeatId);
                    });                    
                });
            }
            await RefreshSeats();
        }
        public RelayCommandAsync RefuseSeatCommand { get; set; }
        private async Task ExecuteRefuseSeatAsync()
        {
            foreach (var selectedPlace in SelectedSeats)
            {
                var vm = await _applicationService.OpenPopup<ReasonWindowVm>();
                if (vm.IsValidated)
                {
                    await HandleMessageBoxError.ExecuteAsync(async () =>
                    {
                        await Task.Run(() =>
                        {
                            _applicationService.Command<RefuseSeat>().Execute(selectedPlace.SeatId, vm.Reason);
                        });                        
                    });
                }
            }
            await RefreshSeats();
        }

        public RelayCommandAsync RefreshSeatsCommand { get; }
        private async Task ExecuteRefreshSeatsAsync()
        {
            await RefreshSeats();
        }

        public RelayCommandAsync GenerateAgreementCommand { get; }
        private async Task ExecuteGenerateAgreementAsync()
        {
            await _applicationService.OpenPopup<CreateAgreementWindowVm>(_sessionInfos, SelectedSeats.ToList());
            await RefreshSeats();
        }

        public RelayCommandAsync OpenAgreementCommand { get; }
        private async Task ExecuteOpenAgreementAsync()
        {
            var place = SelectedSeats.First();
            if (place.AgreementId.HasValue && !string.IsNullOrWhiteSpace(place.Agreement))
            {                
                await _applicationService.OpenPopup<ManageAgreementWindowVm>(place.AgreementId);
                await RefreshSeats();
            }
        }      

        public RelayCommandAsync EditStudentCommand { get; }
        private async Task ExecuteEditStudentAsync()
        {
            var selectedSeat = SelectedSeats.FirstOrDefault();

            var vm = await _applicationService.OpenPopup<EditStudentWindowVm>(Students, selectedSeat.StudentId.HasValue ? selectedSeat.StudentId.Value : Guid.Empty);
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync(async () =>
                {                    
                    await Task.Run(() =>_applicationService.Command<UpdateSeatStudent>().Execute(selectedSeat.SeatId, vm.SelectedStudent?.Id));
                    await RefreshSeats();
                });
            }            
        }
    }
}
