using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using GestionFormation.App.Core;
using GestionFormation.Applications.Trainings;
using GestionFormation.CoreDomain.Trainings.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class TrainingListVm : EditableListVm<EditableTraining>
    {
        private readonly ITrainingQueries _queries;

        public override string Title => "Liste des formations";

        public TrainingListVm(ITrainingQueries queries, IApplicationService applicationService) : base(applicationService)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        protected override async Task<IReadOnlyList<EditableTraining>> LoadAsync()
        {
            return await Task.Run(() => _queries.GetAll().Select(a=>new EditableTraining(a, this)).ToList());
        }

        protected override async Task CreateAsync(EditableTraining item)
        {
            if (item == null) return;
            await Task.Run(() => ApplicationService.Command<CreateTraining>().Execute(item.Name, item.Seats, ColorHelper.ToInt(item.Color)));
        }

        protected override async Task UpdateAsync(EditableTraining item)
        {
            if( item == null ) return;
            await Task.Run(()=> ApplicationService.Command<UpdateTraining>().Execute(item.GetId(), item.Name, item.Seats, ColorHelper.ToInt(item.Color)));
        }

        protected override async Task DeleteAsync(EditableTraining item)
        {
            if (item == null) return;
            await Task.Run(()=> ApplicationService.Command<DeleteTraining>().Execute(item.GetId()));
        }        
    }

    public class EditableTraining : EditableItem
    {
        private Color _color;
        private Color _previousColor;

        public EditableTraining()
        {
            
        }

        public EditableTraining(ITrainingResult result, TrainingListVm parent) : base(result.Id, parent)
        {
            Name = result.Name;
            Seats = result.Seats;
            _color = ColorHelper.FromInt(result.Color);
        }

        [DisplayName("Nom")]
        public string Name { get; set; }
        [DisplayName("Places")]
        public int Seats { get; set; }

        [DisplayName("Couleur")]
        public Color Color
        {
            get => _color;
            set
            {
                // BIG HACK due to devexpress, because it reset the color after pressing the UpdateButton.                
                if (value != _previousColor)
                {
                    _previousColor = _color;
                    _color = value;
                }
            }
        }       
    }
}