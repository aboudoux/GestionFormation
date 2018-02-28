using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GestionFormation.App.Views.Seats;
using GestionFormation.CoreDomain.Companies.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class CreateContactWindowVm : CreateItemVm
    {
        private readonly ICompanyQueries _companyQueries;
        private readonly EditableContact _source;
        private string _lastname;
        private string _firstname;
        private string _email;
        private string _telephone;
        private ObservableCollection<Item> _companies;
        private Item _selectedCompanie;

        public CreateContactWindowVm(string title, ICompanyQueries companyQueries, EditableContact source) : base(title, source)
        {
            _companyQueries = companyQueries ?? throw new ArgumentNullException(nameof(companyQueries));
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public override async Task Init()
        {
            Lastname = _source.Lastname;
            Firstname = _source.Firstname;
            Email = _source.Email;
            Telephone = _source.Telephone;            

            Companies = new ObservableCollection<Item>(await Task.Run(()=>_companyQueries.GetAll().Select(a=>new Item(){ Id = a.CompanyId, Label = a.Name})));
            SelectedCompanie = Companies.FirstOrDefault(a => a.Id == _source.GetSocieteId());
        }

        public string Lastname
        {
            get => _lastname;
            set { Set(() => Lastname, ref _lastname, value); }
        }

        public string Firstname
        {
            get => _firstname;
            set { Set(()=>Firstname, ref _firstname, value); }
        }

        public string Email
        {
            get => _email;
            set { Set(()=>Email, ref _email, value); }
        }

        public string Telephone
        {
            get => _telephone;
            set { Set(()=>Telephone, ref _telephone, value); }
        }

        public ObservableCollection<Item> Companies
        {
            get => _companies;
            set { Set(()=>Companies, ref _companies, value); }
        }

        public Item SelectedCompanie
        {
            get => _selectedCompanie;
            set { Set(()=>SelectedCompanie, ref _selectedCompanie, value); }
        }

        protected override async Task ExecuteValiderAsync()
        {
            if (SelectedCompanie == null)
            {
                MessageBox.Show("Veuillez sélectionner une société", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Item = new EditableContact(SelectedCompanie.Id, _source.GetId())
            { 
                Lastname = Lastname,
                Firstname = Firstname,
                Email = Email,
                Telephone = Telephone,                
            };
            await base.ExecuteValiderAsync();
        }
    }
}