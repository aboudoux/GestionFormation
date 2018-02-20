using System;
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
using GestionFormation.Kernel;

namespace GestionFormation.App.Views.Sessions
{
    public class ClotureWindowVm : PopupWindowVm
    {
        private readonly Guid _sessionId;
        private readonly IApplicationService _applicationService;
        private readonly ISessionQueries _sessionQueries;
        private readonly ISeatQueries _seatQueries;
        private bool _surveyAvailable;
        private bool _timesheetAvailable;
        private readonly DocumentManager _documentManager;

        private Guid _surveyId;
        private Guid SurveyId
        {
            get => _surveyId;
            set
            {
                _surveyId = value;
                SurveyAvailable = true;
            }
        }

        private Guid _timesheetId;
        private string _sessionTitle;
        private ObservableCollection<StudentItem> _seats;

        private Guid TimesheetId
        {
            get => _timesheetId;
            set
            {
                _timesheetId = value;
                TimesheetAvailable = true;
            }
        }

        public ClotureWindowVm(Guid sessionId, IApplicationService applicationService, IDocumentRepository documentRepository, ISessionQueries sessionQueries, ISeatQueries seatQueries)
        {
            GuidAssert.AreNotEmpty(sessionId);
            _sessionId = sessionId;
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _documentManager = new DocumentManager(documentRepository);
            _sessionQueries = sessionQueries ?? throw new ArgumentNullException(nameof(sessionQueries));
            _seatQueries = seatQueries ?? throw new ArgumentNullException(nameof(seatQueries));
            SendSurveyCommand = new RelayCommandAsync(ExecuteSendSurveyAsync);
            SendTimesheetCommand = new RelayCommandAsync(ExecuteSendTimesheetAsync);
            DisplaySurveyCommand = new RelayCommandAsync(ExecuteDisplaySurveyAsync);
            DisplayTimesheetCommand = new RelayCommandAsync(ExecuteDisplayTimesheetAsync);            
        }

        
        public override string Title => "Clôturer la session";

        public bool SurveyAvailable
        {
            get => _surveyAvailable;
            set { Set(()=>SurveyAvailable, ref _surveyAvailable, value); }
        }

        public bool TimesheetAvailable
        {
            get => _timesheetAvailable;
            set { Set(()=>TimesheetAvailable, ref _timesheetAvailable, value); }
        }

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
            var seatResult = seatsTask.Result.Where(a=>a.IsMissing == false).Select(a=>new StudentItem(a, _documentManager, _applicationService));

            SessionTitle = $"Session {closedSessionResult.TrainingName}" + Environment.NewLine +
                            $"du {closedSessionResult.Start:D}";

            if(closedSessionResult.SurveyId.HasValue)
                SurveyId = closedSessionResult.SurveyId.Value;
            if (closedSessionResult.TimesheetId.HasValue)
                TimesheetId = closedSessionResult.TimesheetId.Value;

            Seats = new ObservableCollection<StudentItem>(seatResult);
        }

        public RelayCommandAsync SendSurveyCommand { get; }
        private async Task ExecuteSendSurveyAsync()
        {
            await _documentManager.SendDocument(documentId =>
            {
                _applicationService.Command<SendSurvey>().Execute(_sessionId, documentId);
                SurveyId = documentId;
            });
        }

        public RelayCommandAsync SendTimesheetCommand { get; }
        private async Task ExecuteSendTimesheetAsync()
        {
            await _documentManager.SendDocument(documentId =>
            {
                _applicationService.Command<SendTimeSheet>().Execute(_sessionId, documentId);
                TimesheetId = documentId;
            });
        }

        public RelayCommandAsync DisplaySurveyCommand { get; }
        private async Task ExecuteDisplaySurveyAsync()
        {
            await _documentManager.OpenDocument(SurveyId);
        }
        public RelayCommandAsync DisplayTimesheetCommand { get; }
        private async Task ExecuteDisplayTimesheetAsync()
        {
            await _documentManager.OpenDocument(TimesheetId);
        }
        
    }

    public class DocumentManager
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentManager(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
        }

        public async Task SendDocument(Action<Guid> command)
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
            }
        }

        public async Task OpenDocument(Guid documentId)
        {
            var documentPath = await Task.Run(() => _documentRepository.GetDocument(documentId));
            Process.Start(documentPath);
        }
    }

    public class StudentItem : ViewModelBase
    {
        private readonly DocumentManager _documentManager;
        private readonly IApplicationService _applicationService;
        private readonly Guid _seatId;
        private Guid? _certificateOfAttendanceId;

        public StudentItem(ISeatValidatedResult seat, DocumentManager documentManager, IApplicationService applicationService)
        {
            _documentManager = documentManager ?? throw new ArgumentNullException(nameof(documentManager));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            Student = seat.Student.ToString();
            Company = seat.Company;
            CertificateOfAttendanceId = seat.CertificateOfAttendanceId;
            _seatId = seat.SeatId;

            DisplayCertificatCommand = new RelayCommandAsync(ExecuteDisplayCertificatAsync);
            SendCertificatCommand = new RelayCommandAsync(ExecuteSendCertificatAsync);
        }

        public string Student { get; }
        public string Company { get; }

        public string Icon
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
                RaisePropertiesChanged(nameof(Icon));
                RaisePropertiesChanged(nameof(CertificatAvailable));
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
            await _documentManager.SendDocument( certifId =>
            {
                _applicationService.Command<SendCertificatOfAttendance>().Execute(_seatId, certifId);
                CertificateOfAttendanceId = certifId;
            });
        }
    }
}