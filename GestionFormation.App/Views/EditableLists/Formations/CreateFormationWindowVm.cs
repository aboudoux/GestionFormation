using System.Threading.Tasks;

namespace GestionFormation.App.Views.EditableLists.Formations
{
    public class CreateFormationWindowVm : CreateItemVm
    {
        private string _nom;
        private int _places;
        private System.Windows.Media.Color _couleur;

        public CreateFormationWindowVm(string title, object item) : base(title, item)
        {
        }

        public string Nom
        {
            get => _nom;
            set { Set(()=>Nom, ref _nom, value); }
        }

        public int Places
        {
            get => _places;
            set { Set(()=>Places, ref _places, value); }
        }

        public System.Windows.Media.Color Couleur
        {
            get => _couleur;
            set { Set(()=>Couleur, ref _couleur, value); }
        }

        protected override Task ExecuteValiderAsync()
        {
            Item = new EditableFormation()
            {
                Nom = Nom,
                Places = Places,
                Couleur = Couleur
            };
            return base.ExecuteValiderAsync();
        }
    }
}