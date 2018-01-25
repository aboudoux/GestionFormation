using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GestionFormation.App.Core;
using GestionFormation.App.Views.EditableLists;
using GestionFormation.Applications.Formateurs;
using GestionFormation.Applications.Formations;
using GestionFormation.Applications.Lieux;
using GestionFormation.Applications.Sessions;
using GestionFormation.CoreDomain.Formateurs.Queries;
using GestionFormation.CoreDomain.Formations.Queries;
using GestionFormation.CoreDomain.Lieux.Queries;

namespace GestionFormation.App.Views.Sessions
{
    public class CreateSessionWindowVm : PopupWindowVm
    {
        private readonly AppointmentItem _appointmentItem;
        private readonly IApplicationService _applicationService;
        private readonly ILieuQueries _lieuQueries;
        private readonly IFormateurQueries _formateurQueries;
        private readonly IFormationQueries _formationQueries;
        private ObservableCollection<FormationItem> _formations;
        private FormationItem _selectedFormation;
        private ObservableCollection<FormateurItem> _formateurs;
        private FormateurItem _selectedFormateur;
        private ObservableCollection<LieuItem> _lieux;
        private LieuItem _selectedLieu;
        private DateTime? _dateDebut;
        private int _durée;
        private int _places;
        private Guid? _sessionId;        

        public string Title => "Planifier une nouvelle session";

        public CreateSessionWindowVm(IApplicationService applicationService, IFormationQueries formationQueries, IFormateurQueries formateurQueries, ILieuQueries lieuQueries, AppointmentItem appointmentItem)
        {
            _appointmentItem = appointmentItem ?? throw new ArgumentNullException(nameof(appointmentItem));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _lieuQueries = lieuQueries ?? throw new ArgumentNullException(nameof(lieuQueries));
            _formateurQueries = formateurQueries ?? throw new ArgumentNullException(nameof(formateurQueries));
            _formationQueries = formationQueries ?? throw new ArgumentNullException(nameof(formationQueries));

            AddFormationCommand = new RelayCommandAsync(ExecuteAddFormationAsync);
            AddLieuCommand = new RelayCommandAsync(ExecuteAddLieuAsync);
            AddFormateurCommand = new RelayCommandAsync(ExecuteAddFormateurAsync);
        }        

        public ObservableCollection<FormationItem> Formations
        {
            get => _formations;
            set{Set(() => Formations, ref _formations, value);                }
        }

        public FormationItem SelectedFormation
        {
            get => _selectedFormation;
            set
            {
                Set(() => SelectedFormation, ref _selectedFormation, value);

                if(value == null) return;
                if (Places == 0 || value.Places < Places)
                    Places = value.Places;
            }
        }

        public ObservableCollection<FormateurItem> Formateurs
        {
            get => _formateurs;
            set { Set(() => Formateurs, ref _formateurs, value); }
        }

        public FormateurItem SelectedFormateur
        {
            get => _selectedFormateur;
            set { Set(() => SelectedFormateur, ref _selectedFormateur, value); }
        }

        public ObservableCollection<LieuItem> Lieux
        {
            get => _lieux;
            set { Set(() => Lieux, ref _lieux, value); }
        }

        public LieuItem SelectedLieu
        {
            get => _selectedLieu;
            set
            {
                Set(()=>SelectedLieu, ref _selectedLieu, value );

                if (value == null) return;
                if (Places == 0 || value.Places < Places)
                    Places = value.Places;
            }
        }

