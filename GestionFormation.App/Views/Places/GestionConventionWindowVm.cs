using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight.CommandWpf;
using GestionFormation.App.Core;
using GestionFormation.Applications.Conventions;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Contacts.Queries;
using GestionFormation.CoreDomain.Conventions.Queries;
using GestionFormation.CoreDomain.Places.Queries;

namespace GestionFormation.App.Views.Places
{
    public class GestionConventionWindowVm : PopupWindowVm
    {
        private readonly IApplicationService _applicationService;
        private readonly Guid _conventionId;
        private readonly IConventionQueries _conventionQueries;
        private readonly IPlacesQueries _placesQueries;
        private readonly IContactQueries _contactQueries;
        private readonly IDocumentRepository _documentRepository;
        private string _nom;
        private string _prenom;
        private string _email;
        private string _telephone;
        private ObservableCollection<IConventionPlaceResult> _places;
        private string _documentPath;

        public GestionConventionWindowVm(Guid conventionId, IConventionQueries conventionQueries, IPlacesQueries placesQueries, IApplicationService applicationService, IContactQueries contactQueries, IDocumentRepository documentRepository)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));            
            _conventionQueries = conventionQueries ?? throw new ArgumentNullException(nameof(conventionQueries));
            _placesQueries = placesQueries ?? throw new ArgumentNullException(nameof(placesQueries));
            _contactQueries = contactQueries ?? throw new ArgumentNullException(nameof(contactQueries));
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
            _conventionId = conventionId;

            ChooseDocumentCommand = new RelayCommand(ExecuteChooseDocumentAsync);
        }
                
        public override async Task Init()
        {
            var placesTask = Task.Run(()=>_placesQueries.GetConventionPlaces(_conventionId));
            var contactTask = LoadContact(_conventionId);

            await Task.WhenAll(placesTask, contactTask);

            Places = new ObservableCollection<IConventionPlaceResult>(placesTask.Result);
        }

        private async Task LoadContact(Guid conventionId)
        {
            var contact = await Task.Run(() => _contactQueries.GetConventionContact(conventionId));
            if (contact != null)
            {
                Nom = contact.Nom;
                Prenom = contact.Prenom;
                Email = contact.Email;
                Telephone = contact.Telephone;
            }
        }

        public ObservableCollection<IConventionPlaceResult> Places
        {
            get => _places;
            set { Set(()=>Places, ref _places, value); }
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

        public string DocumentPath
        {
            get => _documentPath;
            set { Set(()=>DocumentPath, ref _documentPath, value); }
        }

        public RelayCommand ChooseDocumentCommand { get; }
        private void ExecuteChooseDocumentAsync()
        {
            var openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Pdf|*.pdf|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;
          
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                DocumentPath = openFileDialog1.FileName;
        }

        protected override async Task ExecuteValiderAsync()
        {
            if (!File.Exists(DocumentPath))
            {
                MessageBox.Show("Le fichier sélectionné est introuvable", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await HandleMessageBoxError.Execute(async () => { 
                var documentId = await Task.Run(()=>_documentRepository.SaveConvention(DocumentPath));
                await Task.Run(()=>_applicationService.Command<SignConvention>().Execute(_conventionId, documentId));
                await base.ExecuteValiderAsync();
            });

        }             
    }
}