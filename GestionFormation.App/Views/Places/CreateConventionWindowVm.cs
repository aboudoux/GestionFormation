using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GestionFormation.App.Core;
using GestionFormation.App.Views.EditableLists;
using GestionFormation.Applications.Contacts;
using GestionFormation.Applications.Conventions;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Contacts.Queries;
using GestionFormation.CoreDomain.Conventions;

namespace GestionFormation.App.Views.Places
{
    public class CreateConventionWindowVm : PopupWindowVm
    {
        private readonly SessionInfos _sessionInfos;
        private readonly IApplicationService _applicationService;
        private readonly IContactQueries _contactQueries;
        private readonly List<PlaceItem> _selectedPlaces;
        private readonly IComputerService _computerService;
        private ObservableCollection<PlaceItem> _places;
        private ObservableCollection<ContactItem> _contacts;
        private ContactItem _selectedContact;
        private TypeConvention _typeConvention;

        public CreateConventionWindowVm(SessionInfos sessionInfos, IApplicationService applicationService, IContactQueries contactQueries, List<PlaceItem> selectedPlaces, IComputerService computerService)
        {
            _sessionInfos = sessionInfos ?? throw new ArgumentNullException(nameof(sessionInfos));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _contactQueries = contactQueries ?? throw new ArgumentNullException(nameof(contactQueries));
            _selectedPlaces = selectedPlaces ?? throw new ArgumentNullException(nameof(selectedPlaces));
            _computerService = computerService ?? throw new ArgumentNullException(nameof(computerService));
            AddContactCommand = new RelayCommandAsync(ExecuteAddContactAsync);
            UnknowTypeConventionCommand = new RelayCommand(ExecuteUnknowTypeConvention);

            SetValiderCommandCanExecute(() => SelectedContact != null);
        }

        public override async Task Init()
        {
            Places = new ObservableCollection<PlaceItem>(_selectedPlaces);
            await InitContacts(null);
        }

        public TypeConvention TypeConvention
        {
            get => _typeConvention;
            set { Set(() => TypeConvention, ref _typeConvention, value); }
        }

        public RelayCommand UnknowTypeConventionCommand { get; }
        private void ExecuteUnknowTypeConvention()
        {
            if (!Places.Any())
            {
                MessageBox.Show("Aucune place n'a été validée pour cette convention", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            HandleMessageBoxError.Execute(()=>{

                var firstPlace = Places.First();

                _computerService.OpenTypeConventionMail(
                    $"Convention formation {_sessionInfos.FormationName} du {_sessionInfos.Result.DateDebut:d} société {firstPlace.SocieteNom}",
                    "Bonjour," + Environment.NewLine + Environment.NewLine +
                    $"La société {firstPlace.SocieteNom} envoie {Places.Count} stagiaire(s) à la formation {_sessionInfos.FormationName} le {_sessionInfos.Result.DateDebut:D}." + Environment.NewLine + Environment.NewLine +
                    "La convention doit-elle être gratuite ou payante ?" + Environment.NewLine + Environment.NewLine +
                    "Merci pour votre retour rapide" + Environment.NewLine + Environment.NewLine +
                    "Cordialement," + Environment.NewLine
                    );
            });
        }

        public ObservableCollection<PlaceItem> Places
        {
            get => _places;
            set { Set(() => Places, ref _places, value); }
        }

        public ObservableCollection<ContactItem> Contacts
        {
            get => _contacts;
            set { Set(()=>Contacts, ref _contacts, value); }
        }

        public RelayCommandAsync AddContactCommand { get; }
        private async Task ExecuteAddContactAsync()
        {
            var firstPlace = Places.FirstOrDefault();
            if (firstPlace == null)
                return;

            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer un contact", new EditableContact(firstPlace.SocieteId));
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync(async () =>
                {
                    var item = vm.Item as EditableContact;
                    var newItem = await Task.Run(() => _applicationService.Command<CreateContact>().Execute(item.GetSocieteId(), item.Nom, item.Prenom, item.Email, item.Telephone));
                    await InitContacts(newItem.AggregateId);
                });
            }
        }

        private async Task InitContacts(Guid? selectedFormationId)
        {
            var firstPlace = Places.FirstOrDefault();
            if(firstPlace == null)
                return;

            var contactsTask = await Task.Run(() => _contactQueries.GetAll(firstPlace.SocieteId).Select(a => new ContactItem(a)));
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
                await Task.Run(() => _applicationService.Command<CreateConvention>().Execute(SelectedContact.Id, Places.Select(a => a.PlaceId), TypeConvention));
                await base.ExecuteValiderAsync();
            });
        }
    }

    public class ContactItem
    {
        public ContactItem(IContactResult contactResult)
        {
            Id = contactResult.Id;
            Nom = contactResult.Nom;
            Prenom = contactResult.Prenom;
            Telephone = contactResult.Telephone;
            Email = contactResult.Email;
        }
        public Guid Id { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public string Telephone { get; }
        public string Email { get; }

        public override string ToString()
        {
            return Nom + " " + Prenom;
        }
    }
}