        protected override async Task ExecuteValiderAsync()
        {
            var error = false;
            if (SelectedFormation == null)
            {
                MessageBox.Show("Vous n'avez pas renseigné la formation", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                error = true;
            }
            else if (SelectedFormateur == null)
            {
                MessageBox.Show("Vous n'avez pas renseigné le formateur", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                error = true;
            }
            else if (SelectedLieu == null)
            {
                MessageBox.Show("Vous n'avez pas renseigné le lieu", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                error = true;
            }

            if (Places > SelectedFormation?.Places)
            {
                if (MessageBox.Show("Vous avez défini plus de places que ne peut en accueillir la formation.\r\nEtes-vous sûr de vouloir valider ?", "ATTENTION", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            }
            if (Places > SelectedLieu?.Places)
            {
                if (MessageBox.Show("Vous avez défini plus de places que ne peut en accueillir la lieu.\r\nEtes-vous sûr de vouloir valider ?", "ATTENTION", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            }

            if (!error)
            {
                try
                {
                    if (_sessionId.HasValue)
                        await Task.Run(() => _applicationService.Command<UpdateSession>().Execute(_sessionId.Value, SelectedFormation.Id, DateDebut.Value, Durée, Places, SelectedLieu.Id, SelectedFormateur.Id));
                    else
                        await Task.Run(() => _applicationService.Command<PlanSession>().Execute(SelectedFormation.Id, DateDebut.Value, Durée, Places, SelectedLieu.Id, SelectedFormateur.Id));

                    await base.ExecuteValiderAsync();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.LastMessage(), "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
            
        public DateTime? DateDebut
        {
            get => _dateDebut;
            set { Set(() => DateDebut, ref _dateDebut, value); }
        }

        public int Durée
        {
            get => _durée;
            set { Set(()=>Durée, ref _durée, value); }
        }

        public int Places
        {
            get => _places;
            set
            {
                Set(() => Places, ref _places, value);                
            }
        }

        public RelayCommandAsync AddFormationCommand { get; }
        private async Task ExecuteAddFormationAsync()
        {
            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer une formation", new EditableFormation());
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.Execute( async ()=>{
                    var item = vm.Item as EditableFormation;
                    var newItem = await Task.Run(() => _applicationService.Command<CreateFormation>().Execute(item.Nom, item.Places));
                    await InitFormations(newItem.AggregateId);
                });
            }
        }

        public RelayCommandAsync AddLieuCommand { get; }
        private async Task ExecuteAddLieuAsync()
        {
            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer un lieu", new EditableLieu());
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.Execute(async () => {                   
                    var item = vm.Item as EditableLieu;
                    var newItem = await Task.Run(() => _applicationService.Command<CreateLieu>().Execute(item.Nom, item.Addresse, item.Places));
                    await InitLieux(newItem.AggregateId);
                });
            }
        }

        public RelayCommandAsync AddFormateurCommand { get; }
        private async Task ExecuteAddFormateurAsync()
        {
            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer un formateur", new EditableFormateur());
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.Execute(async () => {
                    var item = vm.Item as EditableFormateur;
                    var newItem = await Task.Run(() => _applicationService.Command<CreateFormateur>().Execute(item.Nom, item.Prenom, item.Email));
                    await InitFormateurs(newItem.AggregateId);
                });
            }
        }

        public override async Task Init()
        {
            var t1 = InitFormations(_appointmentItem.FormationId);
            var t2 = InitFormateurs(_appointmentItem.FormateurId);
            var t3 = InitLieux(_appointmentItem.LieuId);

            DateDebut = _appointmentItem.Start;
            Durée = _appointmentItem.Durée;
            _sessionId = _appointmentItem.SessionId;

            await Task.WhenAll(t1, t2, t3);

            Places = _appointmentItem.Places;
        }

        private async Task InitFormations(Guid? selectedFormationId)
        {
            var formationsTask = await Task.Run(() => _formationQueries.GetAll().Select(a => new FormationItem(a)));
            Formations = new ObservableCollection<FormationItem>(formationsTask);
            SelectedFormation = Formations.FirstOrDefault(a => a.Id == selectedFormationId);
        }

        private async Task InitFormateurs(Guid? selectedFormateurId)
        {
            var formateursTask = await Task.Run(() => _formateurQueries.GetAll().Select(a => new FormateurItem(a)));
            Formateurs = new ObservableCollection<FormateurItem>(formateursTask);
            SelectedFormateur = Formateurs.FirstOrDefault(a => a.Id == selectedFormateurId);
        }

        private async Task InitLieux(Guid? selectedLieuId)
        {
            var lieuxTask = await Task.Run(() => _lieuQueries.GetAll().Select(a => new LieuItem(a)));
            Lieux = new ObservableCollection<LieuItem>(lieuxTask);
            SelectedLieu = Lieux.FirstOrDefault(a => a.Id == selectedLieuId);
        }
    }

    public class FormateurItem
    {
        private readonly IFormateurResult _result;

        public FormateurItem(IFormateurResult result)
        {
            _result = result;
        }

        public Guid Id => _result.Id;

        public override string ToString()
        {
            return _result.Prenom + " " + _result.Nom;
        }
    }

    public class FormationItem
    {
        private readonly IFormationResult _result;

        public FormationItem(IFormationResult result)
        {
            _result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public Guid Id => _result.Id;
        public int Places => _result.Places;

        public override string ToString()
        {
            return _result.Nom;
        }
    }

    public class LieuItem
    {
        private readonly ILieuResult _result;

        public LieuItem(ILieuResult result)
        {
            _result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public Guid Id => _result.LieuId;
        public int Places => _result.Places;

        public override string ToString()
        {
            return _result.Nom;
        }
    }

    public class AppointmentItem
    {
        public AppointmentItem(DateTime? start, int durée, Guid? formateurId, Guid? lieuId, Guid? formationId, int places, Guid? sessionId)
        {
            Start = start;
            Durée = durée;
            FormateurId = formateurId;
            LieuId = lieuId;
            FormationId = formationId;
            Places = places;
            SessionId = sessionId;
        }

        public DateTime? Start { get; }
        public int Durée { get; }
        public Guid? FormationId { get; }
        public Guid? FormateurId { get; }
        public Guid? LieuId { get; }
        public int Places { get; }
        public Guid? SessionId { get; }
    }
}
