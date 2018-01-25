using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Formations;
using GestionFormation.CoreDomain.Formations.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class FormationListVm : EditableListVm<EditableFormation>
    {
        private readonly IFormationQueries _queries;

        public override string Title => "Liste des formations";

        public FormationListVm(IFormationQueries queries, IApplicationService applicationService) : base(applicationService)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        protected override async Task<IReadOnlyList<EditableFormation>> LoadAsync()
        {
            return await Task.Run(() => _queries.GetAll().Select(a=>new EditableFormation(a)).ToList());
        }

        protected override async Task CreateAsync(EditableFormation item)
        {
            if (item == null) return;
            await Task.Run(() => ApplicationService.Command<CreateFormation>().Execute(item.Nom, item.Places));
        }

        protected override async Task UpdateAsync(EditableFormation item)
        {
            if( item == null ) return;
            await Task.Run(()=> ApplicationService.Command<UpdateFormation>().Execute(item.GetId(), item.Nom, item.Places));
        }

        protected override async Task DeleteAsync(EditableFormation item)
        {
            if (item == null) return;
            await Task.Run(()=> ApplicationService.Command<DeleteFormation>().Execute(item.GetId()));
        }        
    }

    public class EditableFormation : EditableItem
    {
        public EditableFormation()
        {
            
        }

        public EditableFormation(IFormationResult result) : base(result.Id)
        {
            Nom = result.Nom;
            Places = result.Places;
        }

        public string Nom { get; set; }
        public int Places { get; set; }
    }
}