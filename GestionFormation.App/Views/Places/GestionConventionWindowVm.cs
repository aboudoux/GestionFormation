using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight.CommandWpf;
using GestionFormation.App.Core;
using GestionFormation.Applications.Agreements;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Agreements.Queries;
using GestionFormation.CoreDomain.Contacts.Queries;
using GestionFormation.CoreDomain.Seats.Queries;
using GestionFormation.CoreDomain.Sessions.Queries;

namespace GestionFormation.App.Views.Places
{
    public class GestionConventionWindowVm : PopupWindowVm
    {
        private readonly Guid _conventionId;
        private readonly IApplicationService _applicationService;
        private readonly ISeatQueries _seatQueries;
        private readonly IContactQueries _contactQueries;
        private readonly IDocumentRepository _documentRepository;
        private readonly IAgreementQueries _agreementQueries;
        private string _nom;
        private string _prenom;
        private string _email;
        private string _telephone;
        private ObservableCollection<IAgreementSeatResult> _places;
        private string _documentPath;

        public GestionConventionWindowVm(Guid conventionId, ISeatQueries seatQueries, IApplicationService applicationService, IContactQueries contactQueries, IDocumentRepository documentRepository, IAgreementQueries agreementQueries)
        {
            _conventionId = conventionId;
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));            
            _seatQueries = seatQueries ?? throw new ArgumentNullException(nameof(seatQueries));
            _contactQueries = contactQueries ?? throw new ArgumentNullException(nameof(contactQueries));
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
            _agreementQueries = agreementQueries ?? throw new ArgumentNullException(nameof(agreementQueries));

            ChooseDocumentCommand = new RelayCommand(ExecuteChooseDocumentAsync);
            PrintCommand = new RelayCommandAsync(ExecutePrintAsync);

            SetValiderCommandCanExecute(()=>File.Exists(DocumentPath));
        }
        
        public override async Task Init()
        {
            var placesTask = Task.Run(()=>_seatQueries.GetSeatAgreements(_conventionId));
            var contactTask = LoadContact(_conventionId);

            await Task.WhenAll(placesTask, contactTask);

            Places = new ObservableCollection<IAgreementSeatResult>(placesTask.Result);
        }

        private async Task LoadContact(Guid conventionId)
        {
            var contact = await Task.Run(() => _contactQueries.GetAgreementContact(conventionId));
            if (contact != null)
            {
                Nom = contact.Lastname;
                Prenom = contact.Firstname;
                Email = contact.Email;
                Telephone = contact.Telephone;
            }
        }

        public ObservableCollection<IAgreementSeatResult> Places
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
            set
            {
                Set(()=>DocumentPath, ref _documentPath, value);
                ValiderCommand.RaiseCanExecuteChanged();
            }
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

        public RelayCommandAsync PrintCommand { get; }
        private async Task ExecutePrintAsync()
        {
            await HandleMessageBoxError.ExecuteAsync(async ()=>{

                var firstPlace = Places.First();
                string doc;

                var conv = await Task.Run(()=>_agreementQueries.GetPrintableAgreement(_conventionId));

                if (conv.AgreementType == AgreementType.Free)
                    doc = _documentRepository.CreateConventionGratuite(conv.AgreementNumber, firstPlace.Company, firstPlace.Address, firstPlace.ZipCode, firstPlace.City, new FullName(Nom, Prenom), conv.Training, conv.StartDate, conv.Duration, conv.Location, Places.Select(a=>new Participant(a.Student, a.Company)).ToList());
                else
                    doc = _documentRepository.CreateConventionPayante(conv.AgreementNumber, firstPlace.Company, firstPlace.Address, firstPlace.ZipCode, firstPlace.City, new FullName(Nom, Prenom), conv.Training, conv.StartDate, conv.Duration, conv.Location, Places.Select(a => new Participant(a.Student, a.Company)).ToList());

                Process.Start(doc);
            });
        }

        protected override async Task ExecuteValiderAsync()
        {
            if (!File.Exists(DocumentPath))
            {
                MessageBox.Show("Le fichier sélectionné est introuvable", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await HandleMessageBoxError.ExecuteAsync(async () => { 
                var documentId = await Task.Run(()=>_documentRepository.SaveConvention(DocumentPath));
                await Task.Run(()=>_applicationService.Command<SignAgreement>().Execute(_conventionId, documentId));
                await base.ExecuteValiderAsync();
            });

        }             
    }
}