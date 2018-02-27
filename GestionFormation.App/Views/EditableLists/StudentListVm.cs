using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Students;
using GestionFormation.CoreDomain.Students.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class StudentListVm : EditableListVm<EditableStudent>
    {
        private readonly IStudentQueries _studentQueries;

        public override string Title => "Liste des stagiaires";

        public StudentListVm(IApplicationService applicationService, IStudentQueries studentQueries) : base(applicationService)
        {
            _studentQueries = studentQueries ?? throw new ArgumentNullException(nameof(studentQueries));
        }

        protected override async Task<IReadOnlyList<EditableStudent>> LoadAsync()
        {
            return await Task.Run(()=>_studentQueries.GetAll().Select(a=>new EditableStudent(a)).ToList());
        }

        protected override async Task CreateAsync(EditableStudent item)
        {
            await Task.Run(()=> ApplicationService.Command<CreateStudent>().Execute(item.Lastname, item.Firstname));
        }

        protected override async Task UpdateAsync(EditableStudent item)
        {
            await Task.Run(()=>ApplicationService.Command<UpdateStudent>().Execute(item.GetId(), item.Lastname, item.Firstname));
        }

        protected override async Task DeleteAsync(EditableStudent item)
        {
            await Task.Run(() => ApplicationService.Command<DeleteStudent>().Execute(item.GetId()));
        }        
    }

    public class EditableStudent : EditableItem
    {
        public EditableStudent()
        {
            
        }
        public EditableStudent(IStudentResult result) : base(result.Id)
        {
            Lastname = result.Lastname;
            Firstname = result.Firstname;
        }


        [DisplayName("Nom")]
        public string Lastname { get; set; }
        [DisplayName("Prénom")]
        public string Firstname { get; set; }
    }
}