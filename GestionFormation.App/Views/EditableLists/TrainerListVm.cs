using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Trainers;
using GestionFormation.CoreDomain.Trainers.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class TrainerListVm : EditableListVm<EditableTrainer>
    {
        private readonly ITrainerQueries _trainerQueries;

        public TrainerListVm(IApplicationService applicationService, ITrainerQueries trainerQueries) : base(applicationService)
        {
            _trainerQueries = trainerQueries ?? throw new ArgumentNullException(nameof(trainerQueries));
        }

        protected override async Task<IReadOnlyList<EditableTrainer>> LoadAsync()
        {
            return await Task.Run(() => _trainerQueries.GetAll().Select(a=>new EditableTrainer(a)).ToList());
        }

        protected override async Task CreateAsync(EditableTrainer item)
        {            
            await Task.Run(()=> ApplicationService.Command<CreateTrainer>().Execute(item.Lastname, item.Firstname, item.Email));
        }

        protected override async Task UpdateAsync(EditableTrainer item)
        {
            await Task.Run(() => ApplicationService.Command<UpdateTrainer>().Execute(item.GetId(), item.Lastname, item.Firstname, item.Email) );            
        }

        protected override async Task DeleteAsync(EditableTrainer item)
        {
            await Task.Run(() => ApplicationService.Command<DeleteTrainer>().Execute(item.GetId()));            
        }

        public override string Title => "Liste des formateurs";
    }

    public class EditableTrainer : EditableItem
    {
        public EditableTrainer()
        {
            
        }
        public EditableTrainer(ITrainerResult result) : base(result.Id)
        {
            Lastname = result.Lastname;
            Firstname = result.Firstname;
            Email = result.Email;
        }

        [DisplayName("Nom")]
        public string Lastname { get; set; }
        [DisplayName("Prénom")]
        public string Firstname { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
    }
}