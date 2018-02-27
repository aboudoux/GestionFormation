using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GestionFormation.App.Core;
using GestionFormation.App.Views.EditableLists;
using GestionFormation.Applications.Agreements;
using GestionFormation.Applications.Contacts;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Contacts.Queries;

namespace GestionFormation.App.Views.Seats
{
    public class CreateAgreementWindowVm : PopupWindowVm
    {
        private readonly SessionInfos _sessionInfos;
        private readonly IApplicationService _applicationService;
        private readonly IContactQueries _contactQueries;
        private readonly List<SeatItem> _selectedPlaces;
        private readonly IComputerService _computerService;
        private ObservableCollection<SeatItem> _seats;
        private ObservableCollection<ContactItem> _contacts;
        private ContactItem _selectedContact;
        private AgreementType _agreementType;

        public CreateAgreementWindowVm(SessionInfos sessionInfos, IApplicationService applicationService, IContactQueries contactQueries, List<SeatItem> selectedPlaces, IComputerService computerService)
        {
            _sessionInfos = sessionInfos ?? throw new ArgumentNullException(nameof(sessionInfos));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _contactQueries = contactQueries ?? throw new ArgumentNullException(nameof(contactQueries));
            _selectedPlaces = selectedPlaces ?? throw new ArgumentNullException(nameof(selectedPlaces));
            _computerService = computerService ?? throw new ArgumentNullException(nameof(computerService));
            AddContactCommand = new RelayCommandAsync(ExecuteAddContactAsync);
            UnknowTypeAgreementCommand = new RelayCommand(ExecuteUnknowTypeConvention);

            SetValiderCommandCanExecute(() => SelectedContact != null && AgreementType != AgreementType.Unknow);
        }

        public override async Task Init()
        {
            Seats = new ObservableCollection<SeatItem>(_selectedPlaces);
            await InitContacts(null);
        }

        public AgreementType AgreementType
        {
            get => _agreementType;
            set
            {
                Set(() => AgreementType, ref _agreementType, value);
                ValiderCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand UnknowTypeAgreementCommand { get; }
        private void ExecuteUnknowTypeConvention()
        {
            if (!Seats.Any())
            {
                MessageBox.Show("Aucune place n'a été validée pour cette convention", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            HandleMessageBoxError.Execute(()=>{

                var firstPlace = Seats.First();

                _computerService.OpenMailInOutlook(
                    $"Convention formation {_sessionInfos.TrainingName} du {_sessionInfos.Result.SessionStart:d} société {firstPlace.CompanyName}",
                    "Bonjour," + Environment.NewLine + Environment.NewLine +
                    $"La société {firstPlace.CompanyName} envoie {Seats.Count} stagiaire(s) à la formation {_sessionInfos.TrainingName} le {_sessionInfos.Result.SessionStart:D}." + Environment.NewLine + Environment.NewLine +
                    "La convention doit-elle être gratuite ou payante ?" + Environment.NewLine + Environment.NewLine +
                    "Merci pour votre retour rapide" + Environment.NewLine + Environment.NewLine +
                    "Cordialement," + Environment.NewLine
                    );
            });
        }

        public ObservableCollection<SeatItem> Seats
        {
            get => _seats;
            set { Set(() => Seats, ref _seats, value); }
        }

        public ObservableCollection<ContactItem> Contacts
        {
            get => _contacts;
            set { Set(()=>Contacts, ref _contacts, value); }
        }

        public RelayCommandAsync AddContactCommand { get; }
        private async Task ExecuteAddContactAsync()
        {
            var firstPlace = Seats.FirstOrDefault();
            if (firstPlace == null)
                return;

            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer un contact", new EditableContact(firstPlace.CompanyId));
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync(async () =>
                {
                    var item = vm.Item as EditableContact;
                    var newItem = await Task.Run(() => _applicationService.Command<CreateContact>().Execute(item.GetSocieteId(), item.Lastname, item.Firstname, item.Email, item.Telephone));
                    await InitContacts(newItem.AggregateId);
                });
            }
        }

        private async Task InitContacts(Guid? selectedFormationId)
        {
            var firstPlace = Seats.FirstOrDefault();
            if(firstPlace == null)
                return;

            var contactsTask = await Task.Run(() => _contactQueries.GetAll(firstPlace.CompanyId).Select(a => new ContactItem(a)));
            Contacts = new ObservableCollection<ContactItem>(contactsTask);
            SelectedContact = Contacts.FirstOrDefault(a => a.Id == selectedFormationId);
        }

        public ContactItem SelectedContact
        {
            get => _selectedContact;
            set
            {
                Set(()=>SelectedContact, ref _selectedContact, value);
                ValiderCommand.RaiseCanExecuteChanged();
            }
        }

        protected override async Task ExecuteValiderAsync()
        {
            await HandleMessageBoxError.ExecuteAsync(async () => {
                await Task.Run(() =>
                {
                    var result = _applicationService.Command<CreateAgreement>().Execute(SelectedContact.Id, Seats.Select(a => a.SeatId), AgreementType);
                    return result;

                });
                await base.ExecuteValiderAsync();
            });
        }

        public override string Title => "Créer une convention";
    }
}