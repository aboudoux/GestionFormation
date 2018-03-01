using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Mvvm;
using GestionFormation.App.Core;
using GestionFormation.Applications.Seats;
using GestionFormation.Applications.Sessions;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Seats.Queries;
using GestionFormation.CoreDomain.Sessions.Queries;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.App.Views.Sessions
{
    public class CloseSessionWindowVm : PopupWindowVm
    {
        private readonly Guid _sessionId;
        private readonly IApplicationService _applicationService;
        private readonly ISessionQueries _sessionQueries;
        private readonly ISeatQueries _seatQueries;
        private readonly IDocumentCreator _documentCreator;
        private readonly IComputerService _computerService;
        private readonly DocumentManager _documentManager;

        private string _trainingName;
        private DateTime _sessionStart;
      
        private Guid? _timesheetId;
        private string _sessionTitle;
        private ObservableCollection<StudentItem> _seats;

        private Guid? TimesheetId
        {
            get => _timesheetId;
            set
            {
                _timesheetId = value;
                RaisePropertyChanged(nameof(TimesheetAvailable));
                Messenger.Default.Send(new RefreshButtonMessage());
            }
        }

        public CloseSessionWindowVm(Guid sessionId, IApplicationService applicationService, 
            IDocumentRepository documentRepository, ISessionQueries sessionQueries, 
            ISeatQueries seatQueries, IDocumentCreator documentCreator, IComputerService computerService)
        {
            if (documentCreator == null) throw new ArgumentNullException(nameof(documentCreator));
            if (computerService == null) throw new ArgumentNullException(nameof(computerService));
            GuidAssert.AreNotEmpty(sessionId);
            Seats = new ObservableCollection<StudentItem>();
            _sessionId = sessionId;
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _documentManager = new DocumentManager(documentRepository);
            _sessionQueries = sessionQueries ?? throw new ArgumentNullException(nameof(sessionQueries));
            _seatQueries = seatQueries ?? throw new ArgumentNullException(nameof(seatQueries));
            _documentCreator = documentCreator;
            _computerService = computerService;
            SendTimesheetCommand = new RelayCommandAsync(ExecuteSendTimesheetAsync);
            DisplayTimesheetCommand = new RelayCommandAsync(ExecuteDisplayTimesheetAsync);            
        }        

        public override string Title => "Clôturer la session";

        public bool TimesheetAvailable => TimesheetId.HasValue;

        public string SessionTitle
        {
            get => _sessionTitle;
            set { Set(() => SessionTitle, ref _sessionTitle, value); }
        }

        public ObservableCollection<StudentItem> Seats
        {
            get => _seats;
            set { Set(()=>Seats, ref _seats, value); }
        }

        public override async Task Init()
        {
            var closedSessionTask = Task.Run(()=>_sessionQueries.GetClosedSession(_sessionId));
            var seatsTask = Task.Run(() => _seatQueries.GetValidatedSeats(_sessionId));

            await Task.WhenAll(closedSessionTask, seatsTask);

            var closedSessionResult = closedSessionTask.Result;
            var seatResult = seatsTask.Result.Where(a=>a.IsMissing == false).Select(a=>new StudentItem(this, a, _documentManager, _applicationService));

            _trainingName = closedSessionResult.TrainingName;
            _sessionStart = closedSessionResult.Start;

            SessionTitle = $"Session {_trainingName}" + Environment.NewLine +
                            $"du {_sessionStart:D}";
            
            if (closedSessionResult.TimesheetId.HasValue)
                TimesheetId = closedSessionResult.TimesheetId.Value;

            Seats = new ObservableCollection<StudentItem>(seatResult);
        }
        
        public RelayCommandAsync SendTimesheetCommand { get; }
        private async Task ExecuteSendTimesheetAsync()
        {
            var timesheetId = await _documentManager.SendDocument(documentId =>
            {
                _applicationService.Command<SendTimeSheet>().Execute(_sessionId, documentId);                
            });

            if( timesheetId.HasValue )
                TimesheetId = timesheetId.Value;
        }
        
        public RelayCommandAsync DisplayTimesheetCommand { get; }
        private async Task ExecuteDisplayTimesheetAsync()
        {
            if(TimesheetId.HasValue)
                await _documentManager.OpenDocument(TimesheetId.Value);
        }

        public async Task CreateFinalMailAsync(string companyName)
        {
            var firstSeat = Seats.FirstOrDefault(a => a.Company == companyName);
            if (firstSeat == null)
            {
                MessageBox.Show("Aucun stagiaire pour générer le mail", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!TimesheetId.HasValue)
            {
                MessageBox.Show("La feuille de présence n'est pas disponible", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
             
            if (!firstSeat.AgreementId.HasValue) {
                MessageBox.Show("La convention n'est pas disponible", "Erreur", MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            if (!Seats.Where(a => a.Company == companyName).All(b => b.CertificateOfAttendanceId.HasValue))
            {
                MessageBox.Show("Il manque un certificat d'assiduité", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            var documents = new List<MailAttachement>();

            var firstPage = _documentCreator.CreateFirstPage(_trainingName, _sessionStart, firstSeat.Company, firstSeat.Contact, firstSeat.Address, firstSeat.ZipCode, firstSeat.City);
            documents.Add(new MailAttachement(firstPage, "Courrier fin de stage"));

            var timesheet = await Task.Run(() => _documentManager.Repo.GetDocument(TimesheetId.Value));
            documents.Add(new MailAttachement(timesheet, "Feuille de présence"));

            var signedAgreement = await Task.Run(()=>_documentManager.Repo.GetDocument(firstSeat.AgreementId.Value));
            documents.Add(new MailAttachement(signedAgreement, "Convention signée"));

            foreach (var item in Seats.Where(a=>a.Company == companyName))
            {
                var certificat = await Task.Run(()=>_documentManager.Repo.GetDocument(item.CertificateOfAttendanceId.Value));
                documents.Add(new MailAttachement(certificat, "Certificat assiduité " + item.Student));
            }

            _computerService.OpenMailInOutlook($"Stage TREND - {_trainingName} du {_sessionStart:D}", 
                "Madame, Monsieur," + Environment.NewLine + Environment.NewLine +
                $"Suite au stage dispensé pour la formation «{_trainingName}» du {_sessionStart:D}, veuillez trouver, ci-joint, les documents de fin de stage suivants:" + Environment.NewLine + Environment.NewLine +
                "\t- La convention de formation dûment signée." + Environment.NewLine +
                "\t- La feuille de présence émargée." + Environment.NewLine +
                "\t- Les certificats d’assiduités." + Environment.NewLine + Environment.NewLine +
                "Nous vous souhaitons bonne réception de ces éléments et restons à votre disposition pour tout renseignement complémentaire." + Environment.NewLine + Environment.NewLine +
                "Cordialement,"
                , documents, firstSeat.Email);
                          
            await Task.Delay(0);
        }
    }

    public class DocumentManager
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentManager(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
        }

        public async Task<Guid?> SendDocument(Action<Guid> command)
        {
            var openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Pdf|*.pdf|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var documentPath = openFileDialog1.FileName;
                var documentId = await Task.Run(() => _documentRepository.Save(documentPath));
                await Task.Run(() => command(documentId));
                return documentId;
            }

            return null;
        }

        public async Task OpenDocument(Guid documentId)
        {
            var documentPath = await Task.Run(() => _documentRepository.GetDocument(documentId));
            Process.Start(documentPath);
        }

        public IDocumentRepository Repo => _documentRepository;
    }

    public class StudentItem : ViewModelBase
    {
        private readonly CloseSessionWindowVm _parent;
        private readonly DocumentManager _documentManager;
        private readonly IApplicationService _applicationService;
        private readonly Guid _seatId;        

        public StudentItem(CloseSessionWindowVm parent, ISeatValidatedResult seat, 
            DocumentManager documentManager, IApplicationService applicationService)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _documentManager = documentManager ?? throw new ArgumentNullException(nameof(documentManager));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));

            GenerateFinalMailCommand = new RelayCommandAsync(ExecuteGenerateFinalMailAsync, () => _parent.Seats.Where(a => a.Company == Company).All(b => b.AgreementAvailable && b.CertificatAvailable && _parent.TimesheetAvailable));

            Student = seat.Student.ToString();
            Company = seat.Company;

            Contact = seat.Contact;
            Email = seat.Email;
            Address = seat.Address;
            ZipCode = seat.ZipCode;
            City = seat.City;
            
                
            CertificateOfAttendanceId = seat.CertificateOfAttendanceId;
            AgreementId = seat.SignedAgreementId;
            
            _seatId = seat.SeatId;

            DisplayCertificatCommand = new RelayCommandAsync(ExecuteDisplayCertificatAsync);
            SendCertificatCommand = new RelayCommandAsync(ExecuteSendCertificatAsync);

            DisplayAgreementCommand = new RelayCommandAsync(ExecuteDisplayAgreementAsync);

            Messenger.Default.Register<RefreshButtonMessage>(this, e => GenerateFinalMailCommand.RaiseCanExecuteChanged());            
        }    

        public string Student { get; }
        public string Company { get; }

        public FullName Contact { get; }
        public string Email { get; }
        public string Address { get; }
        public string ZipCode { get; }
        public string City { get; }

        #region certificat
        private Guid? _certificateOfAttendanceId;
        public string CertificatStateIcon
        {
            get
            {
                if (CertificateOfAttendanceId.HasValue)
                    return "/Images/Apply_32x32.png";
                return "/Images/Cancel_32x32.png";
            }
        }
        public Guid? CertificateOfAttendanceId
        {
            get => _certificateOfAttendanceId;
            private set
            {
                _certificateOfAttendanceId = value; 
                RaisePropertiesChanged(nameof(CertificatStateIcon));
                RaisePropertiesChanged(nameof(CertificatAvailable));
                GenerateFinalMailCommand.RaiseCanExecuteChanged();
                Messenger.Default.Send(new RefreshButtonMessage());
            }
        }
        public bool CertificatAvailable => CertificateOfAttendanceId.HasValue;

        public RelayCommandAsync DisplayCertificatCommand { get; }
        private async Task ExecuteDisplayCertificatAsync()
        {
            if(CertificateOfAttendanceId.HasValue)
                await _documentManager.OpenDocument(CertificateOfAttendanceId.Value);
        }

        public RelayCommandAsync SendCertificatCommand { get; }
        private async Task ExecuteSendCertificatAsync()
        {
            var certificatId = await _documentManager.SendDocument( certifId =>
            {
                _applicationService.Command<SendCertificatOfAttendance>().Execute(_seatId, certifId);                
            });

            if( certificatId.HasValue )
                CertificateOfAttendanceId = certificatId;
        }
        #endregion

        #region agreement
        private Guid? _agreementId;
        public string AgreementStateIcon
        {
            get
            {
                if (AgreementId.HasValue)
                    return "/Images/Apply_32x32.png";
                return "/Images/Cancel_32x32.png";
            }
        }

        public Guid? AgreementId
        {
            get => _agreementId;
            private set
            {
                _agreementId = value;
                RaisePropertiesChanged(nameof(AgreementStateIcon));
                RaisePropertiesChanged(nameof(CertificatAvailable));
                GenerateFinalMailCommand.RaiseCanExecuteChanged();
            }
        }
        public bool AgreementAvailable => AgreementId.HasValue;
        public RelayCommandAsync DisplayAgreementCommand { get; }
        private async Task ExecuteDisplayAgreementAsync()
        {
            if (AgreementId.HasValue)
                await _documentManager.OpenDocument(AgreementId.Value);
        }
        #endregion

        public RelayCommandAsync GenerateFinalMailCommand { get; }
        private async Task ExecuteGenerateFinalMailAsync()
        {
            await _parent.CreateFinalMailAsync(Company);
        }
    }

    public class RefreshButtonMessage { }   
}