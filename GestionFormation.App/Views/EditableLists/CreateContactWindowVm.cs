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
        private string _lastname;
        private string _firstname;
        private string _email;
        private string _telephone;
        private ObservableCollection<Item> _companies;
        private Item _selectedCompanie;

        public CreateContactWindowVm(string title, object item, ICompanyQueries companyQueries) : base(title, item)
        {
            _companyQueries = companyQueries;
        }

        public override async Task Init()
        {
            Companies = new ObservableCollection<Item>(await Task.Run(()=>_companyQueries.GetAll().Select(a=>new Item(){ Id = a.CompanyId, Label = a.Name})));
            _companyQueries.GetAll();
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

            Item = new EditableContact(SelectedCompanie.Id)
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