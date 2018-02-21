using System;
using System.Collections.Generic;
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
using GestionFormation.Infrastructure;

namespace GestionFormation.App.Views.Places
{
    public class GestionConventionWindowVm : PopupWindowVm
    {
        private readonly Guid _agreementId;
        private readonly IApplicationService _applicationService;
        private readonly ISeatQueries _seatQueries;
        private readonly IContactQueries _contactQueries;
        private readonly IDocumentCreator _documentCreator;
        private readonly IDocumentRepository _documentRepository;
        private readonly IAgreementQueries _agreementQueries;
        private readonly IComputerService _computerService;
        private string _nom;
        private string _prenom;
        private string _email;
        private string _telephone;
        private ObservableCollection<IAgreementSeatResult> _places;
        private string _documentPath;
        private bool _showSavedDocument;

        public GestionConventionWindowVm(Guid agreementId, ISeatQueries seatQueries, 
            IApplicationService applicationService, IContactQueries contactQueries, IAgreementQueries agreementQueries,
            IComputerService computerService, IDocumentCreator documentCreator, IDocumentRepository documentRepository)
        {
            _agreementId = agreementId;
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));            
            _seatQueries = seatQueries ?? throw new ArgumentNullException(nameof(seatQueries));
            _contactQueries = contactQueries ?? throw new ArgumentNullException(nameof(contactQueries));
            _documentCreator = documentCreator ?? throw new ArgumentNullException(nameof(documentCreator));
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
            _agreementQueries = agreementQueries ?? throw new ArgumentNullException(nameof(agreementQueries));
            _computerService = computerService ?? throw new ArgumentNullException(nameof(computerService));

            ChooseDocumentCommand = new RelayCommand(ExecuteChooseDocumentAsync);
            PrintCommand = new RelayCommandAsync(ExecutePrintAsync);
            SendMailCommand = new RelayCommandAsync(ExecuteSendEmailAsync);
            OpenSignedDocumentCommand = new RelayCommandAsync(ExecuteOpenSignedDocumentAsync);
            ReassignSignedDocumentCommand = new RelayCommand(ExecuteReassignSignedDocument);
            RemindAgreementCommand = new RelayCommandAsync(ExecuteRemingAgreementAsync);

            SetValiderCommandCanExecute(()=>File.Exists(DocumentPath));
        }        

        public override async Task Init()
        {
            var placesTask = Task.Run(()=>_seatQueries.GetSeatAgreements(_agreementId));
            var documentTask = Task.Run(() => _agreementQueries.GetSignedAgreementDocumentId(_agreementId));
            var contactTask = LoadContact(_agreementId);

            await Task.WhenAll(placesTask, contactTask, documentTask);

            Places = new ObservableCollection<IAgreementSeatResult>(placesTask.Result);
            SignedDocumentId = documentTask.Result;
            if (SignedDocumentId.HasValue)
                ShowSavedDocument = true;
        }

        public Guid? SignedDocumentId { get; set; }

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

        public bool ShowSavedDocument
        {
            get => _showSavedDocument;
            set { Set(()=>ShowSavedDocument, ref _showSavedDocument, value); }
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
            await HandleMessageBoxError.ExecuteAsync(async ()=>
            {
                var doc = await GenerateAgreementDocument();
                Process.Start(doc);
            });
        }

        public RelayCommandAsync SendMailCommand { get; }
        private async Task ExecuteSendEmailAsync()
        {
            await HandleMessageBoxError.ExecuteAsync(async () =>
            {                
                var doc = await GenerateAgreementDocument();
                var conv = await Task.Run(() => _agreementQueries.GetPrintableAgreement(_agreementId));
                
                _computerService.OpenMailInOutlook($"TREND - Convention de formation {conv.Training} du {conv.StartDate:d}", 
                    "Bonjour," + Environment.NewLine +
                    $"Veuillez trouver ci-joint la convention � nous retourner sign�e pour la formation {conv.Training} du {conv.StartDate:D} qui se d�roulera sur {conv.Duration} jour(s)" + Environment.NewLine + Environment.NewLine +
                    "En vous souhaitant bonne r�ception." + Environment.NewLine +
                    "Cordialement," + Environment.NewLine,
                    new List<MailAttachement>(){ new MailAttachement(doc, "convention")}, Email);
            });
        }

        public RelayCommandAsync OpenSignedDocumentCommand { get; }
        private async Task ExecuteOpenSignedDocumentAsync()
        {
            var documentPath = await Task.Run(()=>_documentRepository.GetDocument(SignedDocumentId.Value));
            Process.Start(documentPath);
        }

        public RelayCommand ReassignSignedDocumentCommand { get; }
        private async Task ExecuteRemingAgreementAsync()
        {
            var doc = await GenerateAgreementDocument();
            var conv = await Task.Run(() => _agreementQueries.GetPrintableAgreement(_agreementId));

            _computerService.OpenMailInOutlook("Retour convention sign�e",
                "Bonjour," + Environment.NewLine +
                $"Dans le cadre de la formation {conv.Training} du {conv.StartDate:D}, nous sommes toujours en attente d'un retour de convention sign�e." + Environment.NewLine +
                "Pouvez-vous s'il vous plait nous la faire parvenir au plus vite afin que nous puissions avancer sur le dossier." + Environment.NewLine +
                "En vous souhaitant bonne reception." + Environment.NewLine +
                "Cordialement,", new List<MailAttachement>() { new MailAttachement(doc, "convention") }, Email);                           
        }

        private void ExecuteReassignSignedDocument()
        {
            ShowSavedDocument = false;
        }

        public RelayCommandAsync RemindAgreementCommand { get; }

        private async Task<string> GenerateAgreementDocument()
        {
            var firstPlace = Places.First();

            var conv = await Task.Run(() => _agreementQueries.GetPrintableAgreement(_agreementId));

            if (conv.AgreementType == AgreementType.Free)
                return _documentCreator.CreateFreeAgreement(conv.AgreementNumber, firstPlace.Company, firstPlace.Address, firstPlace.ZipCode, firstPlace.City, new FullName(Nom, Prenom), conv.Training, conv.StartDate, conv.Duration, conv.Location, Places.Select(a => new Participant(a.Student, a.Company)).ToList());

            return _documentCreator.CreatePaidAgreement(conv.AgreementNumber, firstPlace.Company, firstPlace.Address, firstPlace.ZipCode, firstPlace.City, new FullName(Nom, Prenom), conv.Training, conv.StartDate, conv.Duration, conv.Location, Places.Select(a => new Participant(a.Student, a.Company)).ToList());
        }

        protected override async Task ExecuteValiderAsync()
        {
            if (!File.Exists(DocumentPath))
            {
                MessageBox.Show("Le fichier s�lectionn� est introuvable", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await HandleMessageBoxError.ExecuteAsync(async () => { 
                var documentId = await Task.Run(()=>_documentRepository.Save(DocumentPath));
                await Task.Run(()=>
                {
                    _applicationService.Command<SignAgreement>().Execute(_agreementId, documentId);
                });
                await base.ExecuteValiderAsync();
            });
        }

        public override string Title => "Gestion de convention";
    }
}