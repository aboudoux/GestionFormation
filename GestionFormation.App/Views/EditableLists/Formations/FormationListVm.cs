using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using GestionFormation.App.Core;
using GestionFormation.Applications.Trainings;
using GestionFormation.CoreDomain.Trainings.Queries;

namespace GestionFormation.App.Views.EditableLists.Formations
{
    public class FormationListVm : EditableListVm<EditableFormation, EditableFormation, CreateFormationWindowVm>
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
            await Task.Run(() => ApplicationService.Command<CreateTraining>().Execute(item.Nom, item.Places, ColorHelper.ToInt(item.Couleur)));
        }

        protected override async Task UpdateAsync(EditableFormation item)
        {
            if( item == null ) return;
            await Task.Run(()=> ApplicationService.Command<UpdateTraining>().Execute(item.GetId(), item.Nom, item.Places, ColorHelper.ToInt(item.Couleur)));
        }

        protected override async Task DeleteAsync(EditableFormation item)
        {
            if (item == null) return;
            await Task.Run(()=> ApplicationService.Command<DeleteTraining>().Execute(item.GetId()));
        }        
    }

    public class EditableFormation : EditableItem
    {
        private Color _couleur;
        private Color _previousColor;

        public EditableFormation()
        {
            
        }

        public EditableFormation(ITrainingResult result) : base(result.Id)
        {
            Nom = result.Name;
            Places = result.Seats;
            _couleur = ColorHelper.FromInt(result.Color);
        }

        public string Nom { get; set; }
        public int Places { get; set; }

        public System.Windows.Media.Color Couleur
        {
            get => _couleur;
            set
            {
                // BIG HACK due to devexpress, because it reset the color after pressing the UpdateButton.                
                if (value != _previousColor)
                {
                    _previousColor = _couleur;
                    _couleur = value;
                }
            }
        }
    }
}