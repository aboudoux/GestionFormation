using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.App.Views.EditableLists;
using GestionFormation.Applications.Contacts;
using GestionFormation.Applications.Conventions;
using GestionFormation.CoreDomain.Contacts.Queries;

namespace GestionFormation.App.Views.Places
{
    public class CreateConventionWindowVm : PopupWindowVm
    {
        private readonly IApplicationService _applicationService;
        private readonly IContactQueries _contactQueries;
        private readonly List<PlaceItem> _selectedPlaces;
        private ObservableCollection<PlaceItem> _places;
        private ObservableCollection<ContactItem> _contacts;
        private ContactItem _selectedContact;

        public CreateConventionWindowVm(IApplicationService applicationService, IContactQueries contactQueries, List<PlaceItem> selectedPlaces)
        {            
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _contactQueries = contactQueries ?? throw new ArgumentNullException(nameof(contactQueries));
            _selectedPlaces = selectedPlaces ?? throw new ArgumentNullException(nameof(selectedPlaces));
            AddContactCommand = new RelayCommandAsync(ExecuteAddContactAsync);
        }

        public override async Task Init()
        {
            Places = new ObservableCollection<PlaceItem>(_selectedPlaces);
            await InitContacts(null);
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
            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer un contact", new EditableContact());
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.Execute(async () =>
                {
                    var item = vm.Item as EditableContact;
                    var newItem = await Task.Run(() => _applicationService.Command<CreateContact>().Execute(item.Nom, item.Prenom, item.Email, item.Telephone));
                    await InitContacts(newItem.AggregateId);
                });
            }
        }

        private async Task InitContacts(Guid? selectedFormationId)
        {
            var contactsTask = await Task.Run(() => _contactQueries.GetAll().Select(a => new ContactItem(a)));
            Contacts = new ObservableCollection<ContactItem>(contactsTask);
            SelectedContact = Contacts.FirstOrDefault(a => a.Id == selectedFormationId);
        }

        public ContactItem SelectedContact
        {
            get => _selectedContact;
            set { Set(()=>SelectedContact, ref _selectedContact, value); }
        }

        protected override async Task ExecuteValiderAsync()
        {
            await HandleMessageBoxError.Execute(async () => {
                await Task.Run(() => _applicationService.Command<CreateConvention>().Execute(SelectedContact.Id, Places.Select(a => a.PlaceId)));
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