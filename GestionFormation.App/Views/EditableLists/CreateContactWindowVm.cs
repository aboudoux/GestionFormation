using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GestionFormation.App.Views.Places;
using GestionFormation.CoreDomain.Societes.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class CreateContactWindowVm : CreateItemVm
    {
        private readonly ISocieteQueries _societeQueries;
        private string _nom;
        private string _prenom;
        private string _email;
        private string _telephone;
        private ObservableCollection<Item> _societes;
        private Item _selectedSociete;

        public CreateContactWindowVm(string title, object item, ISocieteQueries societeQueries) : base(title, item)
        {
            _societeQueries = societeQueries;
        }

        public override async Task Init()
        {
            Societes = new ObservableCollection<Item>(await Task.Run(()=>_societeQueries.GetAll().Select(a=>new Item(){ Id = a.SocieteId, Label = a.Nom})));
            _societeQueries.GetAll();
        }

        public string Nom
        {
            get => _nom;
            set { Set(() => Nom, ref _nom, value); }
        }

        public string Prenom
        {
            get => _prenom;
            set { Set(()=>Prenom, ref _prenom, value); }
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

        public ObservableCollection<Item> Societes
        {
            get => _societes;
            set { Set(()=>Societes, ref _societes, value); }
        }

        public Item SelectedSociete
        {
            get => _selectedSociete;
            set { Set(()=>SelectedSociete, ref _selectedSociete, value); }
        }

        protected override async Task ExecuteValiderAsync()
        {
            if (SelectedSociete == null)
            {
                MessageBox.Show("Veuillez sélectionner une société", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Item = new EditableContact(SelectedSociete.Id)
            { 
                Nom = Nom,
                Prenom = Prenom,
                Email = Email,
                Telephone = Telephone,                
            };
            await base.ExecuteValiderAsync();
        }
    }
}