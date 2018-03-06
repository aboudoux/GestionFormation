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

namespace GestionFormation.App.Views.Seats
{
    public class ManageAgreementWindowVm : PopupWindowVm
    {
        private readonly Guid _agreementId;
        private readonly IApplicationService _applicationService;
        private readonly ISeatQueries _seatQueries;
        private readonly IContactQueries _contactQueries;
        private readonly IDocumentCreator _documentCreator;
        private readonly IDocumentRepository _documentRepository;
        private readonly IAgreementQueries _agreementQueries;
        private readonly IComputerService _computerService;
        private string _lastname;
        private string _firstname;
        private string _email;
        private string _telephone;
        private ObservableCollection<IAgreementSeatResult> _seats;
        private string _documentPath;
        private bool _showSavedDocument;
        private string _agreementType;
        private bool _showPrices;
        private AgreementPriceType _agreementPriceType;
        private decimal _pricePerDayAndPerStudent;
        private decimal _packagePrice;
        private bool _canSavePrices;

        public ManageAgreementWindowVm(Guid agreementId, ISeatQueries seatQueries, 
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
            SavePricesCommand = new RelayCommandAsync(ExecuteSavePricesAsync, () => CanSavePrices);

            SetValiderCommandCanExecute(()=>File.Exists(DocumentPath));
        }        

        public override async Task Init()
        {
            var placesTask = Task.Run(()=>_seatQueries.GetSeatAgreements(_agreementId));
            var documentTask = Task.Run(() => _agreementQueries.GetAgreementDocument(_agreementId));
            var agreementTask = Task.Run(() => _agreementQueries.GetPrintableAgreement(_agreementId));
            var contactTask = LoadContact(_agreementId);                        

            await Task.WhenAll(placesTask, contactTask, documentTask, agreementTask);

            Seats = new ObservableCollection<IAgreementSeatResult>(placesTask.Result);

            SignedDocumentId = documentTask.Result.SignedDocumentId;
            AAgreementType = documentTask.Result.Type == AgreementType.Free ? "Gratuite" : "Payante";
            ShowPrices = documentTask.Result.Type == AgreementType.Paid;

            if (SignedDocumentId.HasValue)
                ShowSavedDocument = true;

            PricePerDayAndPerStudent = agreementTask.Result.PricePerDayAndPerStudent;            
            PackagePrice = agreementTask.Result.PackagePrice;            
            if (PricePerDayAndPerStudent == 0 && PackagePrice == 0)
            {
                AgreementPriceType = AgreementPriceType.DetailedPrice;
                PricePerDayAndPerStudent = 450; 
            }
            else if(agreementTask.Result.PackagePrice > 0)
            {
                AgreementPriceType = AgreementPriceType.PackagePrice;
                PackagePrice = agreementTask.Result.PackagePrice;
            }
            else
            {
                AgreementPriceType = AgreementPriceType.DetailedPrice;
                PricePerDayAndPerStudent = agreementTask.Result.PricePerDayAndPerStudent;
            }
            CanSavePrices = false;
        }

        public Guid? SignedDocumentId { get; set; }

        private async Task LoadContact(Guid conventionId)
        {
            var contact = await Task.Run(() => _contactQueries.GetAgreementContact(conventionId));
            if (contact != null)
            {
                Lastname = contact.Lastname;
                Firstname = contact.Firstname;
                Email = contact.Email;
                Telephone = contact.Telephone;                
            }
        }

        public bool ShowSavedDocument
        {
            get => _showSavedDocument;
            set { Set(()=>ShowSavedDocument, ref _showSavedDocument, value); }
        }

        public ObservableCollection<IAgreementSeatResult> Seats
        {
            get => _seats;
            set { Set(()=>Seats, ref _seats, value); }
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

        public string DocumentPath
        {
            get => _documentPath;
            set
            {
                Set(()=>DocumentPath, ref _documentPath, value);
                ValiderCommand.RaiseCanExecuteChanged();
            }
        }

        public string AAgreementType
        {
            get => _agreementType;
            set { Set(()=>AAgreementType, ref _agreementType, value); }
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
                    $"Veuillez trouver ci-joint la convention à nous retourner signée pour la formation {conv.Training} du {conv.StartDate:D} qui se déroulera sur {conv.Duration} jour(s)" + Environment.NewLine + Environment.NewLine +
                    "En vous souhaitant bonne réception." + Environment.NewLine +
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

            _computerService.OpenMailInOutlook("Retour convention signée",
                "Bonjour," + Environment.NewLine +
                $"Dans le cadre de la formation {conv.Training} du {conv.StartDate:D}, nous sommes toujours en attente d'un retour de convention signée." + Environment.NewLine +
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
            var firstPlace = Seats.First();

            var conv = await Task.Run(() => _agreementQueries.GetPrintableAgreement(_agreementId));

            if (conv.AgreementType == AgreementType.Free)
                return _documentCreator.CreateFreeAgreement(conv.AgreementNumber, firstPlace.Company, firstPlace.Address, firstPlace.ZipCode, firstPlace.City, new FullName(Lastname, Firstname), conv.Training, conv.StartDate, conv.Duration, conv.Location, Seats.Select(a => new Attendee(a.Student, a.Company)).ToList());

            var type = AgreementPriceType;
            var price = type == AgreementPriceType.DetailedPrice ? PricePerDayAndPerStudent : PackagePrice;
            return _documentCreator.CreatePaidAgreement(conv.AgreementNumber, firstPlace.Company, firstPlace.Address, firstPlace.ZipCode, firstPlace.City, new FullName(Lastname, Firstname), conv.Training, conv.StartDate, conv.Duration, conv.Location, Seats.Select(a => new Attendee(a.Student, a.Company)).ToList(), type, price);
        }

        protected override async Task ExecuteValiderAsync()
        {
            if (!File.Exists(DocumentPath))
            {
                MessageBox.Show("Le fichier sélectionné est introuvable", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public bool ShowPrices
        {
            get => _showPrices;
            set { Set(()=>ShowPrices, ref _showPrices, value); }
        }

        public AgreementPriceType AgreementPriceType
        {
            get => _agreementPriceType;
            set
            {
                Set(()=>AgreementPriceType, ref _agreementPriceType, value);
                CanSavePrices = false;
            }
        }
      

        public decimal PricePerDayAndPerStudent
        {
            get => _pricePerDayAndPerStudent;
            set
            {
                Set(()=>PricePerDayAndPerStudent, ref _pricePerDayAndPerStudent, value);
                CanSavePrices = true;
            }
        }

        public decimal PackagePrice
        {
            get => _packagePrice;
            set
            {
                Set(()=>PackagePrice, ref _packagePrice, value);
                CanSavePrices = true;
            }
        }

        public bool CanSavePrices
        {
            get => _canSavePrices;
            set
            {
                Set(()=>CanSavePrices, ref _canSavePrices, value);
                SavePricesCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommandAsync SavePricesCommand { get; }
        private async Task ExecuteSavePricesAsync()
        {
            await HandleMessageBoxError.ExecuteAsync(async () =>
            {
                if (AgreementPriceType == AgreementPriceType.DetailedPrice)
                {
                    await Task.Run(() => _applicationService.Command<UpdateAgreement>().ByDetailedPrice(_agreementId, PricePerDayAndPerStudent));
                }
                else
                {
                    await Task.Run(() => _applicationService.Command<UpdateAgreement>().ByPackagePrice(_agreementId, PackagePrice));
                }

                CanSavePrices = false;
            });            
        }

        public override string Title => "Gestion de convention";
    }
}