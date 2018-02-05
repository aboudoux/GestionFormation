using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Trainings;
using GestionFormation.CoreDomain.Trainings.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class FormationListVm : EditableListVm<EditableFormation>
    {
        private readonly ITrainingQueries _queries;

        public override string Title => "Liste des formations";

        public FormationListVm(ITrainingQueries queries, IApplicationService applicationService) : base(applicationService)
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
            await Task.Run(() => ApplicationService.Command<CreateTraining>().Execute(item.Nom, item.Places));
        }

        protected override async Task UpdateAsync(EditableFormation item)
        {
            if( item == null ) return;
            await Task.Run(()=> ApplicationService.Command<UpdateTraining>().Execute(item.GetId(), item.Nom, item.Places));
        }

        protected override async Task DeleteAsync(EditableFormation item)
        {
            if (item == null) return;
            await Task.Run(()=> ApplicationService.Command<DeleteTraining>().Execute(item.GetId()));
        }        
    }

    public class EditableFormation : EditableItem
    {
        public EditableFormation()
        {
            
        }

        public EditableFormation(ITrainingResult result) : base(result.Id)
        {
            Nom = result.Name;
            Places = result.Seats;
        }

        public string Nom { get; set; }
        public int Places { get; set; }
    }
}