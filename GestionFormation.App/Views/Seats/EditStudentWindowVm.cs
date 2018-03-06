using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.App.Views.EditableLists;
using GestionFormation.Applications.Students;
using GestionFormation.CoreDomain.Students.Queries;

namespace GestionFormation.App.Views.Seats
{
    public class EditStudentWindowVm : PopupWindowVm
    {
        private ObservableCollection<Item> _students;
        private Item _selectedStudent;
        public override string Title => "Modifier le stagiaire";
        private Guid? _previousSelectedStudent;
        private readonly IApplicationService _applicationService;
        private readonly IStudentQueries _studentQueries;

        public EditStudentWindowVm(ObservableCollection<Item> students, Guid currentStudentId, IApplicationService applicationService, IStudentQueries studentQueries)
        {
            _previousSelectedStudent = currentStudentId;
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _studentQueries = studentQueries ?? throw new ArgumentNullException(nameof(studentQueries));
            SetValiderCommandCanExecute(()=>SelectedStudent?.Id != _previousSelectedStudent);

            CreateStudentCommand = new RelayCommandAsync(ExecuteCreateStudentAsync);
            Students = students;            
        }
        
        public ObservableCollection<Item> Students
        {
            get => _students;
            set { Set(() => Students, ref _students, value); }
        }

        public override Task Init()
        {
            if (_previousSelectedStudent.HasValue)
                SelectedStudent = Students.FirstOrDefault(a => a.Id == _previousSelectedStudent);

            return base.Init();
        }

        public Item SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                Set(() => SelectedStudent, ref _selectedStudent, value);
                ValiderCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommandAsync CreateStudentCommand { get; }
        private async Task ExecuteCreateStudentAsync()
        {
            var vm = await _applicationService.OpenPopup<CreateItemVm>("Créer un stagiaire", new EditableStudent());
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync(async () => {
                    var item = vm.Item as EditableStudent;
                    var newStudents = await Task.Run(() => _applicationService.Command<CreateStudent>().Execute(item.Lastname, item.Firstname));

                    var students = await Task.Run(() => _studentQueries.GetAll().Select(a => new Item { Id = a.Id, Label = a.Firstname + " " + a.Lastname }));
                    Students = new ObservableCollection<Item>(students);
                    SelectedStudent = Students.FirstOrDefault(a => a.Id == newStudents.AggregateId);
                });
            }
        }
    }
}