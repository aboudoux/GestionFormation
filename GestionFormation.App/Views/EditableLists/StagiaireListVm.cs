using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Stagiaires;
using GestionFormation.CoreDomain.Students.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class StagiaireListVm : EditableListVm<EditableStagiaire>
    {
        private readonly IStudentQueries _studentQueries;

        public override string Title => "Liste des stagiaires";

        public StagiaireListVm(IApplicationService applicationService, IStudentQueries studentQueries) : base(applicationService)
        {
            _studentQueries = studentQueries ?? throw new ArgumentNullException(nameof(studentQueries));
        }

        protected override async Task<IReadOnlyList<EditableStagiaire>> LoadAsync()
        {
            return await Task.Run(()=>_studentQueries.GetAll().Select(a=>new EditableStagiaire(a)).ToList());
        }

        protected override async Task CreateAsync(EditableStagiaire item)
        {
            await Task.Run(()=> ApplicationService.Command<CreateStagiaire>().Execute(item.Nom, item.Prenom));
        }

        protected override async Task UpdateAsync(EditableStagiaire item)
        {
            await Task.Run(()=>ApplicationService.Command<UpdateStagiaire>().Execute(item.GetId(), item.Nom, item.Prenom));
        }

        protected override async Task DeleteAsync(EditableStagiaire item)
        {
            await Task.Run(() => ApplicationService.Command<DeleteStagiaire>().Execute(item.GetId()));
        }        
    }

    public class EditableStagiaire : EditableItem
    {
        public EditableStagiaire()
        {
            
        }
        public EditableStagiaire(IStudentResult result) : base(result.Id)
        {
            Nom = result.Lastname;
            Prenom = result.Firstname;
        }

        public string Nom { get; set; }
        public string Prenom { get; set; }
    }
}